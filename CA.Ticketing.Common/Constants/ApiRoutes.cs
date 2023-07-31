namespace CA.Ticketing.Common.Constants
{
    public class ApiRoutes
    {
        private const string _root = "api";

        public class Employees
        {
            public const string _root = $"{ApiRoutes._root}/employees";

            public const string List = $"{_root}/list";

            public const string Get = $"{_root}/{{employeeId}}";

            public const string Create = $"{_root}/create";

            public const string Update = $"{_root}/update";

            public const string Delete = $"{_root}/delete";

            public const string AddLogin = $"{_root}/add-login";

            public const string ResetPassword = $"{_root}/reset-password";

            public const string DeleteLogin = $"{_root}/delete-login";

            public const string Birthdays = $"{_root}/birthdays";

            public const string WorkAnniversairies = $"{_root}/work-anniversairies";
        }

        public class Customers
        {
            public const string _root = $"{ApiRoutes._root}/customers";

            public const string List = $"{_root}/list";

            public const string Get = $"{_root}/{{customerId}}";

            public const string Create = $"{_root}/create";

            public const string Update = $"{_root}/update";

            public const string Delete = $"{_root}/delete";

            public const string LocationList = $"{_root}/location-list/{{customerId}}";

            public const string AddLocation = $"{_root}/add-location";

            public const string UpdateLocation = $"{_root}/update-location";

            public const string DeleteLocation = $"{_root}/delete-location";

            public const string AddContact = $"{_root}/add-contact";

            public const string UpdateContact = $"{_root}/update-contact";

            public const string DeleteContact = $"{_root}/delete-contact";

            public const string AddLogin = $"{_root}/add-login";

            public const string AddPassword = $"{_root}/add-password";

            public const string ResetPassword = $"{_root}/reset-password";

            public const string DeleteLogin = $"{_root}/delete-login";
        }

        public class Equipment
        {
            public const string _root = $"{ApiRoutes._root}/equipment";

            public const string List = $"{_root}/list";

            public const string ListCategory = $"{_root}/list/{{equipmentCategory}}";

            public const string Get = $"{_root}/{{equipmentId}}";

            public const string Create = $"{_root}/create";

            public const string Update = $"{_root}/update";

            public const string Delete = $"{_root}/delete";

            public const string CreateEquipmentCharge = $"{_root}/create-equipment-charge";

            public const string UpdateEquipmentCharge = $"{_root}/update-equipment-charge";

            public const string DeleteEquipmentCharge = $"{_root}/delete-equipment-charge";
        }

        public class Tickets
        {
            public const string _root = $"{ApiRoutes._root}/tickets";

            public const string List = $"{_root}/list";

            public const string ListByDates = $"{_root}/list/startDate={{startDate}}&endDate={{endDate}}";

            public const string ListByLocation = $"{_root}/list/location={{search}}";

            public const string Get = $"{_root}/{{ticketId}}";

            public const string Create = $"{_root}/create";

            public const string Update = $"{_root}/update";
        }

        public class Invoices
        {
            public const string _root = $"{ApiRoutes._root}/invoices";

            public const string List = $"{_root}/list";

            public const string Create = $"{_root}/create";

            public const string Update = $"{_root}/update";
        }

        public class Scheduling
        {
            public const string _root = $"{ApiRoutes._root}/scheduling";

            public const string List = $"{_root}/list";

            public const string Get = $"{_root}/{{schedulingId}}";

            public const string Create = $"{_root}/create";

            public const string Update = $"{_root}/update";

            public const string Delete = $"{_root}/delete";

        }

        public class Authentication
        {
            public const string _root = $"{ApiRoutes._root}/authentication";

            public const string Login = $"{_root}/login";

            public const string EmailLogin = $"{_root}/email-login";

            public const string GenerateResetPasswordLink = $"{_root}/GenerateResetPasswordLink";

            public const string SetPasswordFromLink = $"{_root}/SetPasswordFromLink";
        }
    }
}
