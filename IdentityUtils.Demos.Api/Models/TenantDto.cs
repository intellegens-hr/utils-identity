using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;

namespace IdentityUtils.Demos.Api.Models
{
    public class TenantDto : IIdentityManagerTenantDto
    {
        public Guid TenantId { get; set; }

        public List<string> Hostnames { get; set; }
    }
}