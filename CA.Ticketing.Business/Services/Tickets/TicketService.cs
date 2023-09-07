using AutoMapper;
using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.FileManager;
using CA.Ticketing.Business.Services.Pdf;
using CA.Ticketing.Business.Services.Pdf.Dto;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace CA.Ticketing.Business.Services.Tickets
{
    public class TicketService : EntityServiceBase, ITicketService
    {
        private readonly IUserContext _userContext;

        private readonly IRazorViewToStringRenderer _viewRenderer;

        private readonly IPdfGeneratorService _pdfGeneratorService;

        private readonly IFileManagerService _fileManagerService;

        private readonly string _ticketTemplate = "/Views/Tickets/TicketTemplate.cshtml";

        public TicketService(
            CATicketingContext context, 
            IMapper mapper, 
            IUserContext userContext,
            IRazorViewToStringRenderer viewRenderer,
            IPdfGeneratorService pdfGeneratorService,
            IFileManagerService fileManagerService) : base(context, mapper)
        {
            _userContext = userContext;
            _viewRenderer = viewRenderer;
            _pdfGeneratorService = pdfGeneratorService;
            _fileManagerService = fileManagerService;
        }
        
        public async Task<IEnumerable<TicketDto>> GetAll()
        {
            Expression<Func<FieldTicket, bool>> ticketsFilter = x => true;

            if (_userContext.User!.Role == ApplicationRole.ToolPusher)
            {
                ticketsFilter = x => x.CreatedBy == _userContext.User.Id;
            }

            if (_userContext.User!.Role == ApplicationRole.Customer)
            {
                var customerId = (await _context.CustomerContacts
                    .FirstAsync(x => x.Id == _userContext.User.CustomerContactId)).CustomerId;

                ticketsFilter = x => x.CustomerId == customerId;
            }

            var tickets = await GetTicketIncludes()
                .Where(ticketsFilter)
                .ToListAsync();

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

            VerifyCanUpdateTicket(ticket);

            VerifyCharges(ticket, manageTicketDto);

            _mapper.Map(manageTicketDto, ticket);

            await GenerateCharges(ticket);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateHours(ManageTicketHoursDto manageTicketHours)
        {
            var ticket = await GetTicket(manageTicketHours.TicketId);

            VerifyCanUpdateTicket(ticket);

            _mapper.Map(manageTicketHours, ticket);

            CalculateCharges(ticket);
            
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var ticket = await GetTicket(id);

            VerifyCanUpdateTicket(ticket);

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

            VerifyCanUpdateTicket(ticket);

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

        public async Task UpdatePayrollData(PayrollDataDto payrollDataDto)
        {
            var payrollData = await _context.PayrollData
                .Include(x => x.FieldTicket)
                .SingleOrDefaultAsync(x => x.Id == payrollDataDto.Id);

            if (payrollData == null)
            {
                throw new KeyNotFoundException(nameof(PayrollData));
            }

            VerifyCanUpdateTicket(payrollData.FieldTicket);

            payrollData.RoustaboutHours = payrollDataDto.RoustaboutHours;
            payrollData.YardHours = payrollDataDto.YardHours;

            await _context.SaveChangesAsync();
        }

        public async Task RemovePayroll(int payrollDataId)
        {
            var payrollData = _context.PayrollData
                .Include(x => x.FieldTicket)
                .SingleOrDefault(x => x.Id == payrollDataId);

            if (payrollData == null)
            {
                throw new KeyNotFoundException(nameof(PayrollData));
            }

            VerifyCanUpdateTicket(payrollData.FieldTicket);

            _context.Entry(payrollData).State = EntityState.Deleted;

            await _context.SaveChangesAsync();
        }

        public async Task<UpdateTicketSpecResponse> UpdateTicketSpecification(TicketSpecificationDto ticketSpecificationDto)
        {
            var ticketSpec = await _context.TicketSpecifications
                .Include(x => x.FieldTicket)
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

            VerifyCanUpdateTicket(ticketSpec.FieldTicket);

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

        public async Task EmployeeSignature(SignatureBaseDto signature)
        {
            var fieldTicket = await GetTicket(signature.TicketId);

            if (fieldTicket.SignedOn.HasValue)
            {
                throw new Exception("This document is already signed.");
            }

            var user = await _context.Users.SingleAsync(x => x.Id == _userContext.User!.Id);

            if (!string.IsNullOrEmpty(user.Signature) && string.IsNullOrEmpty(signature.Signature))
            {
                throw new Exception("User has no signature defined");
            }

            if (!string.IsNullOrEmpty(signature.Signature))
            {
                user.Signature = signature.Signature;
            }

            fieldTicket.SignedBy = _userContext.User!.Id;
            fieldTicket.SignedOn = DateTime.UtcNow;
            fieldTicket.EmployeeSignature = user.Signature;
            fieldTicket.EmployeePrintedName = user.DisplayName;

            await _context.SaveChangesAsync();
        }

        public async Task<string> PreviewTicket(int ticketId)
        {
            var fieldTicket = await GetTicket(ticketId);

            if (fieldTicket.CustomerSignedOn.HasValue)
            {
                throw new Exception("This document is already signed");
            }

            var model = new TicketReport(fieldTicket);
            var ticketPreviewHtml = await _viewRenderer.RenderViewToStringAsync(_ticketTemplate, model);
            return ticketPreviewHtml;
        }

        public async Task CustomerSignature(CustomerSignatureDto customerSignatureDto)
        {
            var fieldTicket = await GetTicket(customerSignatureDto.TicketId);

            if (!fieldTicket.SignedOn.HasValue)
            {
                throw new Exception("Ticket is not yet signed");
            }

            fieldTicket.CustomerSignedOn = DateTime.UtcNow;
            fieldTicket.CustomerPrintedName = customerSignatureDto.PrintedName;

            if (_userContext.User!.Role == ApplicationRole.Customer)
            {
                fieldTicket.CustomerSignedBy = _userContext.User!.Id;
            }

            var model = new TicketReport(fieldTicket, customerSignatureDto.Signature);
            var ticketPreviewHtml = await _viewRenderer.RenderViewToStringAsync(_ticketTemplate, model);

            var pdfGenerated = _pdfGeneratorService.GeneratePdf(ticketPreviewHtml);

            _fileManagerService.SaveTicketFile(pdfGenerated, fieldTicket.FileName);

            await _context.SaveChangesAsync();
        }

        public async Task<byte[]> DownloadTicket(int ticketId)
        {
            var fieldTicket = await GetTicket(ticketId);

            if (fieldTicket.CustomerSignedOn.HasValue)
            {
                return _fileManagerService.GetTicketBytes(fieldTicket.FileName);
            }

            var model = new TicketReport(fieldTicket);
            var ticketPreviewHtml = await _viewRenderer.RenderViewToStringAsync(_ticketTemplate, model);

            return _pdfGeneratorService.GeneratePdf(ticketPreviewHtml);
        }

        public async Task UploadTicket(Stream fileStream, int ticketId)
        {
            var ticket = await GetTicket(ticketId);

            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            var fileBytes = memoryStream.ToArray();

            _fileManagerService.SaveTicketFile(fileBytes, ticket.FileName);

            ticket.CustomerSignedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task ResetSignatures(int ticketId)
        {
            var ticket = await GetTicket(ticketId);
            ticket.SignedOn = null;
            ticket.EmployeePrintedName = string.Empty;
            ticket.CustomerSignedBy = string.Empty;
            ticket.CustomerSignedOn = null;
            ticket.CustomerPrintedName = string.Empty;
            ticket.EmployeeSignature = string.Empty;

            _fileManagerService.DeleteTicket(ticket.FileName);

            await _context.SaveChangesAsync();
        }

        private static void VerifyCharges(FieldTicket ticket, ManageTicketDto manageTicketDto)
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

        private static void UpdateChargeQuantity(FieldTicket ticket, string chargeName, double chargeQuantity)
        {
            var charge = ticket.TicketSpecifications
                .SingleOrDefault(x => x.Charge == chargeName);
            if (charge == null)
            {
                return;
            }
            charge.Quantity = chargeQuantity;
        }

        private async Task<FieldTicket> GetTicket(int id, bool includeSpecs = true)
        {
            var ticket = await GetTicketIncludes(includeSpecs)
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

        private static void VerifyCanUpdateTicket(FieldTicket ticket)
        {
            if (ticket.SignedOn.HasValue)
            {
                throw new Exception("Unable to modify ticket after it has been signed");
            }
        }
    }
}
