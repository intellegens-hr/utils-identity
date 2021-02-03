using System.Collections.Generic;

namespace IdentityUtils.Api.Models.Authentication
{
    public class UserProfile : IUserProfile
    {
        public IEnumerable<Claim> Claims { get; set; }
    }
}