using Microsoft.EntityFrameworkCore;
using System;

namespace IdentityUtils.Demos.Google.IS4.Data
{
    public class ApplicationDbContextPostgres : ApplicationDbContext
    {
        public ApplicationDbContextPostgres(): base()
        {

        }

        public ApplicationDbContextPostgres(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql("Host=dumbo.db.elephantsql.com;Database=rlkmzevj;Username=rlkmzevj;Password=i0t5czAMpUtUCYKQUFVT0GJAWgZMvZjL",
                options => options.SetPostgresVersion(new Version(9, 6)));

            base.OnConfiguring(optionsBuilder);
        }
    }
}