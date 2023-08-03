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

        Task<IEnumerable<InvoiceDto>> GetByDates(DateTime startDate, DateTime endDate);

        Task<IEnumerable<InvoiceDto>> GetByCustomer(string search);

        Task<InvoiceDetailsDto> GetById(int id);

        Task<int> Create(CreateInvoiceDto entity);

        Task Update(CreateInvoiceDto entity);

        Task Delete(int id);
    }
}
