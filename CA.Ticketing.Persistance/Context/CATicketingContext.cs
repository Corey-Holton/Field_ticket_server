using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Persistance.Context
{
    public class CATicketingContext : IdentityDbContext<ApplicationUser>
    {
        public CATicketingContext(DbContextOptions<CATicketingContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerLocation> CustomerLocations { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }

        public DbSet<Equipment> Equipment { get; set; }

        public DbSet<Charge> Charges { get; set; }

        public DbSet<EquipmentCharge> EquipmentCharges { get; set; }

        public DbSet<FieldTicket> FieldTickets { get; set; }

        public DbSet<TicketSpecification> TicketSpecifications { get; set; }

        public DbSet<PayrollData> PayrollData { get; set; }

        public DbSet<Scheduling> Scheduling { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Setting> Settings { get; set; }
    }
}
