using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;

namespace IdentityUtils.Api.Extensions.Cli.Models
{
    public class TenantDto : IIdentityManagerTenantDto
    {
        public ICollection<string> Hostnames { get; set; } = new List<string>();
        public string Name { get; set; }
        public Guid TenantId { get; set; }
    }
}