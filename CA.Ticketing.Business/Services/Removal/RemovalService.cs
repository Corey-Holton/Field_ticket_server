using CA.Ticketing.Business.Extensions;
using CA.Ticketing.Business.Services.FileManager;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using CA.Ticketing.Persistance.Models.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Business.Services.Removal
{
    public class RemovalService : IRemovalService
    {
        private readonly CATicketingContext _context;

        private readonly IFileManagerService _fileManagerService;

        public RemovalService(CATicketingContext context, IFileManagerService fileManagerService)
        {
            _context = context;
            _fileManagerService = fileManagerService;
        }

        public void Remove<T>(T entity) where T : IdentityModel
        {
            if (entity is Customer customer)
            {
                RemoveCustomer(customer);
                return;
            }
            if (entity is CustomerLocation customerLocation)
            {
                RemoveCustomerLocation(customerLocation);
                return;
            }
            if (entity is CustomerContact customerContact)
            {
                RemoveCustomerContact(customerContact);
                return;
            }
            if (entity is Equipment equipment)
            {
                RemoveEquipment(equipment);
                return;
            }
            if (entity is Employee employee)
            {
                RemoveEmployee(employee);
                return;
            }
            if (entity is Invoice invoice)
            {
                RemoveInvoice(invoice);
                return;
            }
            if (entity is FieldTicket ticket)
            {
                RemoveTicket(ticket);
                return;
            }
            if(entity is EmployeeNote note)
            {
                RemoveNote(note);
                return;
            }
        }

        private void RemoveCustomer(Customer customer)
        {
            if (customer.Tickets.Any())
            {
                throw new Exception("Can't delete customer. There are tickets created for this customer");
            }

            if (customer.Invoices.Any())
            {
                throw new Exception("Can't delete customer. There are invoices created for this customer");
            }

            customer.Locations.ToList()
                .ForEach(x => RemoveCustomerLocation(x));

            customer.Contacts.ToList()
                .ForEach(x => RemoveCustomerContact(x));

            customer.ScheduledJobs.ToList()
                .DeleteRelated(_context);

            _context.Entry(customer).State = EntityState.Deleted;
        }

        private void RemoveCustomerLocation(CustomerLocation location)
        {
            if (location.FieldTickets.Any())
            {
                throw new Exception("Can't delete location. There are tickets connected to it");
            }

            location.ScheduledJobs.ToList()
                .ForEach(x => x.CustomerLocationId = null);

            _context.Entry(location).State = EntityState.Deleted;
        }

        private void RemoveCustomerContact(CustomerContact customerContact)
        {
            customerContact.ScheduledJobs.ToList()
                .ForEach(x => x.CustomerContactId = null);

            if (customerContact.ApplicationUser != null)
            {
                _context.Entry(customerContact.ApplicationUser).State = EntityState.Deleted;
            }

            _context.Entry(customerContact).State = EntityState.Deleted;
        }

        private void RemoveEquipment(Equipment equipment)
        {
            if (equipment.Tickets.Any())
            {
                throw new Exception("Can't delete rig. There are tickets connected to it");
            }

            equipment.ScheduledJobs.DeleteRelated(_context);

            equipment.Charges.DeleteRelated(_context);

            equipment.Crew.ToList()
                .ForEach(x => x.AssignedRigId = null);

            equipment.Files.ToList()
                .ForEach(x => _fileManagerService.DeleteFile(Path.Combine(FilePaths.Equipment, equipment.Id), x.FileIndicator));

            equipment.Files.DeleteRelated(_context);

            _context.Entry(equipment).State = EntityState.Deleted;
        }

        private void RemoveEmployee(Employee employee)
        {
            if (employee.Payrolls.Any())
            {
                throw new Exception("Can't delete employee. There are payrolls connected to it");
            }

            if (employee.ApplicationUser != null)
            {
                _context.Entry(employee.ApplicationUser).State = EntityState.Deleted;
            }

            _context.Entry(employee).State = EntityState.Deleted;
        }

        private void RemoveInvoice(Invoice invoice)
        {
            invoice.Tickets.ToList()
                .ForEach(x => x.InvoiceId = null);

            invoice.InvoiceLateFees.DeleteRelated(_context);

            _context.Entry(invoice).State = EntityState.Deleted;
        }

        private void RemoveTicket(FieldTicket ticket)
        {
            if (!string.IsNullOrEmpty(ticket.InvoiceId))
            {
                throw new Exception("Can't delete ticket. It has already been invoiced");
            }

            ticket.PayrollData.DeleteRelated(_context);
            ticket.TicketSpecifications.DeleteRelated(_context);
            ticket.EmployeeNotes.DeleteRelated(_context);
            ticket.WellRecord.DeleteRelated(_context);

            _fileManagerService.DeleteFile(FilePaths.Tickets, ticket.FileName);

            _context.Entry(ticket).State = EntityState.Deleted;
        }

        private void RemoveNote(EmployeeNote note)
        {
            _context.Entry(note).State = EntityState.Deleted;
        }
    }
}
