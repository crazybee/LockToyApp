using ToyContracts;

namespace LockToyApp.Models
{
    public class AuthenticationResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string Token { get; set; }
        public AuthenticationResponse(User user, string token)
        {
            UserId = user.Id;

            UserName = user.UserName;

            Token = token;

        }
    }
}
