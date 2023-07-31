using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Persistance.Context;
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
    }
}
