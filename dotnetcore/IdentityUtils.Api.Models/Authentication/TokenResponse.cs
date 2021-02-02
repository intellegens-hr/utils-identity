using System.Collections.Generic;

namespace IdentityUtils.Api.Models.Authentication
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public int Lifetime { get; set; }
        public string RefreshToken { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }
}