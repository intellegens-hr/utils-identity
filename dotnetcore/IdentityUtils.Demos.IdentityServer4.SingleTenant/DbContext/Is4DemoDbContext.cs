using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Demos.IdentityServer4.SingleTenant.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityUtils.Demos.IdentityServer4.SingleTenant.DbContext
{
    public class Is4DemoDbContext : IdentityManagerDbContext<IdentityManagerUser, IdentityManagerRole>
    {
        private readonly DbConfig dbConfig;

        public Is4DemoDbContext(DbContextOptions options, DbConfig dbConfig) : base()
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