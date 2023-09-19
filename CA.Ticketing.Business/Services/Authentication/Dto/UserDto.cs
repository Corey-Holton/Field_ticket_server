using CA.Ticketing.Business.Services.Base;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class UserDto : EntityDtoBase
    {   
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string TicketIdentifier { get; set; }

        public string Role { get; set; }
    }
}
