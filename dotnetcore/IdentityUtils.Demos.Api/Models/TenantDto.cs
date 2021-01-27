using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;

namespace IdentityUtils.Demos.Api.Models
{
    public class TenantDto : IIdentityManagerTenantDto
    {
        public ICollection<string> Hostnames { get; set; } = new List<string>();
        public Guid TenantId { get; set; }
    }
}