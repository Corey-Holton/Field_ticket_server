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

            public const string AddLocation = $"{_root}/add-location";

            public const string UpdateLocation = $"{_root}/update-location";

            public const string DeleteLocation = $"{_root}/delete-location";

            public const string AddContact = $"{_root}/add-contact";

            public const string UpdateContact = $"{_root}/update-contact";

            public const string DeleteContact = $"{_root}/delete-contact";

            public const string AddLogin = $"{_root}/add-login";

            public const string ResendInvite = $"{_root}/resend-invite";

            public const string ResetPassword = $"{_root}/reset-password";

            public const string DeleteLogin = $"{_root}/delete-login";
        }

        public class Charges
        {
            public const string _root = $"{ApiRoutes._root}/charges";

            public const string List = $"{_root}/list";

            public const string Update = $"{_root}/update";
        }

        public class Settings
        {
            public const string _root = $"{ApiRoutes._root}/settings";

            public const string Get = _root;

            public const string Update = $"{_root}/update";

            public const string GetProfile = $"{_root}/profile";

            public const string UpdateProfile = $"{_root}/profile/update";
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

            public const string ListEquipmentCharges = $"{_root}/{{equipmentId}}/charges/list";

            public const string UpdateEquipmentCharges = $"{_root}/{{equipmentId}}/charges/update";

            public const string PermitExpirationDate = $"{_root}/permits-expiration";

            public const string RigsNotWorking = $"{_root}/rigs-not-working";

            public const string FilesList = $"{_root}/files";

            public const string Upload = $"{FilesList}/upload";

            public const string DeleteFile = $"{FilesList}/delete";

            public const string Download = $"{FilesList}/download";
        }

        public class Tickets
        {
            public const string _root = $"{ApiRoutes._root}/tickets";

            public const string List = $"{_root}/list";

            public const string ListByDates = $"{_root}/list/startDate={{startDate}}&endDate={{endDate}}";

            public const string Get = $"{_root}/{{ticketId}}";

            public const string Create = $"{_root}/create";

            public const string UpdateDetails = $"{_root}/update-details";

            public const string UpdateHours = $"{_root}/update-hours";

            public const string UpdateSpecifications = $"{_root}/update-specs";

            public const string AddPayrollEntry = $"{_root}/payroll/add";

            public const string UpdatePayrollEntry = $"{_root}/payroll/update";

            public const string DeletePayrollEntry = $"{_root}/payroll/delete";

            public const string Delete = $"{_root}/delete";

            public const string Preview = $"{_root}/preview";

            public const string EmployeeSignature = $"{_root}/sign";

            public const string CustomerSignature = $"{_root}/customer-sign";

            public const string CustomerUpload = $"{_root}/upload";

            public const string Download = $"{_root}/download";

            public const string Reset = $"{_root}/reset";
        }

        public class Payrolls
        {
            public const string _root = $"{ApiRoutes._root}/payrolls";

            public const string GetPayrolls = $"{_root}/list";
        }

        public class Invoices
        {
            public const string _root = $"{ApiRoutes._root}/invoices";

            public const string List = $"{_root}/list";
                
            public const string Create = $"{_root}/create";

            public const string MarkAsPaid = $"{_root}/mark-paid";

            public const string SendToCustomer = $"{_root}/send";

            public const string Download = $"{_root}/download";

            public const string Delete = $"{_root}/delete";
        }

        public class Scheduling
        {
            public const string _root = $"{ApiRoutes._root}/scheduling";

            public const string List = $"{_root}/list";

            public const string GetForRig = $"{_root}/rigList";

            public const string Get = $"{_root}/{{schedulingId}}";

            public const string Create = $"{_root}/create";

            public const string Update = $"{_root}/update";

            public const string Delete = $"{_root}/delete";

        }

        public class Authentication
        {
            public const string _root = $"{ApiRoutes._root}/authentication";

            public const string Login = $"{_root}/login";

            public const string GenerateResetPasswordLink = $"{_root}/GenerateResetPasswordLink";

            public const string GetEmployeeResetPasswordToken = $"{_root}/reset-employee-password";

            public const string SetPasswordFromLink = $"{_root}/SetPasswordFromLink";

            public const string ChangePassword = $"{_root}/account/change-password";
        }

        public class Users
        {
            public const string _root = $"{ApiRoutes._root}/users";

            public const string List = $"{_root}/list";

            public const string Get = $"{_root}/{{userId}}";

            public const string Create = $"{_root}/create";

            public const string Update = $"{_root}/update";

            public const string Delete = $"{_root}/delete";

            public const string ResetPassword = $"{_root}/reset-password";
        }

        public class Sync
        {
            private const string _root = $"{ApiRoutes._root}/sync";

            public const string GetOrUpdateGeneric = $"{_root}/{{entityType}}";

            public const string Status = $"{_root}/status";

            public const string Health = $"{_root}/health";
        }
    }
}
