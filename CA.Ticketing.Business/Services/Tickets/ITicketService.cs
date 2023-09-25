using CA.Ticketing.Business.Services.Tickets.Dto;

namespace CA.Ticketing.Business.Services.Tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAll();

        Task<IEnumerable<TicketDto>> GetByDates(DateTime startDate, DateTime endDate);

        Task<TicketDetailsDto> GetById(string id);

        Task<string> Create(ManageTicketDto manageTicketDto);

        Task Update(ManageTicketDto manageTicketDto);

        Task UpdateHours(ManageTicketHoursDto manageTicketHours);

        Task Delete(string id);

        Task<List<PayrollDataDto>> GetPayrollData(string ticketId);

        Task AddPayroll(PayrollDataDto payrollDataDto);

        Task UpdatePayrollData(PayrollDataDto payrollDataDto);

        Task RemovePayroll(string payrollDataId);

        Task<UpdateTicketSpecResponse> UpdateTicketSpecification(TicketSpecificationDto ticketSpecificationDto);

        Task<string> PreviewTicket(string ticketId);

        Task EmployeeSignature(SignatureBaseDto signatureBaseDto);

        Task CustomerSignature(CustomerSignatureDto customerSignatureDto);

        Task<byte[]> DownloadTicket(string ticketId);

        Task UploadTicket(Stream fileStream, string ticketId);

        Task ResetSignatures(string ticketId);

    }
}
