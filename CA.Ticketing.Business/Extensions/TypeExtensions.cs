using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Identity;

namespace CA.Ticketing.Business.Extensions
{
    public static class TypeExtensions
    {
        public static List<Type> SyncEntities = new()
        {
            typeof(Charge),
            typeof(Customer),
            typeof(CustomerLocation),
            typeof(CustomerContact),
            typeof(Equipment),
            typeof(EquipmentCharge),
            typeof(EquipmentFile),
            typeof(Employee),
            typeof(Invoice),
            typeof(InvoiceLateFee),
            typeof(FieldTicket),
            typeof(PayrollData),
            typeof(Scheduling),
            typeof(Setting),
            typeof(TicketSpecification),
            typeof(ApplicationUser),
            typeof(IdentityRole),
            typeof(IdentityUserRole<string>)
        };

        public static Type GetTypeFromString(string entityType) => entityType.ToLower() switch
        {
            "charge" => typeof(Charge),
            "customer" => typeof(Customer),
            "customerlocation" => typeof(CustomerLocation),
            "customercontact" => typeof(CustomerContact),
            "equipment" => typeof(Equipment),
            "equipmentcharge" => typeof(EquipmentCharge),
            "equipmentfile" => typeof(EquipmentFile),
            "employee" => typeof(Employee),
            "invoice" => typeof(Invoice),
            "invoicelatefee" => typeof(InvoiceLateFee),
            "fieldticket" => typeof(FieldTicket),
            "payrolldata" => typeof(PayrollData),
            "scheduling" => typeof(Scheduling),
            "setting" => typeof(Setting),
            "ticketspecification" => typeof(TicketSpecification),
            "applicationuser" => typeof(ApplicationUser),
            "identityrole" => typeof(IdentityRole),
            "identityuserrole`1" => typeof(IdentityUserRole<string>),
            _ => throw new Exception("Unknown type requested")
        };
    }
}
