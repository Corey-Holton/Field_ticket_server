using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using static CA.Ticketing.Common.Constants.ApiRoutes;

namespace CA.Ticketing.Business.Services.Customers
{
    public class CustomerService : EntityServiceBase, ICustomerService
    {
        private readonly IAccountsService _accountsService;
        public CustomerService(CATicketingContext context, IMapper mapper, IAccountsService accountsService) : base (context, mapper)
        {
            _accountsService = accountsService;
        }

        public async Task<IEnumerable<CustomerDto>> GetAll()
        {
            var customers = await _context.Customers
                .ToListAsync();
            return customers.Select(x => _mapper.Map<CustomerDto>(x));
        }

        public async Task<CustomerDetailsDto> GetById(int id)
        {
            var customer = await GetCustomer(id);
            return _mapper.Map<CustomerDetailsDto>(customer);
        }

        public async Task<int> Create(CustomerDetailsDto entity)
        {
            var customer = _mapper.Map<Customer>(entity);
            
            if(entity.Locations != null)
                customer.Locations = _mapper.Map<List<CustomerLocation>>(entity.Locations);
            
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer.Id;
        }

        public async Task Update(CustomerDetailsDto entity)
        {
            var customer = await GetCustomer(entity.Id);
            
            _mapper.Map(entity, customer);
            
            if(entity.Locations != null)
                customer.Locations = _mapper.Map<List<CustomerLocation>>(entity.Locations);
            
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var customer = await GetCustomer(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<int> AddLocation(CustomerLocationDto entity)
        {
            var location = _mapper.Map<CustomerLocation>(entity);
            _context.CustomerLocations.Add(location);
            await _context.SaveChangesAsync();
            return location.Id;
        }

        public async Task AddLogin(int customerId)
        {
            var customerContact = await GetCustomerContact(customerId);
            if (!customerContact.InviteSent)
            {
                var customerAddLoginModel = _mapper.Map<CreateCustomerContactLoginDto>(customerContact);
                await _accountsService.CreateCustomerContactLogin(customerAddLoginModel);
                customerContact.InviteSent = true;
                customerContact.InviteSentOn = DateTime.Now;
            }
            else if(customerContact.InviteSent)
            {
                ResendInviteDto inviteDto = new ResendInviteDto();
                inviteDto.CustomerContactId = customerContact.Id;
                await _accountsService.ResendCustomerContactEmail(inviteDto);
                customerContact.InviteSentOn = DateTime.Now;
            }
                
            await _context.SaveChangesAsync();
        }

        public async Task AddPassword(AddCustomerContactPasswordDto addCustomerContactPasswordModel)
        {
            var customerAddPasswordModel = _mapper.Map<SetCustomerPasswordDto>(addCustomerContactPasswordModel);
            await _accountsService.SetCustomerPassword(customerAddPasswordModel);
        }

        private async Task<Customer> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(x => x.Locations)
                .ThenInclude(x => x.Contacts)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                throw new KeyNotFoundException(nameof(Customer));
            }

            return customer!;
        }

        private async Task<CustomerContact> GetCustomerContact(int id)
        {
            var customerContact = await _context.CustomerContacts
                .SingleOrDefaultAsync(x => x.Id == id);
            if (customerContact == null)
            {
                throw new KeyNotFoundException(nameof(CustomerContact));
            }
            return customerContact;
        }
    }
}
