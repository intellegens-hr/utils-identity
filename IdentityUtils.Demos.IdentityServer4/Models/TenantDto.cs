using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;

namespace IdentityUtils.Demos.IdentityServer4.Models
{
    public class TenantDto : IIdentityManagerTenantDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public List<string> Hostnames { get; set; }
    }
}