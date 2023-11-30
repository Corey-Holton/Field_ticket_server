using AutoMapper;
using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.Authentication;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Base;
using CA.Ticketing.Business.Services.Customers.Dto;
using CA.Ticketing.Business.Services.Removal;
using CA.Ticketing.Common.Authentication;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Customers
{
    public class CustomerService : EntityServiceBase, ICustomerService
    {
        private readonly IAccountsService _accountsService;

        private readonly IUserContext _userContext;

        private readonly IRemovalService _removalService;
        public CustomerService(
            CATicketingContext context,
            IMapper mapper,
            IAccountsService accountsService,
            IUserContext userContext,
            IRemovalService removalService) : base(context, mapper)
        {
            _accountsService = accountsService;
            _userContext = userContext;
            _removalService = removalService;
        }

        public async Task<IEnumerable<CustomerDto>> GetAll()
        {
            var customers = await _context.Customers
                .OrderBy(c => c.Name)
                .ToListAsync();

            if (_userContext.User!.Role == Common.Enums.ApplicationRole.Customer)
            {
                var customerContact = _context.CustomerContacts.Single(x => x.Id == _userContext.User!.CustomerContactId!);
                customers = customers.Where(x => x.Id == customerContact.CustomerId).ToList();
            }

            return customers.Select(x => _mapper.Map<CustomerDto>(x));
        }

        public async Task<CustomerDetailsDto> GetById(string id)
        {
            var customer = await GetCustomer(id);

            if (customer == null)
            {
                return new CustomerDetailsDto();
            }

            return _mapper.Map<CustomerDetailsDto>(customer);
        }

        public async Task<string> Create(CustomerDetailsDto entity)
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

        public async Task Delete(string id)
        {
            var customer = await _context.Customers
                .Include(x => x.Locations)
                    .ThenInclude(x => x.ScheduledJobs)
                .Include(x => x.Contacts)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.Contacts)
                    .ThenInclude(x => x.ScheduledJobs)
                .Include(x => x.Tickets)
                .Include(x => x.Invoices)
                .Include(x => x.ScheduledJobs)
                .AsSplitQuery()
                .SingleAsync(x => x.Id == id);

            _removalService.Remove(customer);

            await _context.SaveChangesAsync();
        }

        public async Task<string> AddLocation(CustomerLocationDto entity)
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

        public async Task DeleteLocation(string id)
        {
            var location = await _context.CustomerLocations
                .Include(x => x.FieldTickets)
                .Include(x => x.ScheduledJobs)
                .AsSplitQuery()
                .SingleAsync(x => x.Id == id);

            _removalService.Remove(location);

            await _context.SaveChangesAsync();
        }

        public async Task<string> AddContact(CustomerContactDto entity)
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

        public async Task DeleteContact(string id)
        {
            var contact = await _context.CustomerContacts
               .Include(x => x.ApplicationUser)
               .Include(x => x.ScheduledJobs)
               .SingleAsync(x => x.Id == id);

            _removalService.Remove(contact);
            
            await _context.SaveChangesAsync();
        }

        public async Task AddLogin(CustomerLoginDto customerLoginDto)
        {
            var customerContact = await GetCustomerContact(customerLoginDto.CustomerContactId);

            var customerAddLoginModel = _mapper.Map<CreateCustomerContactLoginDto>(customerContact);
            customerAddLoginModel.RedirectUrl = customerLoginDto.RedirectUrl;
            await _accountsService.CreateCustomerContactLogin(customerAddLoginModel);
            customerContact.InviteSent = true;
            customerContact.InviteSentOn = DateTime.UtcNow;
                
            await _context.SaveChangesAsync();
        }

        public async Task ResendInvitation(CustomerLoginDto customerLoginDto)
        {
            var customerContact = await GetCustomerContact(customerLoginDto.CustomerContactId);
            await _accountsService.ResendCustomerContactEmail(customerLoginDto);
            customerContact.InviteSentOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task ResetPassword(ResetCustomerContactPasswordDto resetCustomerContactPasswordModel)
        {
            await _accountsService.ResetCustomerContactPassword(resetCustomerContactPasswordModel);
        }

        public async Task DeleteLogin(string customerContactId)
        {
            var customerContact = await GetCustomerContact(customerContactId);
            await _accountsService.DeleteUser(customerContact.ApplicationUser!.Id);
        }

        private async Task<Customer?> GetCustomer(string? id)
        {
            var customer = await _context.Customers
                .Include(x => x.Locations)
                .Include(x => x.Contacts)
                    .ThenInclude(x => x.ApplicationUser)
                .SingleOrDefaultAsync(x => x.Id == id);

            return customer;
        }

        private async Task<CustomerLocation> GetLocation(string? id)
        {
            var location = await _context.CustomerLocations
                .SingleOrDefaultAsync (x => x.Id == id);

            if(location == null)
            {
                throw new KeyNotFoundException(nameof(CustomerLocation));
            }

            return location!;
        }

        private async Task<CustomerContact> GetCustomerContact(string? id)
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
