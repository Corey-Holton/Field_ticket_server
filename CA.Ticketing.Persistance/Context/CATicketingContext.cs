using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Persistance.Context
{
    public class CATicketingContext : IdentityDbContext<ApplicationUser>
    {
        public CATicketingContext(DbContextOptions<CATicketingContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
    }
}
