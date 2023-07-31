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

        public async Task<int> Create(InvoiceDto entity)
        {
            var invoice = _mapper.Map<Invoice>(entity);

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice.Id;
        }

        public async Task Update(InvoiceDto entity)
        {
            var invoice = await GetInvoice(entity.Id);

            _mapper.Map(entity, invoice);

            await _context.SaveChangesAsync();
        }

        private async Task<Invoice> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .SingleOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
            {
                throw new KeyNotFoundException(nameof(Invoice));
            }

            return invoice!;
        }
    }
}
