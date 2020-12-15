using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant.DbContext
{
    public class Is4DemoDbContext : IdentityManagerTenantDbContext<IdentityManagerUser, IdentityManagerRole, IdentityManagerTenant>
    {
        private readonly DbConfig dbConfig;

        public Is4DemoDbContext(DbContextOptions options, DbConfig dbConfig) : base(options)
        {
            this.dbConfig = dbConfig;
        }

        protected Is4DemoDbContext(DbConfig dbConfig): base()
        {
            this.dbConfig = dbConfig;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite($@"Data Source={dbConfig.DatabaseName};");

            optionsBuilder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
        }
    }
}