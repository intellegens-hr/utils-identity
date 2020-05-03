using IdentityUtils.Api.Extensions;
using IdentityUtils.Api.Extensions.RestClients;

namespace IdentityUtils.Demos.Api.Configuration
{
    public class ApiExtensionsConfig : IApiExtensionsIs4Config, IApiExtensionsConfig
    {
        private readonly AppSettings appSettings;

        public ApiExtensionsConfig(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public string Hostname => appSettings.Is4Host;

        public string ClientId => appSettings.Is4ManagementApiClientId;

        public string ClientSecret => appSettings.Is4ManagementApiClientSecret;

        public string ClientScope => appSettings.Is4ManagementApiClientScope;

        public string UserManagementBaseRoute => appSettings.UserManagementBaseRoute;

        public string RoleManagementBaseRoute => appSettings.RoleManagementBaseRoute;

        public string TenantManagementBaseRoute => appSettings.TenantManagementBaseRoute;
    }
}