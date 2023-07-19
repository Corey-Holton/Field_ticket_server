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
    }
}
