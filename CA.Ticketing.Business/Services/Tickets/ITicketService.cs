using CA.Ticketing.Business.Services.Tickets.Dto;

namespace CA.Ticketing.Business.Services.Tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAll();

        Task<IEnumerable<TicketDto>> GetByDates(DateTime startDate, DateTime endDate);

        Task<TicketDetailsDto> GetById(int id);

        Task<int> Create(ManageTicketDto manageTicketDto);

        Task Update(ManageTicketDto manageTicketDto);

        Task UpdateHours(ManageTicketHoursDto manageTicketHours);

        Task Delete(int id);

        Task AddPayroll(PayrollDataDto payrollDataDto);

        Task UpdatePayrollData(PayrollDataDto payrollDataDto);

        Task RemovePayroll(int payrollDataId);

        Task<UpdateTicketSpecResponse> UpdateTicketSpecification(TicketSpecificationDto ticketSpecificationDto);

        Task<string> PreviewTicket(int ticketId);

        Task EmployeeSignature(SignatureBaseDto signatureBaseDto);

        Task CustomerSignature(CustomerSignatureDto customerSignatureDto);

        Task<byte[]> DownloadTicket(int ticketId);

        Task UploadTicket(Stream fileStream, int ticketId);

        Task ResetSignatures(int ticketId);

    }
}
