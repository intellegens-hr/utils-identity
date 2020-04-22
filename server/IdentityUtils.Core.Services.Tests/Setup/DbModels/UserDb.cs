using IdentityUtils.Core.Contracts.Users;

namespace IdentityUtils.Core.Services.Tests.Setup.DbModels
{
    public class UserDb : IdentityManagerUser
    {
        public string DisplayName { get; set; }
    }
}