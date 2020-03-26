using IdentityUtils.Core.Contracts.Tenants;
using System;

namespace IdentityUtils.Demos.IdentityServer4.Models
{
    public class TenantDto : IIdentityManagerTenantDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string Hostname { get; set; }
    }
}