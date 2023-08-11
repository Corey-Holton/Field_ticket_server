using AutoMapper;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Customers
{
    public class CustomerService : EntityServiceBase, ICustomerService
    {
        private readonly IAccountsService _accountsService;
        public CustomerService(CATicketingContext context, IMapper mapper, IAccountsService accountsService) : base(context, mapper)
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
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer.Id;
        }

        public async Task Update(CustomerDetailsDto entity)
        {
            var customer = await GetCustomer(entity.Id);
            _mapper.Map(entity, customer);
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

        public async Task UpdateLocation(CustomerLocationDto entity)
        {
            var location = await GetLocation(entity.Id);
            _mapper.Map(entity, location);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLocation(int id)
        {
            var location = await GetLocation(id);
            _context.CustomerLocations.Remove(location);
            await _context.SaveChangesAsync();
        }

        public async Task<int> AddContact(CustomerContactDto entity)
        {
            var contact = _mapper.Map<CustomerContact>(entity);
            _context.CustomerContacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact.Id;
        }

        public async Task UpdateContact(CustomerContactDto entity)
        {
            var contact = await GetCustomerContact(entity.Id);
            _mapper.Map(entity, contact);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteContact(int id)
        {
            var contact = await GetCustomerContact(id);
            _context.CustomerContacts.Remove(contact);
            await _context.SaveChangesAsync();
        }

        public async Task AddLogin(CustomerLoginDto customerLoginDto)
        {
            var customerContact = await GetCustomerContact(customerLoginDto.CustomerContactId);

            var customerAddLoginModel = _mapper.Map<CreateCustomerContactLoginDto>(customerContact);
            customerAddLoginModel.RedirectUrl = customerLoginDto.RedirectUrl;
            await _accountsService.CreateCustomerContactLogin(customerAddLoginModel);
            customerContact.InviteSent = true;
            customerContact.InviteSentOn = DateTime.Now;
                
            await _context.SaveChangesAsync();
        }

        public async Task ResendInvitation(CustomerLoginDto customerLoginDto)
        {
            var customerContact = await GetCustomerContact(customerLoginDto.CustomerContactId);
            await _accountsService.ResendCustomerContactEmail(customerLoginDto);
            customerContact.InviteSentOn = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task ResetPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordModel)
        {
            await _accountsService.ResetCustomerContactPassword(resetCustomerContactPasswordModel);
        }

        public async Task DeleteLogin(int customerContactId)
        {
            var customerContact = await GetCustomerContact(customerContactId);
            await _accountsService.DeleteUser(customerContact.ApplicationUser!.Id);
        }

        private async Task<Customer> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(x => x.Locations)
                .Include(x => x.Contacts)
                    .ThenInclude(x => x.ApplicationUser)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                throw new KeyNotFoundException(nameof(Customer));
            }

            return customer;
        }

        private async Task<CustomerLocation> GetLocation(int id)
        {
            var location = await _context.CustomerLocations
                .SingleOrDefaultAsync (x => x.Id == id);

            if(location == null)
            {
                throw new KeyNotFoundException(nameof(CustomerLocation));
            }

            return location!;
        }

        private async Task<CustomerContact> GetCustomerContact(int id)
        {
            var customerContact = await _context.CustomerContacts
                .Include(x => x.ApplicationUser)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (customerContact == null)
            {
                throw new KeyNotFoundException(nameof(CustomerContact));
            }
            return customerContact;
        }
    }
}
