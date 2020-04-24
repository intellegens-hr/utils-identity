using Microsoft.EntityFrameworkCore;

namespace IdentityUtils.Demos.Google.IS4.Data
{
    public class ApplicationDbContextSqLite : ApplicationDbContext
    {
        public ApplicationDbContextSqLite() : base()
        {

        }

        public ApplicationDbContextSqLite(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite($@"Data Source=AspIdUsers.db;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}