using Newtonsoft.Json;

namespace CA.Ticketing.Business.Services.Authentication.Dto
{
    public class AuthenticationResultDto
    {
        public bool IsSuccess { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? ErrorMessage { get; set; }

        public AuthenticatedUser User { get; set; }

        public AuthenticationResultDto()
        {
            ErrorMessage = "Bad username or password";
        }

        public AuthenticationResultDto(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public AuthenticationResultDto(AuthenticatedUser user)
        {
            User = user;
            IsSuccess = true;
        }
    }
}
