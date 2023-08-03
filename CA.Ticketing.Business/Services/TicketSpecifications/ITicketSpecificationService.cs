using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Business.Services.TicketSpecifications.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.TicketSpecifications
{
    public interface ITicketSpecificationService
    {
        Task<int> Create(CreateTicketSpecificationDto entity);

        Task Update(CreateTicketSpecificationDto entity);

        Task Delete(int id);
    }
}
