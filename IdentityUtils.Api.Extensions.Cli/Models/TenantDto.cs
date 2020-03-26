using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;

namespace IdentityUtils.Api.Extensions.Cli.Models
{
    public class TenantDto : IIdentityManagerTenantDto
    {
        public string Name { get; set; }

        public List<string> Hostnames { get; set; } = new List<string>();

        public Guid TenantId { get; set; }
    }
}