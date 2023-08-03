using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Invoices
{
    public class InvoiceService : EntityServiceBase, IInvoiceService
    {
        public InvoiceService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {
            
        }

        public async Task<IEnumerable<InvoiceDto>> GetAll()
        {
            var invoices = await _context.Invoices
                .ToListAsync();
            return invoices.Select(x => _mapper.Map<InvoiceDto>(x));
        }

        public async Task<IEnumerable<InvoiceDto>> GetByDates(DateTime startDate, DateTime endDate)
        {
            var invoices = await _context.Invoices
                .Where(x => x.InvoiceDate >= startDate && x.InvoiceDate <= endDate)
                .ToListAsync();

            return invoices.Select(x => _mapper.Map<InvoiceDto>(x));
        }

        public async Task<InvoiceDetailsDto> GetById(int id)
        {
            var invoice = await GetInvoice(id);
            return _mapper.Map<InvoiceDetailsDto>(invoice);
        }

        public async Task<IEnumerable<InvoiceDto>> GetByCustomer(string customerName)
        {
            var customers = await _context.Customers
                .Where(x => x.Name.Contains(customerName))
                .ToListAsync();

            var fieldTickets = new List<FieldTicket>();

            foreach (var customer in customers)
            {
                var foundTickets = await _context.FieldTickets
                    .Where(x => x.Location.CustomerId == customer.Id)
                    .Include(x => x.Invoice)
                    .ToListAsync();
                if (foundTickets != null)
                    fieldTickets.AddRange(foundTickets); 
            }

            var invoices = new List<Invoice>();

            foreach (var ticket in fieldTickets)
            {
                if (ticket.Invoice != null)
                    if (!invoices.Contains(ticket.Invoice))
                        invoices.Add(ticket.Invoice);
            }

            return invoices.Select(x => _mapper.Map<InvoiceDto>(x));
        }

        public async Task<int> Create(CreateInvoiceDto entity)
        {
            var invoice = _mapper.Map<Invoice>(entity);

            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync();

            if (entity.ticketIds != null)
            {
                foreach(var ticketId in entity.ticketIds)
                {
                    var fieldTicket = await _context.FieldTickets
                        .Where(x => x.Id == ticketId)
                        .FirstOrDefaultAsync();

                    if (fieldTicket != null)
                        fieldTicket.InvoiceId = invoice.Id;
                } 
            }

            await _context.SaveChangesAsync();

            return invoice.Id;
        }

        public async Task Update(CreateInvoiceDto entity)
        {
            var invoice = await GetInvoice(entity.Id);
            
            _mapper.Map(entity, invoice);
            
            if (entity.ticketIds != null)
            {
                // Clear all tickets
                foreach (var ticket in invoice.Tickets)
                {
                    ticket.InvoiceId = null;
                }

                // Re-attach tickets that are in the array of ticketIds
                foreach (var id in entity.ticketIds)
                {
                    var fieldTicket = await _context.FieldTickets
                        .Where(x => x.Id == id)
                        .FirstOrDefaultAsync();

                    if (fieldTicket != null)
                    {
                        fieldTicket.InvoiceId = invoice.Id;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Invoice> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(x => x.Tickets).ThenInclude(x => x.Customer)
                .Include(x => x.Tickets).ThenInclude(x => x.Equipment)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
            {
                throw new KeyNotFoundException(nameof(Invoice));
            }

            return invoice!;
        }
    }
}
