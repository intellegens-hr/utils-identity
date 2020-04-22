using IdentityUtils.Core.Contracts.Tenants;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Core.Services.Tests.Setup.DbModels
{
    public class TenantDb : IdentityManagerTenant
    {
        [StringLength(32)]
        public override string Name { get; set; }
    }
}