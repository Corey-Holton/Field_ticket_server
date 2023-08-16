using AutoMapper;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CA.Ticketing.Business.Services.Tickets
{
    public class TicketService : EntityServiceBase, ITicketService
    {
        private readonly IUserContext _userContext;

        public TicketService(CATicketingContext context, IMapper mapper, IUserContext userContext) : base(context, mapper)
        {
            _userContext = userContext;
        }
        
        public async Task<IEnumerable<TicketDto>> GetAll()
        {
            var tickets = await GetTicketIncludes().ToListAsync();
            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<IEnumerable<TicketDto>> GetByDates(DateTime startDate, DateTime endDate)
        {
            var tickets = await GetTicketIncludes()
               .Where(x => x.ExecutionDate >= startDate && x.ExecutionDate <= endDate)
               .ToListAsync();
            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<TicketDetailsDto> GetById(int id)
        {
            var ticket = await GetTicket(id);
            return _mapper.Map<TicketDetailsDto>(ticket);
        }

        public async Task<int> Create(ManageTicketDto manageTicketDto)
        {
            var createdByUserCount = await _context.FieldTickets
                .Where(x => x.CreatedBy == _userContext.User!.Id)
                .CountAsync();

            var ticket = new FieldTicket
            {
                ExecutionDate = manageTicketDto.ExecutionDate,
                ServiceType = manageTicketDto.ServiceType,
                CustomerId = manageTicketDto.CustomerId,
                LocationId = manageTicketDto.CustomerLocationId,
                EquipmentId = manageTicketDto.EquipmentId,
                TicketId = $"{_userContext.User!.TicketIdentifier}-{createdByUserCount + 1}",
                CreatedBy = _userContext.User!.Id
            };

            await GenerateCharges(ticket);

            if (ticket.ServiceType != ServiceType.PAndA && ticket.ServiceType != ServiceType.Yard)
            {
                var settings = _context.Settings.First();
                ticket.TaxRate = settings.TaxRate;
            }

            var employees = await _context.Employees
                .Where(x => x.AssignedRigId == manageTicketDto.EquipmentId)
                .ToListAsync();

            foreach (var employee in employees)
            {
                ticket.PayrollData.Add(new PayrollData { EmployeeId = employee.Id, Name = employee.DisplayName });
            }

            _context.FieldTickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket.Id;
        }

        public async Task Update(ManageTicketDto manageTicketDto)
        {
            var ticket = await GetTicket(manageTicketDto.Id);

            VerifyCharges(ticket, manageTicketDto);

            _mapper.Map(manageTicketDto, ticket);

            await GenerateCharges(ticket);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateHours(ManageTicketHoursDto manageTicketHours)
        {
            var ticket = await GetTicket(manageTicketHours.TicketId);
            _mapper.Map(manageTicketHours, ticket);

            CalculateCharges(ticket);
            
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var ticket = await GetTicket(id);
            _context.FieldTickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task AddPayroll(PayrollDataDto payrollDataDto)
        {
            var ticket = await _context.FieldTickets
                .Include(x => x.PayrollData)
                .SingleOrDefaultAsync(x => x.Id == payrollDataDto.FieldTicketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            var alreadyExists = payrollDataDto.EmployeeId.HasValue ? 
                ticket.PayrollData
                    .Any(x => x.EmployeeId == payrollDataDto.EmployeeId) :
                ticket.PayrollData
                    .Any(x => string.Equals(x.Name, payrollDataDto.Name, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
            {
                throw new Exception("There is already an employee with the same id or name added");
            }

            ticket.PayrollData.Add(_mapper.Map<PayrollData>(payrollDataDto));

            await _context.SaveChangesAsync();
        }

        public async Task RemovePayroll(int payrollDataId)
        {
            var payrollData = _context.PayrollData
                .SingleOrDefault(x => x.Id == payrollDataId);

            if (payrollData == null)
            {
                throw new KeyNotFoundException(nameof(PayrollData));
            }

            _context.Entry(payrollData).State = EntityState.Deleted;

            await _context.SaveChangesAsync();
        }

        public async Task<UpdateTicketSpecResponse> UpdateTicketSpecification(TicketSpecificationDto ticketSpecificationDto)
        {
            var ticketSpec = await _context.TicketSpecifications
                .SingleOrDefaultAsync(x => x.Id == ticketSpecificationDto.Id);

            if (ticketSpec == null)
            {
                throw new KeyNotFoundException(nameof(TicketSpecification));
            }

            if (!ticketSpec.AllowUoMChange && ticketSpec.UoM != ticketSpecificationDto.UoM)
            {
                throw new Exception("You are not allowed to modify Unit of Measure on this charge");
            }

            if (!ticketSpec.AllowRateAdjustment && ticketSpec.Rate != ticketSpecificationDto.Rate)
            {
                throw new Exception("You are not allowed to modify rate on this charge");
            }

            if (ChargesInfo.ReadonlyCharges.Contains(ticketSpec.Charge))
            {
                throw new Exception("This is a readonly charge");
            }

            _mapper.Map(ticketSpecificationDto, ticketSpec);

            await _context.SaveChangesAsync();

            var ticketTotal = (await _context.FieldTickets
                .Include(x => x.TicketSpecifications)
                .SingleAsync(x => x.Id == ticketSpec.FieldTicketId))
                .TicketSpecifications.Sum(x => x.Quantity * x.Rate);

            return new UpdateTicketSpecResponse
            {
                TicketTotal = ticketTotal,
                TicketSpecification = _mapper.Map<TicketSpecificationDto>(ticketSpec)
            };
        }

        private void VerifyCharges(FieldTicket ticket, ManageTicketDto manageTicketDto)
        {
            if (ticket.ServiceType == manageTicketDto.ServiceType)
            {
                return;
            }

            if (manageTicketDto.ServiceType == ServiceType.Yard)
            {
                ticket.TicketSpecifications.Clear();
            }
        }

        private async Task GenerateCharges(FieldTicket ticket)
        {
            if (ticket.ServiceType == ServiceType.Yard || ticket.TicketSpecifications.Any())
            {
                return;
            }

            var allCharges = await _context.Charges
                    .Where(x => x.IncludeInTicketSpecs)
                    .OrderBy(x => x.Order)
                    .ToListAsync();
            var rigCharges = await _context.EquipmentCharges
                .Include(x => x.Charge)
                .Where(x => x.EquipmentId == ticket.EquipmentId)
                .ToListAsync();

            foreach (var charge in allCharges)
            {
                TicketSpecification? ticketSpec = null;

                if (charge.IsRigSpecific)
                {
                    var rigCharge = rigCharges.Single(x => x.ChargeId == charge.Id);
                    ticketSpec = _mapper.Map<TicketSpecification>(rigCharge);
                }
                else
                {
                    ticketSpec = _mapper.Map<TicketSpecification>(charge);
                }

                ticket.TicketSpecifications.Add(ticketSpec);
            }

            if (ticket.ServiceType != ServiceType.Roustabout)
            {
                UpdateChargeQuantity(ticket, ChargeNames.ToolPusher, 1);
                UpdateChargeQuantity(ticket, ChargeNames.TravelTime, 1);
            }
        }

        private void CalculateCharges(FieldTicket ticket)
        {
            if (!ticket.TicketSpecifications.Any())
            {
                return;
            }

            var totalTime = ticket.StartTime.HasValue && ticket.EndTime.HasValue ? (ticket.EndTime.Value - ticket.StartTime.Value).TotalHours : 0;

            if (ticket.ServiceType == ServiceType.StandBy)
            {
                UpdateChargeQuantity(ticket, ChargeNames.Fuel, 0);
                return;
            }

            if (ticket.ServiceType == ServiceType.Roustabout)
            {
                UpdateChargeQuantity(ticket, ChargeNames.Rig, 0);
                UpdateChargeQuantity(ticket, ChargeNames.Fuel, 0);
                return;
            }

            var settings = _context.Settings.First();

            UpdateChargeQuantity(ticket, ChargeNames.Rig, totalTime);
            UpdateChargeQuantity(ticket, ChargeNames.Fuel, totalTime * settings.FuelCalculationMultiplier);
        }

        private void UpdateChargeQuantity(FieldTicket ticket, string chargeName, double chargeQuantity)
        {
            var charge = ticket.TicketSpecifications
                .SingleOrDefault(x => x.Charge == chargeName);
            if (charge == null)
            {
                return;
            }
            charge.Quantity = chargeQuantity;
        }

        private async Task<FieldTicket> GetTicket(int id)
        {
            var ticket = await GetTicketIncludes(true)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            return ticket;
        }

        private IQueryable<FieldTicket> GetTicketIncludes(bool includeSpecs = false)
        {
            var baseIncludes = _context.FieldTickets
                .Include(x => x.Customer)
                .Include(x => x.Equipment)
                .Include(x => x.Location);

            if (!includeSpecs)
            {
                return baseIncludes;
            }

            return baseIncludes
                .Include(x => x.TicketSpecifications)
                .Include(x => x.PayrollData)
                    .ThenInclude(p => p.Employee);
        }
    }
}
