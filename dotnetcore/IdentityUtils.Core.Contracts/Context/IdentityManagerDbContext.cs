using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;

namespace IdentityUtils.Core.Contracts.Context
{
    /// <summary>
    /// Single tenant user management context.
    /// </summary>
    /// <typeparam name="TUser">User type which inherits IdentityManagerUser</typeparam>
    /// <typeparam name="TRole">Role type which inherits IdentityManagerRole</typeparam>
    public class IdentityManagerDbContext<TUser, TRole>
        : IdentityManagerTenantDbContext<TUser, TRole, IdentityManagerTenant>
        where TUser : IdentityManagerUser
        where TRole : IdentityManagerRole
    {
    }
}