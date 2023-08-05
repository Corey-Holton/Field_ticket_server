using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.TicketSpecifications.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.TicketSpecifications
{
    public class TicketSpecificationService : EntityServiceBase, ITicketSpecificationService
    {
        public TicketSpecificationService(CATicketingContext context, IMapper mapper) : base(context, mapper)
        {

        }

        public async Task<int> Create(CreateTicketSpecificationDto entity)
        {
            var ticketSpecification = _mapper.Map<TicketSpecification>(entity);
            _context.TicketSpecifications.Add(ticketSpecification);
            await _context.SaveChangesAsync();
            return ticketSpecification.Id;
        }

        public async Task Update(CreateTicketSpecificationDto entity)
        {
            var ticketSpecification = await GetTicketSpecification(entity.Id);
            _mapper.Map(entity, ticketSpecification);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var ticketSpecification = await GetTicketSpecification(id);
            _context.TicketSpecifications.Remove(ticketSpecification);
            await _context.SaveChangesAsync();
        }

        private async Task<TicketSpecification> GetTicketSpecification(int id)
        {
            var ticketSpecification = await _context.TicketSpecifications
                .SingleOrDefaultAsync(x => x.Id == id);
            
            if(ticketSpecification == null)
            {
                throw new KeyNotFoundException(nameof(TicketSpecification));
            }

            return ticketSpecification!;
        }
    }
}
