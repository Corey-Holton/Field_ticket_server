﻿using AutoMapper;
using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.FileManager;
using CA.Ticketing.Business.Services.Notifications;
using CA.Ticketing.Business.Services.Pdf;
using CA.Ticketing.Business.Services.Pdf.Dto;
using CA.Ticketing.Business.Services.Removal;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Common.Setup;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Options;
using System.Data;
using System.Drawing;

using System.Linq.Expressions;
using CA.Ticketing.Common.Models;
using System.Linq.Dynamic.Core;
using static CA.Ticketing.Common.Constants.ApiRoutes;
using CA.Ticketing.Business.Services.EmployeeNotes.Dto;
using System.Net.Sockets;

namespace CA.Ticketing.Business.Services.Tickets
{
    public class TicketService : EntityServiceBase, ITicketService
    {
        private readonly IUserContext _userContext;

        private readonly IRazorViewToStringRenderer _viewRenderer;

        private readonly IPdfGeneratorService _pdfGeneratorService;

        private readonly IFileManagerService _fileManagerService;

        private readonly IRemovalService _removalService;

        private readonly MessagesComposer _messagesComposer;

        private readonly INotificationService _notificationService;

        private readonly string _ticketTemplate = "/Views/Tickets/TicketTemplate.cshtml";

        private readonly InitialData _initialData;

        public TicketService(
            CATicketingContext context, 
            IMapper mapper, 
            IUserContext userContext,
            IRazorViewToStringRenderer viewRenderer,
            IPdfGeneratorService pdfGeneratorService,
            IFileManagerService fileManagerService,
            IRemovalService removalService,
            MessagesComposer messagesComposer,
            INotificationService notificationService,
            IOptions<InitialData> initialData) : base(context, mapper)
        {
            _userContext = userContext;
            _viewRenderer = viewRenderer;
            _pdfGeneratorService = pdfGeneratorService;
            _fileManagerService = fileManagerService;
            _removalService = removalService;
            _notificationService = notificationService;
            _messagesComposer = messagesComposer;
            _initialData = initialData.Value;
        }

        public async Task<ListResult<TicketDto>> GetAll(int index, int size, string sorting, string order, string searchString)
        {
            Expression<Func<FieldTicket, bool>> ticketsFilter = x => true;

            if (_userContext.User!.Role == ApplicationRole.ToolPusher)
            {
                ticketsFilter = x => x.CreatedBy == _userContext.User.Id || x.SignedOn.HasValue;
            }

            if (_userContext.User!.Role == ApplicationRole.Customer)
            {
                var customerId = (await _context.CustomerContacts
                    .FirstAsync(x => x.Id == _userContext.User.CustomerContactId)).CustomerId;

                ticketsFilter = x => x.CustomerId == customerId;
            }

            var totalCount = await _context.FieldTickets.Where(ticketsFilter).CountAsync();


            if (sorting == "isInvoiced")
            {
                sorting = "InvoiceId";
            }

            if (sorting == "hasCustomerSignature")
            {
                sorting = "CustomerSignedOn";
            }

            if (sorting == "customerName")
            {
                sorting = "Customer.Name";
            }

            if (sorting == "locationName")
            {
                sorting = "Location.Lease";
            }

            var tickets = GetTicketIncludes()
                .Where(ticketsFilter);

            if (!String.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(ticket => ticket.Customer!.Name.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(sorting))
            {
                tickets = tickets.OrderBy(sorting + " " + order);
            }

            var ticketList = await tickets
                .Skip(index * size)
                .Take(size)
                .AsSplitQuery()
                .ToListAsync();

            var lista = ticketList.Select(x => _mapper.Map<TicketDto>(x));
            var result = new ListResult<TicketDto>
            {
                TotalCount = totalCount,
                ItemsList = lista.ToList()
            };
            return result;
        }

        public async Task<IEnumerable<TicketDto>> GetByDates(DateTime startDate, DateTime endDate)
        {
            var tickets = await GetTicketIncludes()
               .Where(x => x.ExecutionDate >= startDate && x.ExecutionDate <= endDate)
               .ToListAsync();
            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<TicketDto> GetById(string id)
        {
            var ticket = await GetTicket(id);
            ticket.Equipment = _context.Equipment.First(x => x.Id == ticket.EquipmentId);
            ticket.TicketType = _context.TicketType.FirstOrDefault(x => x.Id == ticket.TicketTypeId);
            ticket.TicketSpecifications = ticket.TicketSpecifications.OrderBy(x => x.CreatedDate).ToList();
            var chargesCount = ticket.TicketSpecifications.Count;
            
            var isAdmin = _userContext.User!.Role == ApplicationRole.Admin;
            if (ticket.TicketType != null && ticket.TicketType?.Name != "Base")
            {
                var ticketTypeDetailsDto = _mapper.Map<TicketTypeDetailsDto>(ticket);
                ticketTypeDetailsDto.TicketSpecificationsData = ticket.TicketSpecifications.Where(x => x.SpecialCharge || x.Quantity > 0).ToList().Select(x => _mapper.Map<TicketSpecificationDto>((isAdmin, x)));
                ticketTypeDetailsDto.AllTicketSpecifications = ticket.TicketSpecifications.ToList().Select(x => _mapper.Map<TicketSpecificationDto>((isAdmin, x)));
                ticketTypeDetailsDto.PayrollData.ToList().ForEach(x =>
                {
                    x.EmployeeNote = _mapper.Map<EmployeeNoteDto>(ticket.EmployeeNotes.SingleOrDefault(n => n.EmployeeId == x.EmployeeId));
                });
                return ticketTypeDetailsDto;
            }
            var ticketDetailsDto = _mapper.Map<TicketDetailsDto>(ticket);
            var half = (int)Math.Ceiling((decimal)chargesCount / 2);
            var leftSide = ticket.TicketSpecifications.Take(half).ToList();
            var rightSide = ticket.TicketSpecifications.Skip(half).ToList();
            ticketDetailsDto.TicketSpecificationsLeft = leftSide.Select(x => _mapper.Map<TicketSpecificationDto>((isAdmin, x)));
            ticketDetailsDto.TicketSpecificationsRight = rightSide.Select(x => _mapper.Map<TicketSpecificationDto>((isAdmin, x)));
            ticketDetailsDto.PayrollData.ToList().ForEach(x =>
            {
                x.EmployeeNote = _mapper.Map<EmployeeNoteDto>(ticket.EmployeeNotes.SingleOrDefault(n => n.EmployeeId == x.EmployeeId));
            });
            return ticketDetailsDto;
        }

        public async Task<string> Create(ManageTicketDto manageTicketDto)
        {
            var ticketIdentifier = _userContext.User!.TicketIdentifier;

            if (string.IsNullOrEmpty(ticketIdentifier))
            {
                ticketIdentifier = "CA";
            }

            var initialDataStartId = _initialData.Tickets
                .SingleOrDefault(x => x.Identifier.ToLower() == ticketIdentifier.ToLower())?.StartId ?? 0;

            var lastTicketId = (await _context.FieldTickets
                .Where(x => x.TicketId.ToLower().StartsWith(ticketIdentifier.ToLower()))
                .Select(x => x.TicketId)
                .ToListAsync())
                .Select(x => 
                {
                    var ticketIdSplit = x.Split("-");
                    if (ticketIdSplit.Length == 2 && int.TryParse(ticketIdSplit[1], out var ticketId))
                    {
                        return ticketId;
                    }

                    return 0;
                })
                .Where(x => x >= initialDataStartId)
                .DefaultIfEmpty(initialDataStartId)
                .Max();

            ValidateTicketServiceTypes(manageTicketDto);

            var ticket = _mapper.Map<FieldTicket>(manageTicketDto);
            ticket.TicketId = $"{ticketIdentifier}-{lastTicketId + 1:D4}";
            ticket.CreatedBy = _userContext.User!.Id;

            var equipment = _context.Equipment
                .Include(x => x.TicketType).ThenInclude(x => x.IncludedCharges)
                .Include(x => x.TicketType).ThenInclude(x => x.SpecialCharges)
                .First(x => x.Id == ticket.EquipmentId);

            ticket.Equipment = equipment;

            ticket.TicketType = equipment.TicketType;

            if (!string.IsNullOrEmpty(ticket.CustomerId))
            {
                var customer = await _context.Customers.SingleAsync(x => x.Id == ticket.CustomerId);
                ticket.SendEmailTo = customer.SendInvoiceTo;
            }

            await GenerateCharges(ticket);

                GenerateRecords(ticket);

            

            if (ticket.IsTaxable())
            {
                var settings = await _context.Settings.FirstAsync();
                ticket.TaxRate = settings.TaxRate;
            }
            var terminationCuttoffTime = ticket.ExecutionDate.GetEndofDay();
            var employees = await _context.Employees
                .Where(x => x.AssignedRigId == manageTicketDto.EquipmentId && (!x.TerminationDate.HasValue || x.TerminationDate > terminationCuttoffTime))
                .ToListAsync();

            foreach (var employee in employees)
            {
                ticket.PayrollData.Add(new PayrollData { EmployeeId = employee.Id, Name = employee.DisplayName });
            }
          
            _context.Entry(ticket.Equipment).State = EntityState.Detached;

            UpdateTicketData(ticket);

            ticket.Equipment = null;

            _context.FieldTickets.Add(ticket);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var e = ex;
            }
            return ticket.Id;
        }

        public async Task Update(ManageTicketDto manageTicketDto)
        {
            var ticket = await GetTicket(manageTicketDto.Id);

            VerifyCanUpdateTicket(ticket);

            ValidateTicketServiceTypes(manageTicketDto);

            await VerifyTicketServiceType(ticket, manageTicketDto);

            _mapper.Map(manageTicketDto, ticket);

            ticket.ServiceTypes = manageTicketDto.ServiceTypes;

            await GenerateCharges(ticket);

            UpdateTicketData(ticket);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateHours(ManageTicketHoursDto manageTicketHours)
        {
            var ticket = await GetTicket(manageTicketHours.TicketId);

            VerifyCanUpdateTicket(ticket);

            _mapper.Map(manageTicketHours, ticket);

            UpdateTicketData(ticket);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateOtherDetails(ManageWellOtherDetailsDto manageWellOther)
        {
            var ticket = await GetTicket(manageWellOther.TicketId);

            VerifyCanUpdateTicket(ticket);

            _mapper.Map(manageWellOther, ticket);

             await _context.SaveChangesAsync();

        }

        public async Task Delete(string id)
        {
            var ticket = await _context.FieldTickets
                .Include(x => x.TicketSpecifications)
                .Include(x => x.PayrollData)
                .Include(x => x.EmployeeNotes)
                .Include(x => x.WellRecord)
                .AsSplitQuery()
                .SingleAsync(x => x.Id == id);

            VerifyCanUpdateTicket(ticket);

            _removalService.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PayrollDataDto>> GetPayrollData(string ticketId)
        {
            var ticket = await _context.FieldTickets.Include(x => x.PayrollData).ThenInclude(p => p.Employee).SingleOrDefaultAsync(x => x.Id == ticketId);
            
            if (ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            var notes = await _context.EmployeeNotes.ToListAsync();

            ticket.PayrollData.ToList().ForEach(x =>
            {
                x.EmployeeNote = ticket.EmployeeNotes.SingleOrDefault(n => n.EmployeeId == x.EmployeeId);
            });

            var payrollDataDto = _mapper.Map<List<PayrollDataDto>>(ticket.PayrollData);

            return payrollDataDto.OrderBy(x => x.JobTitle).ToList();
        }

        public async Task<List<TicketSpecificationDto>> GetSpecialTicketSpec(string ticketId)
        {
            var ticket = await GetTicket(ticketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            var isAdmin = _userContext.User!.Role == ApplicationRole.Admin;

            var specialSpecDto = ticket.TicketSpecifications.Where(x => x.SpecialCharge || x.Quantity > 0).ToList().Select(x => _mapper.Map<TicketSpecificationDto>((isAdmin, x)));

            return specialSpecDto.ToList();
        }

        public async Task AddPayroll(PayrollDataDto payrollDataDto)
        {
            var ticket = await _context.FieldTickets
                .Include(x => x.PayrollData)
                .Include(x => x.TicketSpecifications)
                .SingleOrDefaultAsync(x => x.Id == payrollDataDto.FieldTicketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            VerifyCanUpdateTicket(ticket);

            var alreadyExists = !string.IsNullOrEmpty(payrollDataDto.EmployeeId) ? 
                ticket.PayrollData
                    .Any(x => x.EmployeeId == payrollDataDto.EmployeeId) :
                ticket.PayrollData
                    .Any(x => string.Equals(x.Name, payrollDataDto.Name, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
            {
                throw new Exception("There is already an employee with the same id or name added");
            }

            var payrollData = _mapper.Map<PayrollData>(payrollDataDto);
            ticket.PayrollData.Add(payrollData);

            var (totalTime, travelTime) = GetTicketTimes(ticket);
            UpdateEmployeePayroll(payrollData, travelTime, totalTime, ticket);

            UpdateLaborQuantity(ticket);

            UpdateTicketTotal(ticket);

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePayrollData(PayrollDataDto payrollDataDto)
        {
            var ticket = await _context.FieldTickets
                .Include(x => x.PayrollData)
                .Include(x => x.TicketSpecifications)
                .SingleOrDefaultAsync(x => x.Id == payrollDataDto.FieldTicketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }
            
            var payrollData = ticket.PayrollData
                .SingleOrDefault(x => x.Id == payrollDataDto.Id);

            if (payrollData == null)
            {
                throw new KeyNotFoundException(nameof(PayrollData));
            }

            VerifyCanUpdateTicket(payrollData.FieldTicket);

            payrollData.RoustaboutHours = payrollDataDto.RoustaboutHours;
            payrollData.YardHours = payrollDataDto.YardHours;
            payrollData.RigHours = payrollDataDto.RigHours;
            payrollData.TravelHours = payrollDataDto.TravelHours;

            UpdateLaborQuantity(ticket);

            UpdateTicketTotal(ticket);

            await _context.SaveChangesAsync();
        }

        public async Task RemovePayroll(string payrollDataId)
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

            var ticket = await _context.FieldTickets
                .Include(x => x.PayrollData)
                .Include(x => x.TicketSpecifications)
                .Include(x => x.EmployeeNotes)
                .SingleAsync(x => x.Id == payrollData.FieldTicketId);

            UpdateLaborQuantity(ticket);

            UpdateTicketTotal(ticket);

            var note = ticket.EmployeeNotes.SingleOrDefault(n => n.EmployeeId == payrollData.EmployeeId);

            if (note != null)
            {
                _removalService.Remove(note);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<UpdateTicketSpecResponse> UpdateTicketSpecification(TicketSpecificationDto ticketSpecificationDto)
        {
            var ticketSpecification = await _context.TicketSpecifications
                .SingleOrDefaultAsync(x => x.Id == ticketSpecificationDto.Id);

            if (ticketSpecification == null)
            {
                throw new KeyNotFoundException(nameof(TicketSpecification));
            }

            if (!ticketSpecification.AllowUoMChange && ticketSpecification.UoM != ticketSpecificationDto.UoM)
            {
                throw new Exception("You are not allowed to modify Unit of Measure on this charge");
            }

            if (!ticketSpecification.AllowRateAdjustment && ticketSpecification.Rate != ticketSpecificationDto.Rate)
            {
                throw new Exception("You are not allowed to modify rate on this charge");
            }

            var isAdmin = _userContext.User!.Role == ApplicationRole.Admin;
            if (!isAdmin && ChargesInfo.ReadonlyCharges.Contains(ticketSpecification.Charge))
            {
                throw new Exception("This is a readonly charge");
            }

            var ticket = await _context.FieldTickets
                .Include(x => x.PayrollData)
                .Include(x => x.TicketSpecifications)
                .SingleAsync(x => x.Id == ticketSpecification.FieldTicketId);

            VerifyCanUpdateTicket(ticket);

            var ticketSpec = ticket.TicketSpecifications
                .Single(x => x.Id == ticketSpecificationDto.Id);

            _mapper.Map(ticketSpecificationDto, ticketSpec);

            if (ticketSpec.Charge == ChargeNames.TravelTime || ticketSpec.Charge == ChargeNames.Rig)
            {
                UpdateEmployeePayrolls(ticket);
            }

            UpdateLaborQuantity(ticket);

            UpdateTicketTotal(ticket);

            await _context.SaveChangesAsync();

            return new UpdateTicketSpecResponse
            {
                TicketTotal = ticket.Total,
                TicketSpecification = _mapper.Map<TicketSpecificationDto>((isAdmin, ticketSpec))
            };
        }

        public async Task EmployeeSignature(SignatureBaseDto signature)
        {
            var fieldTicket = await GetTicket(signature.TicketId);

            if (fieldTicket.SignedOn.HasValue)
            {
                throw new Exception("This document is already signed.");
            }

            if (!fieldTicket.StartTime.HasValue || !fieldTicket.EndTime.HasValue)
            {
                throw new Exception("Can't sign ticket without adding working hours");
            }

            if (!string.IsNullOrEmpty(fieldTicket.SignedBy))
            {
                fieldTicket.SignedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return;
            }

            var user = await _context.Users.SingleAsync(x => x.Id == _userContext.User!.Id);

            if (!string.IsNullOrEmpty(user.Signature) && string.IsNullOrEmpty(signature.Signature))
            {
                throw new Exception("User has no signature defined");
            }

            if (string.IsNullOrEmpty(user.Signature))
            {
                user.Signature = signature.Signature;
            }

            fieldTicket.SignedBy = _userContext.User!.Id;
            fieldTicket.SignedOn = DateTime.UtcNow;
            fieldTicket.EmployeeSignature = user.Signature;
            fieldTicket.EmployeePrintedName = user.DisplayName;

            await _context.SaveChangesAsync();
        }

        public async Task<string> PreviewTicket(string ticketId)
        {
            var fieldTicket = await GetTicket(ticketId);

            if (fieldTicket.HasCustomerSignature)
            {
                throw new Exception("This document is already signed");
            }

            if (string.IsNullOrEmpty(fieldTicket.SignedBy))
            {
                var user = await _context.Users.SingleAsync(x => x.Id == _userContext.User!.Id);
                fieldTicket.SignedBy = _userContext.User!.Id;
                fieldTicket.EmployeePrintedName = user.DisplayName;
            }

            var employeeNumber = await GetEmployeePhoneNumber(fieldTicket.SignedBy);

            var model = new TicketReport(fieldTicket, employeeNumber)
            {
                ClassName = "preview-wrapper"
            };

            var ticketPreviewHtml = await _viewRenderer.RenderViewToStringAsync(_ticketTemplate, model);
            return ticketPreviewHtml;
        }

        public async Task CustomerSignature(CustomerSignatureDto customerSignatureDto)
        {
            var fieldTicket = await GetTicket(customerSignatureDto.TicketId);

            if (!fieldTicket.SignedOn.HasValue)
            {
                throw new Exception("Ticket is not signed by the employee");
            }

            fieldTicket.CustomerSignedOn = DateTime.UtcNow;
            fieldTicket.CustomerPrintedName = customerSignatureDto.PrintedName;

            if (_userContext.User!.Role == ApplicationRole.Customer)
            {
                fieldTicket.CustomerSignedBy = _userContext.User!.Id;
            }

            var employeeNumber = await GetEmployeePhoneNumber(fieldTicket.SignedBy);
            var model = new TicketReport(fieldTicket, employeeNumber, customerSignatureDto.Signature);
            var ticketPreviewHtml = await _viewRenderer.RenderViewToStringAsync(_ticketTemplate, model);

            var pdfGenerated = _pdfGeneratorService.GeneratePdf(ticketPreviewHtml);

            fieldTicket.FileName = $"{fieldTicket.TicketId}-{fieldTicket.Id}.pdf";

            _fileManagerService.SaveFile(pdfGenerated, FilePaths.Tickets, fieldTicket.FileName);

            await _context.SaveChangesAsync();
        }

        public async Task<byte[]> DownloadTicket(string ticketId)
        {
            var fieldTicket = await GetTicket(ticketId);

            if (fieldTicket.HasCustomerSignature)
            {
                return _fileManagerService.GetFileBytes(FilePaths.Tickets, fieldTicket.FileName);
            }

            return await GetTicketPdf(fieldTicket);
        }

        public async Task UploadTicket(Stream fileStream, string ticketId)
        {
            var ticket = await GetTicket(ticketId);
            if (ticket.HasCustomerSignature)
            {
                throw new Exception("Ticket has customer signature");
            }

            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            var fileBytes = memoryStream.ToArray();

            ticket.FileName = $"{ticket.TicketId}-{ticket.Id}.pdf";
            _fileManagerService.SaveFile(fileBytes, FilePaths.Tickets, ticket.FileName);

            ticket.CustomerSignedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task ResetSignatures(string ticketId)
        {
            var ticket = await GetTicket(ticketId);
            ticket.SignedOn = null;
            ticket.CustomerSignedBy = string.Empty;
            ticket.CustomerSignedOn = null;
            ticket.CustomerPrintedName = string.Empty;

            _fileManagerService.DeleteFile(FilePaths.Tickets, ticket.FileName);
            ticket.FileName = string.Empty;

            await _context.SaveChangesAsync();
        }

        public async Task SendToClient(string ticketId, string redirectUrl)
        {
            var fieldTicket = await GetTicket(ticketId);

            if (string.IsNullOrEmpty(fieldTicket.SendEmailTo) && string.IsNullOrEmpty(fieldTicket.Customer!.SendInvoiceTo))
            {
                throw new Exception("There is no email defined for this customer");
            }

            if (fieldTicket.HasCustomerSignature)
            {
                throw new Exception("Can't send a ticket that has already been signed");
            }

            var ticketPdf = await GetTicketPdf(fieldTicket);

            var ticketStream = new MemoryStream(ticketPdf);

            var callBackUrl = $"{redirectUrl}/dashboard/tickets/edit/{ticketId}";

            var emailMessage = _messagesComposer.GetEmailComposed(EmailMessageKeys.SendTicket, (4, callBackUrl));

            var attachments = new List<(Stream, string)>()
            {
                (ticketStream, $"{fieldTicket.TicketId}-{fieldTicket.Customer!.Name}.pdf")
            };

            var emailAddress = !string.IsNullOrEmpty(fieldTicket.SendEmailTo) ? fieldTicket.SendEmailTo : fieldTicket.Customer!.SendInvoiceTo;

            await _notificationService.SendEmail(emailAddress, emailMessage, attachments);

            fieldTicket.SentToCustomerOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private async Task VerifyTicketServiceType(FieldTicket ticket, ManageTicketDto manageTicketDto)
        {
            if (ticket.ServiceTypeOrdered == manageTicketDto.ServiceTypesOrdered)
            {
                return;
            }

            if ((ticket.IsRigWork() && manageTicketDto.IsNotRigWork())
                || (ticket.IsNotRigWork() && manageTicketDto.IsRigWork()))
            {
                foreach (var payrollData in ticket.PayrollData)
                {
                    payrollData.RoustaboutHours = 0;
                    payrollData.YardHours = 0;
                    payrollData.RigHours = 0;
                    payrollData.TravelHours = 0;
                }
            }

            if (manageTicketDto.ServiceTypes.Contains(ServiceType.PAndA))
            {
                ticket.TaxRate = 0;
                return;
            }

            if (manageTicketDto.ServiceTypes.First() == ServiceType.Yard)
            {
                ticket.TicketSpecifications.Clear();
            }

            if (ticket.IsTaxable())
            {
                var settings = await _context.Settings.FirstAsync();
                ticket.TaxRate = settings.TaxRate;
            }
        }

        private void ValidateTicketServiceTypes(ManageTicketDto manageTicket)
        {
            if (!manageTicket.ServiceTypes.Any() && manageTicket.ServiceType.HasValue)
            {
                manageTicket.ServiceTypes = new ServiceType[] { manageTicket.ServiceType.Value };
                return;
            }

            if (!manageTicket.ServiceTypes.Any())
            {
                throw new Exception("Ticket must have any service type selected");
            }

            if (manageTicket.ServiceTypes.Length == 1)
            {
                return;
            }

            var onlySingleAllowed = new ServiceType[] { ServiceType.Yard, ServiceType.Roustabout, ServiceType.StandBy };

            if (manageTicket.ServiceTypes.Any(x => onlySingleAllowed.Contains(x)))
            {
                throw new Exception("Service types Yard, Roustabout and Standby are not allowed with multiple selections");
            }

            if (manageTicket.ServiceTypes.Contains(ServiceType.PAndA))
            {
                if (manageTicket.ServiceTypes.Any(x => x != ServiceType.PAndA && x != ServiceType.Workover))
                {
                    throw new Exception("P&A is allowed only with Service type Workover");
                }
            }
        }

        private void GenerateRecords(FieldTicket ticket)
        {
            if (ticket.TicketType == null || ticket.TicketType.IncludedCharges == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            if (ticket.TicketType?.Name == "Well")
            {
                if (ticket.WellRecord.Any() || ticket.SwabCups.Any())
                {
                    return;
                }
                
                foreach (var type in (WellRecordType[])Enum.GetValues(typeof(WellRecordType)))
                {
                    for (int i = 0; i < type.GetWellRecordAmount(); i++)
                    {
                        var wellData = new WellRecord { FieldTicketId = ticket.Id, FieldTicket = ticket, WellRecordType = type };

                        ticket.WellRecord.Add(wellData);
                    }
                }

                for (int i = 0; i < 6; i++)
                {
                    var swabData = new SwabCups { FieldTicket = ticket, FieldTicketId = ticket.Id };

                    ticket.SwabCups.Add(swabData);
                }
            }
            return;
        }

        private async Task GenerateCharges(FieldTicket ticket)
        {
            if (ticket.IsServiceType(ServiceType.Yard) || ticket.TicketSpecifications.Any())
            {
                return;
            }

            var chargesContext =  _context.Charges;

            if (ticket.TicketType == null || ticket.TicketType.IncludedCharges == null) {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            var allCharges = await chargesContext
                .Where(x => ticket.TicketType!.IncludedCharges.Contains(x))
                .OrderBy(x => x.Order)
                .ToListAsync();

            var specialCharges = await chargesContext
                .Where(x => ticket.TicketType!.SpecialCharges.Contains(x))
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
                ticketSpec.SpecialCharge = specialCharges.Contains(charge);
                ticket.TicketSpecifications.Add(ticketSpec);
            }

            UpdateChargeQuantity(ticket, ChargeNames.ToolPusher, 1);
            UpdateChargeQuantity(ticket, ChargeNames.TravelTime, 1);
        }

        private void UpdateTicketData(FieldTicket ticket)
        {
            CalculateCharges(ticket);
            UpdateEmployeePayrolls(ticket);
            UpdateLaborQuantity(ticket);
            UpdateTicketTotal(ticket);
        }

        private void CalculateCharges(FieldTicket ticket)
        {
            if (!ticket.TicketSpecifications.Any())
            {
                return;
            }

            var totalTime = ticket.StartTime.HasValue && ticket.EndTime.HasValue ? (ticket.EndTime.Value - ticket.StartTime.Value).TotalHours : 0;

            if (ticket.IsServiceType(ServiceType.StandBy))
            {
                UpdateChargeQuantity(ticket, ChargeNames.Fuel, 0);
                UpdateChargeQuantity(ticket, ChargeNames.Rig, totalTime);
                return;
            }

            if (ticket.IsServiceType(ServiceType.Roustabout))
            {
                UpdateChargeQuantity(ticket, ChargeNames.Rig, 0);
                UpdateChargeQuantity(ticket, ChargeNames.Fuel, 0);
                return;
            }

            UpdateChargeQuantity(ticket, ChargeNames.Rig, totalTime);
            UpdateChargeQuantity(ticket, ChargeNames.Fuel, totalTime * ticket.Equipment!.FuelConsumption);
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

        private static void UpdateTicketTotal(FieldTicket ticket) => ticket.Total = ticket.TicketSpecifications.Sum(x => x.Quantity * x.Rate);

        private static void UpdateEmployeePayrolls(FieldTicket ticket)
        {
            var (totalTime, travelTime) = GetTicketTimes(ticket);
            ticket.PayrollData
                .ToList()
                .ForEach(x => UpdateEmployeePayroll(x, travelTime, totalTime, ticket));
        }

        private static (double TotalTime, double TravelTime) GetTicketTimes(FieldTicket ticket)
        {
            var travelTime = ticket.TicketSpecifications.SingleOrDefault(x => x.Charge == ChargeNames.TravelTime)?.Quantity ?? 0;

            var totalTime = (ticket.StartTime.HasValue && ticket.EndTime.HasValue ? (ticket.EndTime.Value - ticket.StartTime.Value).TotalHours : 0);

            if (totalTime < 0)
            {
                totalTime = 0;
            }
            return (totalTime, travelTime);
        }

        private static void UpdateEmployeePayroll(PayrollData payrollData, double travelTime, double totalTime, FieldTicket ticket)
        {
            payrollData.TravelHours = travelTime;

            if (ticket.IsServiceType(ServiceType.Roustabout))
            {
                payrollData.RoustaboutHours = totalTime;
                return;
            }

            if (ticket.IsServiceType(ServiceType.Yard))
            {
                payrollData.YardHours = totalTime;
                return;
            }

            payrollData.RigHours = totalTime;
        }

        private static void UpdateLaborQuantity(FieldTicket ticket)
        {
            if (!ticket.IsServiceType(ServiceType.Roustabout))
            {
                return;
            }

            var laborCharge = ticket.TicketSpecifications
                .Single(x => x.Charge == ChargeNames.Labor);
            laborCharge.Quantity = ticket.PayrollData.Sum(x => x.RoustaboutHours);
        }

        private async Task<FieldTicket> GetTicket(string? id, bool includeSpecs = true)
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
                .Include(x => x.EmployeeNotes)
                .Include(x => x.Invoice)
                .Include(x => x.Location);

            if (!includeSpecs)
            {
                return baseIncludes;
            }

            return baseIncludes
                .Include(x => x.TicketSpecifications)
                .Include(x => x.WellRecord)
                .Include(x => x.SwabCups)
                .Include(x => x.TicketType)
                .Include(x => x.PayrollData)
                    .ThenInclude(p => p.Employee);
        }

        private async Task<string> GetEmployeePhoneNumber(string? signedBy = null)
        {
            var defaultNumber = "(989) 640-0967";
            var userId = signedBy ?? _userContext.User!.Id;
            var user = await _context.Users.SingleAsync(x => x.Id == userId);
            if (string.IsNullOrEmpty(user.EmployeeId))
            {
                return defaultNumber;
            }

            var employee = _context.Employees.SingleOrDefault(x => x.Id == user.EmployeeId);
            return employee?.Phone ?? defaultNumber;
        }

        private static void VerifyCanUpdateTicket(FieldTicket ticket)
        {
            if (ticket.SignedOn.HasValue)
            {
                throw new Exception("Unable to modify ticket after it has been signed");
            }
        }

        private async Task<byte[]> GetTicketPdf(FieldTicket fieldTicket)
        {
            var employeeNumber = await GetEmployeePhoneNumber(fieldTicket.SignedBy);
            var model = new TicketReport(fieldTicket, employeeNumber);
            var ticketPreviewHtml = await _viewRenderer.RenderViewToStringAsync(_ticketTemplate, model);

            return _pdfGeneratorService.GeneratePdf(ticketPreviewHtml);
        }

        public async Task UpdateWellRecord(WellRecordDto wellRecordDto)
        {
            var wellRecord = await _context.WellRecord
                .SingleOrDefaultAsync(x => x.Id == wellRecordDto.Id);

            if (wellRecord == null)
            {
                throw new KeyNotFoundException(nameof(WellRecord));
            }

            var ticket = await _context.FieldTickets
                .Include(x => x.WellRecord)
                .SingleAsync(x => x.Id == wellRecordDto.FieldTicketId);

            VerifyCanUpdateTicket(ticket);

            var ticketWell = ticket.WellRecord
                .Single(x => x.Id == wellRecordDto.Id);

            _mapper.Map(wellRecordDto, ticketWell);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveWellRecord(WellRecordDto wellRecordDto)
        {
            var wellRecord = _context.WellRecord
                .Include(x => x.FieldTicket)
                .SingleOrDefault(x => x.Id == wellRecordDto.Id);

            if (wellRecord == null)
            {
                throw new KeyNotFoundException(nameof(WellRecord));
            }

            VerifyCanUpdateTicket(wellRecord.FieldTicket);

            _context.Entry(wellRecord).State = EntityState.Deleted;

            await _context.SaveChangesAsync();

            var ticket = await _context.FieldTickets
                .Include(x => x.WellRecord)
                .SingleAsync(x => x.Id == wellRecord.FieldTicketId);

        }

        public async Task AddWellRecord(WellRecordDto wellRecordDto)
        {
            var ticket = await _context.FieldTickets
                .Include(x => x.WellRecord)
                .SingleOrDefaultAsync(x => x.Id == wellRecordDto.FieldTicketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }
            var recordType = wellRecordDto.WellRecordType;

            VerifyCanUpdateTicket(ticket);

            var alreadyExists = ticket.WellRecord.Where(x => x.WellRecordType == recordType).ToList().Count;

            if (recordType.GetWellRecordAmount() == alreadyExists) 
            {
                throw new Exception("Maximum number of records reached");
            }

            var wellData = _mapper.Map<WellRecord>(wellRecordDto);

            ticket.WellRecord.Add(wellData);

            await _context.SaveChangesAsync();
        }

        public async Task AddSwabCharge(SwabCupsDto swabCupsDto)
        {
            var ticket = await _context.FieldTickets
                .Include(x => x.SwabCups)
                .SingleOrDefaultAsync(x => x.Id == swabCupsDto.FieldTicketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            VerifyCanUpdateTicket(ticket);

            var swabCount = ticket.SwabCups.ToList().Count;

            if (swabCount == 6)
            {
                throw new Exception("Maximum number of swab charges reached");
            }

            var swabCharge = _mapper.Map<SwabCups>(swabCupsDto);

            ticket.SwabCups.Add(swabCharge);

            await _context.SaveChangesAsync();

        }

        public async Task UpdateSwabCharge(SwabCupsDto swabCupsDto)
        {
            var swabCharge = await _context.SwabCups
                .SingleOrDefaultAsync(x => x.Id == swabCupsDto.Id);

            if (swabCharge == null)
            {
                throw new KeyNotFoundException(nameof(SwabCups));
            }

            var ticket = await _context.FieldTickets
                .Include(x => x.SwabCups)
                .SingleAsync(x => x.Id == swabCupsDto.FieldTicketId);

            VerifyCanUpdateTicket(ticket);

            var ticketSwab = ticket.SwabCups
                .Single(x => x.Id == swabCupsDto.Id);

            _mapper.Map(swabCupsDto, ticketSwab);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveSwabCharge(SwabCupsDto swabCupsDto)
        {
            var swabCharge = _context.SwabCups
                .Include(x => x.FieldTicket)
                .SingleOrDefault(x => x.Id == swabCupsDto.Id);

            if (swabCharge == null)
            {
                throw new KeyNotFoundException(nameof(SwabCups));
            }

            VerifyCanUpdateTicket(swabCharge.FieldTicket);

            _context.Entry(swabCharge).State = EntityState.Deleted;

            await _context.SaveChangesAsync();
        }
    }
}
