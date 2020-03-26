using IdentityUtils.Api.Extensions.RestClients;

namespace IdentityUtils.Api.Extensions.Cli
{
    public class ServicesConfiguration : IApiExtensionsIs4Config, IApiExtensionsConfig
    {
        public string Hostname { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ClientScope { get; set; }

        public string UserManagementBaseRoute { get; set; } = "/api/management/users";

        public string RoleManagementBaseRoute { get; set; } = "/api/management/roles";

        public string TenantManagementBaseRoute { get; set; } = "/api/management/tenants";
    }
}