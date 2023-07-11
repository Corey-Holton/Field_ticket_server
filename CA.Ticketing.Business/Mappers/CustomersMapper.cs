using AutoMapper;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Mappers
{
    public class CustomersMapper : Profile
    {
        public CustomersMapper()
        {
            CreateMap<Customer, CustomerDto>();
        }
    }
}
