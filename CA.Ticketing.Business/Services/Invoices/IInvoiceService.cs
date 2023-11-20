using CA.Ticketing.Business.Services.Invoices.Dto;
using CA.Ticketing.Common.Models;

namespace CA.Ticketing.Business.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<ListResult<InvoiceDto>> GetAll(int index, int size, string sorting, string order, string searchString);

        Task<IEnumerable<InvoiceDto>> GetByDueDate();
        Task<InvoiceDto> GetById(string id);

        Task<string> Create(CreateInvoiceDto entity);

        Task MarkAsPaid(string id);

        Task SendToCustomer(string id);

        Task<(string InvoiceId, byte[] InvoiceBytes)> Download(string id);

        Task Delete(string id);

        Task RemoveLateFee(string id);
    }
}
