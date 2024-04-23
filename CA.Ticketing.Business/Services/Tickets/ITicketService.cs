using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Common.Models;

namespace CA.Ticketing.Business.Services.Tickets
{
    public interface ITicketService
    {
        Task<ListResult<TicketDto>> GetAll(int index, int size, string sorting, string order, string searchString);

        Task<IEnumerable<TicketDto>> GetByDates(DateTime startDate, DateTime endDate);

        Task<TicketDto> GetById(string id);

        Task UpdateWellRecord(WellRecordDto wellRecordDto);

        Task AddWellRecord(WellRecordDto wellRecordDto);

        Task RemoveWellRecord(WellRecordDto wellRecordDto);

        Task UpdateOtherDetails(ManageWellOtherDetailsDto manageWellOther);

        Task AddSwabCharge(SwabCupsDto swabCupsDto);

        Task UpdateSwabCharge(SwabCupsDto swabCupsDto);

        Task RemoveSwabCharge(SwabCupsDto swabChargeId);

        Task<string> Create(ManageTicketDto manageTicketDto);

        Task Update(ManageTicketDto manageTicketDto);

        Task UpdateHours(ManageTicketHoursDto manageTicketHours);

        Task Delete(string id);

        Task<List<PayrollDataDto>> GetPayrollData(string ticketId);

        Task<List<TicketSpecificationDto>> GetSpecialTicketSpec(string ticketId);

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

        Task SendToClient(string ticketId, string redirectUrl);
    }
}
