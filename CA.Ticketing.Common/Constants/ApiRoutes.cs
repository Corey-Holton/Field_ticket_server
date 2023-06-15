namespace CA.Ticketing.Common.Constants
{
    public class ApiRoutes
    {
        private const string _root = "api";

        public class Employees
        {
            public const string _root = $"{ApiRoutes._root}/employees";

            public const string List = _root;

            public const string Get = $"{_root}/{{employee-id}}";

            public const string Update = $"{Get}/update";
        }
    }
}
