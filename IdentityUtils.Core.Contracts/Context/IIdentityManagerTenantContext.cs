using IdentityUtils.Core.Contracts.Tenants;
using Microsoft.EntityFrameworkCore;

namespace IdentityUtils.Core.Contracts.Context
{
    /// <summary>
    /// Tenant management context
    /// </summary>
    /// <typeparam name="TTenant">Tenant type which inherits IdentityManagerTenant</typeparam>
    public interface IIdentityManagerTenantContext<TTenant> : IDbContextCommon
        where TTenant : IdentityManagerTenant
    {
        DbSet<TTenant> Tenants { get; set; }
    }
}