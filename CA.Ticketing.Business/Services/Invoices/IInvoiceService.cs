using CA.Ticketing.Business.Services.Invoices.Dto;

namespace CA.Ticketing.Business.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAll();

        Task<int> Create(CreateInvoiceDto entity);

        Task MarkAsPaid(int id);

        Task SendToCustomer(int id);

        Task<(string InvoiceId, byte[] InvoiceBytes)> Download(int id);

        Task Delete(int id);
    }
}
