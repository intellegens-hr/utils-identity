using System.Collections.Generic;

namespace IdentityUtils.Api.Models.Authentication
{
    public interface IUserProfile
    {
        public IEnumerable<Claim> Claims { get; set; }
    }
}