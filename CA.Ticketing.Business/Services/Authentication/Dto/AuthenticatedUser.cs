namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class AuthenticatedUser
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string Token { get; set; }
    }
}
