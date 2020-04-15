using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityUtils.Core.Contracts.Claims
{
    public class TenantRolesClaimData : IEquatable<TenantRolesClaimData>
    {
        public TenantRolesClaimData()
        {
        }

        public TenantRolesClaimData(Guid tenantId, string role) : this(tenantId, new List<string> { role })
        {
        }

        public TenantRolesClaimData(Guid tenantId, List<string> roles)
        {
            TenantId = tenantId;
            Roles = roles;
        }

        public Guid TenantId { get; set; }

        public List<string> Roles { get; set; }

        public bool Equals(TenantRolesClaimData other)
        {
            return TenantId == other.TenantId
                && Enumerable.SequenceEqual(Roles, other.Roles);
        }
    }
}