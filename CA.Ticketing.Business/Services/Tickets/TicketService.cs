using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CA.Ticketing.Business.Services.Tickets
{
    public class TicketService : EntityServiceBase, ITicketService
    {
        public TicketService(CATicketingContext context, IMapper mapper, IAccountsService accountsService) : base(context, mapper)
        {

        }
        // TO DO: IF FOREIGN KEY FOR INVOICE EXISTS THEN RETURN INVOICE TRUE IN MAPPER
        public async Task<IEnumerable<TicketDto>> GetAll()
        {
            var tickets = await _context.FieldTickets
                .Include(x => x.Customer)
                .Include(x => x.Equipment)
                .Include(x => x.Location)
                .ToListAsync();

            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<IEnumerable<TicketDto>> GetByDates(DateTime startDate, DateTime endDate)
        {
            var tickets = await _context.FieldTickets
               .Include(x => x.Customer)
               .Include(x => x.Equipment)
               .Include(x => x.Location)
               .Where(x => x.ExecutionDate >= startDate && x.ExecutionDate <= endDate)
               .ToListAsync();

            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<IEnumerable<TicketDto>> GetByLocation(string locationName)
        {
            var locations = await _context.CustomerLocations
                .Where(x => x.Name.Contains(locationName))
                .ToListAsync();

            var tickets = new List<FieldTicket>();

            foreach(var location in locations)
            {
                var ticketsResult = await _context.FieldTickets
                .Include(x => x.Customer)
                .Include(x => x.Equipment)
                .Include(x => x.Location)
                .Where(x => x.LocationId == location.Id)
                .ToListAsync();
                if (tickets != null)
                {
                    tickets.AddRange(ticketsResult);
                }
            }

            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<TicketDetailsDto> GetById(int id)
        {
            var ticket = await GetTicket(id);
            return _mapper.Map<TicketDetailsDto>(ticket);
        }

        public async Task<int> Create(TicketDetailsDto entity)
        {
            var ticket = _mapper.Map<FieldTicket>(entity);

            _context.FieldTickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket.Id;
        }

        public async Task Update(TicketDetailsDto entity)
        {
            var ticket = await GetTicket(entity.Id);
            _mapper.Map(entity, ticket);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var ticket = await GetTicket(id);
            _context.FieldTickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<FieldTicket> GetTicket(int id)
        {
            var ticket = await _context.FieldTickets
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if(ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            return ticket;
        }
    }
}
