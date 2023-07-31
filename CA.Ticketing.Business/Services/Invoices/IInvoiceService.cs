using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Business.Services.Tickets.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAll();

        Task<int> Create(InvoiceDto entity);

        Task Update(InvoiceDto entity);
    }
}
