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

        Task RemovePayroll(int payrollDataId);

        Task<UpdateTicketSpecResponse> UpdateTicketSpecification(TicketSpecificationDto ticketSpecificationDto);
    }
}
