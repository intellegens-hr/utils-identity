using IdentityUtils.Core.Contracts.Users;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Core.Services.Tests.Setup.DbModels
{
    public class UserDb : IdentityManagerUser
    {
        [StringLength(128)]
        public string DisplayName { get; set; }
    }
}