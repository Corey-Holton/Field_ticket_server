using AutoMapper;
using CA.Ticketing.Persistance.Context;

namespace CA.Ticketing.Business.Services.Base
{
    public class EntityServiceBase
    {
        protected readonly CATicketingContext _context;

        protected readonly IMapper _mapper;

        public EntityServiceBase(CATicketingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
