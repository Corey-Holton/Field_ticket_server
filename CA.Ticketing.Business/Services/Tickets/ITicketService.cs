using CA.Ticketing.Business.Services.Tickets.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAll();

        Task<IEnumerable<TicketDto>> GetByDates(DateTime startDate, DateTime endDate);

        Task<IEnumerable<TicketDto>> GetByLocation(string search);

        Task<TicketDetailsDto> GetById(int id);


        Task<int> Create(TicketDetailsDto entity);

        Task Update(TicketDetailsDto entity);


    }
}
