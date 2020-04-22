using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityUtils.Core.Services.Tests.Setup
{
    public class TestDbContext :
        IdentityManagerDbContext<UserDb, RoleDb, TenantDb>
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        public TestDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=:memory:");

            optionsBuilder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserDb>().Property(x => x.DisplayName).HasMaxLength(128);
            builder.Entity<UserDb>().Property(x => x.Email).HasMaxLength(128);

            builder.Entity<TenantDb>().Property(x => x.Name).HasMaxLength(32);

            builder.Entity<RoleDb>().Property(x => x.Name).HasMaxLength(32);
        }
    }
}