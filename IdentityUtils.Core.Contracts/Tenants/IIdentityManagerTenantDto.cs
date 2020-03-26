using System;
using System.Collections.Generic;

namespace IdentityUtils.Core.Contracts.Tenants
{
    /// <summary>
    /// Properties that all tenant domain transfer objects need to implement.
    /// </summary>
    public interface IIdentityManagerTenantDto
    {
        public Guid TenantId { get; }
        public List<string> Hostnames { get; set; }
    }
}