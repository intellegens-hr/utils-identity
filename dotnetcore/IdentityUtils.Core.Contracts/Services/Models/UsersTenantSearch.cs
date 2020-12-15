using System;

namespace IdentityUtils.Core.Contracts.Services.Models
{
    public class UsersTenantSearch : UsersSearch
    {
        public UsersTenantSearch(Guid? tenantId = null, Guid? roleId = null, string? username = null)
        {
            TenantId = tenantId;
            RoleId = roleId;
            Username = username;
        }

        public Guid? TenantId { get; set; }
    }
}