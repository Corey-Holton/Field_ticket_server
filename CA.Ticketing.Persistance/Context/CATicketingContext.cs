using CA.Ticketing.Common.Constants;
using CA.Ticketing.Persistance.Models;
using CA.Ticketing.Persistance.Models.Abstracts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CA.Ticketing.Persistance.Context
{
    public class CATicketingContext : IdentityDbContext<ApplicationUser>
    {
        public CATicketingContext(DbContextOptions<CATicketingContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeNote> EmployeeNotes { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerLocation> CustomerLocations { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }

        public DbSet<Equipment> Equipment { get; set; }

        public DbSet<EquipmentFile> EquipmentFiles { get; set; }

        public DbSet<Charge> Charges { get; set; }

        public DbSet<EquipmentCharge> EquipmentCharges { get; set; }

        public DbSet<FieldTicket> FieldTickets { get; set; }

        public DbSet<TicketSpecification> TicketSpecifications { get; set; }

        public DbSet<PayrollData> PayrollData { get; set; }

        public DbSet<WellRecord> WellRecord { get; set; }

        public DbSet<SwabCups> SwabCups { get; set; }

        public DbSet<TicketType> TicketType { get; set; }

        public DbSet<Scheduling> Scheduling { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceLateFee> InvoiceLateFees { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<SyncData> SyncData { get; set; }

        public DbSet<SyncServerInfo> ServerSyncHistory { get; set; }

        public DbSet<BackgroundJob> BackgroundJobs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Setting>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<Invoice>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<Scheduling>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<PayrollData>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<TicketSpecification>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<FieldTicket>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<EquipmentCharge>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<Charge>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<Equipment>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<Customer>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<CustomerLocation>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<CustomerContact>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<Employee>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<EmployeeNote>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<SwabCups>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<WellRecord>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<EquipmentFile>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<InvoiceLateFee>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<TicketType>().HasQueryFilter(x => !x.DeletedDate.HasValue);
            builder.Entity<TicketType>()
                .HasMany(x => x.IncludedCharges)
                .WithMany(x => x.TicketTypes)
                .UsingEntity<Dictionary<string, object>>(
                    TableNames.TypeCharges,
                    j => j.HasOne<Charge>()
                    .WithMany()
                    .HasForeignKey("ChargeId")
                    .HasConstraintName("FK_TypeCharges_Charge_ChargeId")
                    .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<TicketType>()
                    .WithMany()
                    .HasForeignKey("TicketTypeId")
                    .HasConstraintName("FK_TypeCharges_TicketType_TicketTypeId")
                    .OnDelete(DeleteBehavior.Cascade))
                .ToTable(TableNames.TypeCharges);
            builder.Entity<TicketType>()
                .HasMany(x => x.SpecialCharges)
                .WithMany(x => x.SpecialChargesTicketTypes)
                .UsingEntity<Dictionary<string, object>>(
                    TableNames.SpecialTypeCharges,
                    j => j.HasOne<Charge>()
                    .WithMany()
                    .HasForeignKey("ChargeId")
                    .HasConstraintName("FK_SpecialTypeCharges_Charge_ChargeId")
                    .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<TicketType>()
                    .WithMany()
                    .HasForeignKey("TicketTypeId")
                    .HasConstraintName("FK_SpecialTypeCharges_TicketType_TicketTypeId")
                    .OnDelete(DeleteBehavior.Cascade))
                .ToTable(TableNames.SpecialTypeCharges);
            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var syncedEntities = this.ChangeTracker.Entries()
                .Where(x => typeof(ISyncedEntity).IsAssignableFrom(x.Entity.GetType()))
                .ToList();

            foreach (var syncedEntity in syncedEntities)
            {
                if (syncedEntity.State == EntityState.Deleted)
                {
                    syncedEntity.State = EntityState.Modified;
                    var entityCast = (ISyncedEntity)syncedEntity.Entity;
                    entityCast.LastModifiedDate = DateTime.UtcNow;
                    entityCast.DeletedDate = DateTime.UtcNow;
                }

                if (syncedEntity.State == EntityState.Added)
                {
                    var entityCast = (ISyncedEntity)syncedEntity.Entity;
                    entityCast.CreatedDate = DateTime.UtcNow;
                    entityCast.LastModifiedDate = DateTime.UtcNow;
                }

                if (syncedEntity.State == EntityState.Modified)
                {
                    var entityCast = (ISyncedEntity)syncedEntity.Entity;
                    entityCast.LastModifiedDate = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
