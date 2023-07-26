using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Tickets
{
    public class TicketService : EntityServiceBase, ITicketService
    {
        public TicketService(CATicketingContext context, IMapper mapper, IAccountsService accountsService) : base(context, mapper)
        {

        }

        public async Task<IEnumerable<TicketDto>> GetAll()
        {
            var tickets = await _context.FieldTickets
                .Include(x => x.Customer)
                .ToListAsync();

            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<int> Create(TicketDetailsDto entity)
        {
            var ticket = _mapper.Map<FieldTicket>(entity);

            _context.FieldTickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket.Id;
        }
    }
}
