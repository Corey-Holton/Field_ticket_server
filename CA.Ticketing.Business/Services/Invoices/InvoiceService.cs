using AutoMapper;
using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.FileManager;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Business.Services.Notifications;
using CA.Ticketing.Business.Services.Pdf;
using CA.Ticketing.Business.Services.Pdf.Dto;
using CA.Ticketing.Business.Services.Removal;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Setup;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Options;
using CA.Ticketing.Common.Models;


namespace CA.Ticketing.Business.Services.Invoices
{
    public class InvoiceService : EntityServiceBase, IInvoiceService
    {
        private readonly IPdfGeneratorService _pdfGeneratorService;

        private readonly IRazorViewToStringRenderer _viewRenderer;

        private readonly IRemovalService _removalService;

        private readonly MessagesComposer _messagesComposer;

        private readonly IFileManagerService _fileManagerService;

        private readonly INotificationService _notificationService;

        private readonly string _invoiceTemplate = "/Views/Invoice/InvoiceTemplate.cshtml";

        private readonly InitialData _initialData;

        public InvoiceService(
            CATicketingContext context,
            IMapper mapper, 
            IPdfGeneratorService pdfGeneratorService,
            IRazorViewToStringRenderer viewRenderer,
            IFileManagerService fileManagerService,
            IRemovalService removalService,
            MessagesComposer messagesComposer,
            INotificationService notificationService,
            IOptions<InitialData> initialData) : base(context, mapper)
        {
            _pdfGeneratorService = pdfGeneratorService;
            _viewRenderer = viewRenderer;
            _removalService = removalService;
            _fileManagerService = fileManagerService;
            _notificationService = notificationService;
            _messagesComposer = messagesComposer;
            _initialData = initialData.Value;
        }

        public async Task<ListResult<InvoiceDto>> GetAll(int index, int size, string sorting, string order, string searchString)
        {

            var invoices = _context.Invoices
               .Include(x => x.Customer)
               .Include(x => x.Tickets)
               .Include(x => x.InvoiceLateFees)
               .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                invoices = invoices.Where(invoice => invoice.Customer.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(sorting))
            {
                invoices = invoices.OrderBy(sorting + " " + order);
            }

            var invoicesList = await invoices
                .Skip(index * size)
                .Take(size)
                .AsSplitQuery()
                .ToListAsync();

            var list = invoicesList.Select(x => _mapper.Map<InvoiceDto>(x));
            var result = new ListResult<InvoiceDto>
            {
                TotalCount = await _context.Invoices.CountAsync(),
                ItemsList = list.ToList()
            };
            return result;
        }

        public async Task<IEnumerable<InvoiceDto>> GetByDueDate()
        {
            var invoicesList = await _context.Invoices
                .Include(x => x.Customer)
                .Include(x => x.Tickets)
                .Include(x => x.InvoiceLateFees)
                .Where(invoice => EF.Functions.DateDiffDay(invoice.DueDate, DateTime.Now) <= 31)
                .Where(invoice => invoice.Paid != true)
                .OrderBy(invoice => EF.Functions.DateDiffDay(invoice.DueDate, DateTime.Now))
                .AsSplitQuery()
                .ToListAsync();
            
            var result = invoicesList.Select(x => _mapper.Map<InvoiceDto>(x));
            return result;
        }

        public async Task<InvoiceDto> GetById(string id)
        {
            var invoice = await _context.Invoices
                .Include(x => x.Customer)
                .Include(x => x.Tickets)
                .Include(x => x.InvoiceLateFees)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<InvoiceDto>(invoice)!;
        }

        public async Task<string> Create(CreateInvoiceDto entity)
        {
            var currentInvoiceCount = await _context.Invoices.CountAsync() + _initialData.Invoices.StartId;
            var customer = await _context.Customers
                .SingleAsync(x => x.Id == entity.CustomerId);

            var netTerm = customer.NetTerm > 0 ? customer.NetTerm : 30;

            var invoice = new Invoice()
            {
                InvoiceId = $"A-{currentInvoiceCount + 1}",
                CustomerId = customer.Id,
                InvoiceDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(netTerm)
            };

            var tickets = await _context.FieldTickets
                .Where(x => entity.TicketIds.Contains(x.Id))
                .ToListAsync();

            if (tickets.Any(x => !string.IsNullOrEmpty(x.InvoiceId)))
            {
                throw new Exception("Some tickets are already invoiced");
            }

            if (tickets.Any(x => !x.HasCustomerSignature || string.IsNullOrEmpty(x.FileName)))
            {
                throw new Exception("Some tickets are missing customer signature");
            }

            if (!tickets.All(x => x.CustomerId == invoice.CustomerId))
            {
                throw new Exception("Some tickets do not belong to same customer");
            }

            tickets.ForEach(x => invoice.Tickets.Add(x));

            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync();
            return invoice.Id;
        }

        public async Task MarkAsPaid(string id)
        {
            var invoice = await _context.Invoices
                .SingleAsync(x => x.Id == id);

            invoice.Paid = true;

            await _context.SaveChangesAsync();
        }

        public async Task SendToCustomer(string id)
        {
            var (invoice, pdf) = await GetInvoiceWithPdf(id);

            if (string.IsNullOrEmpty(invoice.Customer.SendInvoiceTo))
            {
                throw new Exception("Customer email is not defined");
            }

            if (!invoice.Tickets.Any(x => x.CustomerSignedOn.HasValue) || invoice.Tickets.Any(x => string.IsNullOrEmpty(x.FileName)))
            {
                throw new Exception("Some tickets are missing customer signature. Please verify.");
            }

            var tickets = invoice.Tickets
                .Select(x => new { x.TicketId, TicketBytes = _fileManagerService.GetFileBytes(FilePaths.Tickets, x.FileName) })
                .ToList();

            var attachments = new List<(Stream, string)>()
            {
                (new MemoryStream(pdf), $"{invoice.InvoiceId}-{invoice.Customer.Name}.pdf")
            };

            foreach (var ticket in tickets)
            {
                attachments.Add((new MemoryStream(ticket.TicketBytes), $"{ticket.TicketId}.pdf"));
            }

            var emailMessage = _messagesComposer.GetEmailComposed(EmailMessageKeys.SendInvoice);
            var emailAddress = invoice.Customer.SendInvoiceTo;

            await _notificationService.SendEmail(emailAddress, emailMessage, attachments);

            invoice.SentToCustomer = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var invoice = await _context.Invoices
                .Include(x => x.InvoiceLateFees)
                .Include(x => x.Tickets)
                .AsSplitQuery()
                .SingleAsync(x => x.Id == id);

            _removalService.Remove(invoice);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveLateFee(string id)
        {
            var invoiceLateFee = await _context.InvoiceLateFees
                .SingleAsync(x => x.Id == id);
            _context.Entry(invoiceLateFee).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task<(string InvoiceId, byte[] InvoiceBytes)> Download(string id)
        {
            var (invoice, pdf) = await GetInvoiceWithPdf(id);
            return (invoice.InvoiceId, pdf);
        }

        public async Task<(Invoice invoice, byte[] pdf)> GetInvoiceWithPdf(string id)
        {
            var invoice = await _context.Invoices
                            .Include(x => x.Customer)
                            .Include(x => x.Tickets)
                                .ThenInclude(x => x.Location)
                            .Include(x => x.InvoiceLateFees)
                            .AsSplitQuery()
                            .SingleAsync(x => x.Id == id);

            var invoiceReport = new InvoiceReport(invoice);

            var invoiceHtml = await _viewRenderer.RenderViewToStringAsync(_invoiceTemplate, invoiceReport);

            var pdf = _pdfGeneratorService.GeneratePdf(invoiceHtml, true);

            return (invoice, pdf);
        }
    }
}
