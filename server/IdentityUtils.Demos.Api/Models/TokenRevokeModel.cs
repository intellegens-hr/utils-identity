namespace IdentityUtils.Demos.Api.Models
{
    public class TokenRevokeModel
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}