using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;

namespace IdentityUtils.Core.Services.Tests.Setup.DtoModels
{
    public class TenantDto : IIdentityManagerTenantDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public List<string> Hostnames { get; set; }
    }
}