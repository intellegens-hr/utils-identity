using Microsoft.Extensions.Configuration;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant.Configuration
{
    public class DbConfig
    {
        public DbConfig(IConfiguration configuration)
        {
            DatabaseName = configuration["DatabaseName"];
        }

        public string DatabaseName { get; set; }
    }
}