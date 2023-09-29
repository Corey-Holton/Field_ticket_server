using AutoMapper;
using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Business.Services.Pdf;
using CA.Ticketing.Business.Services.Pdf.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Invoices
{
    public class InvoiceService : EntityServiceBase, IInvoiceService
    {
        private readonly IPdfGeneratorService _pdfGeneratorService;

        private readonly IRazorViewToStringRenderer _viewRenderer;

        private readonly string _ticketTemplate = "/Views/Invoice/InvoiceTemplate.cshtml";

        public InvoiceService(
            CATicketingContext context,
            IMapper mapper, 
            IPdfGeneratorService pdfGeneratorService,
            IRazorViewToStringRenderer viewRenderer) : base(context, mapper)
        {
            _pdfGeneratorService = pdfGeneratorService;
            _viewRenderer = viewRenderer;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAll()
        {
            var invoices = await _context.Invoices
                .Include(x => x.Customer)
                .Include(x => x.Tickets)
                    .ThenInclude(x => x.TicketSpecifications)
                .Include(x => x.InvoiceLateFees)
                .AsSplitQuery()
                .ToListAsync();
            return invoices.Select(x => _mapper.Map<InvoiceDto>(x));
        }

        public async Task<string> Create(CreateInvoiceDto entity)
        {
            var currentInvoiceCount = await _context.Invoices.CountAsync();
            var customer = await _context.Customers
                .SingleAsync(x => x.Id == entity.CustomerId);

            var netTerm = customer.NetTerm > 0 ? customer.NetTerm : 30;

            var invoice = new Invoice()
            {
                InvoiceId = $"A-{currentInvoiceCount + 1}",
                CustomerId = customer.Id,
                InvoiceDate = DateTime.UtcNow.AddDays(-95),
                DueDate = DateTime.UtcNow.AddDays(-65)
            };

            var tickets = await _context.FieldTickets
                .Where(x => entity.TicketIds.Contains(x.Id))
                .ToListAsync();

            var isAnyTicketInvoiced = tickets.Any(x => !string.IsNullOrEmpty(x.InvoiceId));

            if (isAnyTicketInvoiced)
            {
                throw new Exception("Some tickets are already invoiced");
            }

            var ticketsWihoutCustomerSignature = tickets.Any(x => x.HasCustomerSignature == false);

            if (ticketsWihoutCustomerSignature)
            {
                throw new Exception("Some tickets are missing customer signature");
            }

            var isCustomer = tickets.All(x => x.CustomerId == invoice.CustomerId);

            if (!isCustomer)
            {
                throw new Exception("Some tickets do not have the same customer id");
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
            var invoice = await _context.Invoices
                .SingleAsync(x => x.Id == id);

            invoice.SentToCustomer = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var invoice = await _context.Invoices
                .SingleAsync(x => x.Id == id);
            _context.Invoices.Remove(invoice);
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
            var invoice = await _context.Invoices
                .Include(x => x.Tickets)
                .Include(x => x.Customer)
                .SingleAsync(x => x.Id == id);

            var invoiceReport = new InvoiceReport(invoice);

            var invoiceHtml = await _viewRenderer.RenderViewToStringAsync(_ticketTemplate, invoiceReport);

            var pdf = _pdfGeneratorService.GeneratePdf(invoiceHtml);

            return (invoice.InvoiceId, pdf);
        }

        public async Task<InvoiceDto> GetById(string id)
        {
            var invoice = await _context.Invoices
                .Include(x => x.Tickets).ThenInclude(x => x.Customer)
                .Include(x => x.Tickets).ThenInclude(x => x.Equipment)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
            {
                throw new KeyNotFoundException(nameof(Invoice));
            }

            return _mapper.Map<InvoiceDto>(invoice)!;
        }        
    }
}
