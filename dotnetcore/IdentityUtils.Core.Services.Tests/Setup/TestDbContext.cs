using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityUtils.Core.Services.Tests.Setup
{
    public class TestDbContext :
        IdentityManagerDbContext<UserDb, RoleDb>
    {
        public TestDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=:memory:;foreign keys=true;");

            optionsBuilder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RoleDb>().Property(x => x.Name).HasMaxLength(50);
            builder.Entity<RoleDb>().Property(x => x.NormalizedName).HasMaxLength(50);
        }
    }
}