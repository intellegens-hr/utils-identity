using IdentityUtils.Core.Contracts.Roles;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Core.Services.Tests.Setup.DbModels
{
    public class RoleDb : IdentityManagerRole
    {
        [StringLength(32)]
        public override string Name { get; set; }
    }
}