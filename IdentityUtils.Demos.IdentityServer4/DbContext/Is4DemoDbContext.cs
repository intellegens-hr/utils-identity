using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityUtils.Demos.IdentityServer4.DbContext
{
    public class Is4DemoDbContext : IdentityManagerDbContext<IdentityManagerUser, IdentityManagerRole, IdentityManagerTenant>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=Is4Demo.db;");

            optionsBuilder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
        }
    }
}