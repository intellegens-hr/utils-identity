using IdentityUtils.Core.Contracts.Tenants;
using System;

namespace IdentityUtils.Api.Extensions.Cli.Models
{
    public class TenantDto : IIdentityManagerTenantDto
    {
        public string Name { get; set; }

        public string Hostname { get; set; }

        public Guid TenantId { get; set; }
    }
}