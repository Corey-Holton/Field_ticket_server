using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Tickets.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
            var tickets = await GetTicketIncludes().ToListAsync();
            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<IEnumerable<TicketDto>> GetByDates(DateTime startDate, DateTime endDate)
        {
            var tickets = await GetTicketIncludes()
               .Where(x => x.ExecutionDate >= startDate && x.ExecutionDate <= endDate)
               .ToListAsync();

            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<IEnumerable<TicketDto>> GetByLocation(string locationName)
        {
            var locationIds = await _context.CustomerLocations
                .Where(x => x.Name.Contains(locationName))
                .Select(x => x.Id)
                .ToListAsync();

            var tickets = await GetTicketIncludes()
                .Where(x => x.LocationId.HasValue && locationIds.Contains(x.LocationId.Value))
                .ToListAsync();

            return tickets.Select(x => _mapper.Map<TicketDto>(x));
        }

        public async Task<TicketDetailsDto> GetById(int id)
        {
            var ticket = await GetTicket(id);
            return _mapper.Map<TicketDetailsDto>(ticket);
        }

        public async Task<TicketDetailsDto> Create()
        {
            var ticket = new FieldTicket
            {
                ExecutionDate = DateTime.UtcNow
            };

            var charges = 

            _context.FieldTickets.Add(ticket);
            await _context.SaveChangesAsync();
            return _mapper.Map<TicketDetailsDto>(ticket);
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

        private async Task<FieldTicket> GetTicket(int id)
        {
            var ticket = await GetTicketIncludes(true)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                throw new KeyNotFoundException(nameof(FieldTicket));
            }

            return ticket;
        }

        private IQueryable<FieldTicket> GetTicketIncludes(bool includeSpecs = false)
        {
            var baseIncludes = _context.FieldTickets
                .Include(x => x.Customer)
                .Include(x => x.Equipment)
                .Include(x => x.Location);

            if (!includeSpecs)
            {
                return baseIncludes;
            }

            return baseIncludes
                .Include(x => x.TicketSpecifications);
        }
    }
}
