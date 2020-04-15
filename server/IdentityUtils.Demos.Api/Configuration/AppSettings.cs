using Microsoft.Extensions.Configuration;

namespace IdentityUtils.Demos.Api.Configuration
{
    public class AppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private string GetConfigValue(string key)
            => configuration[key].ToString();

        public string Is4Host => GetConfigValue("Is4Host");
        public string ApiAuthenticationAudience => GetConfigValue("ApiAuthenticationAudience");
        public string Is4JsClientId => GetConfigValue("Is4JsClientId");

        public string Is4ManagementApiClientId => GetConfigValue("Is4ManagementApiClientId");
        public string Is4ManagementApiClientSecret => GetConfigValue("Is4ManagementApiClientSecret");
        public string Is4ManagementApiClientScope => GetConfigValue("Is4ManagementApiClientScope");

        public string UserManagementBaseRoute => GetConfigValue("UserManagementBaseRoute");
        public string RoleManagementBaseRoute => GetConfigValue("RoleManagementBaseRoute");
        public string TenantManagementBaseRoute => GetConfigValue("TenantManagementBaseRoute");
    }
}