using IdentityUtils.Core.Contracts.Roles;
using System;

namespace IdentityUtils.Demos.IdentityServer4.Models
{
    public class RoleDto : IIdentityManagerRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}