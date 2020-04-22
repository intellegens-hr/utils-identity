using IdentityUtils.Core.Contracts.Roles;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Core.Services.Tests.Setup.DbModels
{
    public class RoleDb : IdentityManagerRole
    {
        [StringLength(50)]
        public override string Name { get => base.Name; set => base.Name = value; }
        [StringLength(50)]
        public override string NormalizedName { get => base.NormalizedName; set => base.NormalizedName = value; }
    }
}