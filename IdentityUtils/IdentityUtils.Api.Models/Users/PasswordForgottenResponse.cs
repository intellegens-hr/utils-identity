namespace IdentityUtils.Api.Models.Users
{
    public class PasswordForgottenResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}