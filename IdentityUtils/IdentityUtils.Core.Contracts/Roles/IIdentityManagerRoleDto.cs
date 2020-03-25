using System;

namespace IdentityUtils.Core.Contracts.Roles
{
    /// <summary>
    /// Properties that all role domain transfer objects need to implement.
    /// </summary>
    public interface IIdentityManagerRoleDto
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string NormalizedName { get; set; }
    }
}