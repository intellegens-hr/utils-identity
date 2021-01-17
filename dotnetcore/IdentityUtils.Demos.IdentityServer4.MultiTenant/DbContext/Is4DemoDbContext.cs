using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Threading.Tasks;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant.DbContext
{
    public class Is4DemoDbContext : IdentityManagerTenantDbContext<IdentityManagerUser, IdentityManagerRole, IdentityManagerTenant>, IPersistedGrantDbContext
    {
        private readonly DbConfig dbConfig;

        public Is4DemoDbContext(DbConfig dbConfig) : base()
        {
            this.dbConfig = dbConfig;
        }

        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }
        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite($@"Data Source={dbConfig.DatabaseName};");

            optionsBuilder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var options = new OperationalStoreOptions
            {
                DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
                PersistedGrants = new TableConfiguration("PersistedGrants")
            };

            base.OnModelCreating(builder);
            builder.ConfigurePersistedGrantContext(options);
        }
    }
}