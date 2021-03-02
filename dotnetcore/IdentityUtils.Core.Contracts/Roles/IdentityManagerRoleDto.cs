using System;

namespace IdentityUtils.Core.Contracts.Roles
{
    public class IdentityManagerRoleDto : IIdentityManagerRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}