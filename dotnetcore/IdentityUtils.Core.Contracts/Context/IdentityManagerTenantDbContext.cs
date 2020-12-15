using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdentityUtils.Core.Contracts.Context
{
    /// <summary>
    /// Multitenant user management context.
    /// Custom IdentityManagement types are used instead of IdentityRole<T> or IdentityUser<T>
    /// since all primary keys are specified as GUID. This removes necessity to specify IdentityRole<Guid>
    /// on n places.
    /// </summary>
    /// <typeparam name="TUser">User type which inherits IdentityManagerUser</typeparam>
    /// <typeparam name="TRole">Role type which inherits IdentityManagerRole</typeparam>
    /// <typeparam name="TTenant">Tenant type which inherits IdentityManagerTenant</typeparam>
    public class IdentityManagerTenantDbContext<TUser, TRole, TTenant>
        : IdentityDbContext<TUser, TRole, Guid>,
          IIdentityManagerUserContext<TUser>,
          IIdentityManagerTenantContext<TTenant>
        where TUser : IdentityManagerUser
        where TRole : IdentityManagerRole
        where TTenant : IdentityManagerTenant
    {
        public IdentityManagerTenantDbContext(DbContextOptions options) : base(options)
        {
        }

        protected IdentityManagerTenantDbContext()
        {
        }

        public virtual DbSet<TTenant> Tenants { get; set; }
        public virtual DbSet<IdentityManagerTenantHost> TenantHosts { get; set; }
    }
}