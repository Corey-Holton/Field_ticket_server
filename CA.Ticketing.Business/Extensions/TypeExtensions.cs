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
            typeof(IdentityUserRole<string>),
            typeof(EmployeeNote)
        };

        public static List<Type> SyncHistory = new()
        {
            typeof(TicketSpecification),
            typeof(Scheduling),
            typeof(PayrollData),
            typeof(FieldTicket),
            typeof(InvoiceLateFee),
            typeof(Invoice),
            typeof(Employee),
            typeof(EquipmentFile),
            typeof(EquipmentCharge),
            typeof(Equipment),
            typeof(CustomerContact),
            typeof(CustomerLocation),
            typeof(Customer),
            typeof(Charge),
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
            "employeenote" => typeof(EmployeeNote),
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
