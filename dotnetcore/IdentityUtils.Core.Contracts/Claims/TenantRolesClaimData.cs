using IdentityUtils.Core.Contracts.Services.Models;
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

        public TenantRolesClaimData(Guid tenantId)
        {
            TenantId = tenantId;
        }

        public TenantRolesClaimData(Guid tenantId, RoleBasicData role) : this(tenantId, new List<RoleBasicData> { role })
        {
        }

        public TenantRolesClaimData(Guid tenantId, List<RoleBasicData> roles)
        {
            TenantId = tenantId;
            Roles = roles;
        }

        public IEnumerable<RoleBasicData> Roles { get; set; } = Enumerable.Empty<RoleBasicData>();

        public Guid TenantId { get; set; }

        public bool Equals(TenantRolesClaimData other)
        {
            return TenantId == other.TenantId
                && Enumerable.SequenceEqual(Roles, other.Roles);
        }
    }
}