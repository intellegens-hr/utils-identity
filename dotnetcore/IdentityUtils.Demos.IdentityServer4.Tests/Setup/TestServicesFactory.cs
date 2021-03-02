using IdentityUtils.Api.Extensions;
using IdentityUtils.Api.Extensions.RestClients;
using IdentityUtils.Commons;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Models;
using IdentityUtils.Demos.IdentityServer4.Tests.Setup.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using RoleDtoMultitenant = IdentityUtils.Demos.IdentityServer4.MultiTenant.Models.RoleDto;
using RoleDtoSingletenant = IdentityUtils.Demos.IdentityServer4.SingleTenant.Models.RoleDto;
using UserDtoMultitenant = IdentityUtils.Demos.IdentityServer4.MultiTenant.Models.UserDto;
using UserDtoSingletenant = IdentityUtils.Demos.IdentityServer4.SingleTenant.Models.UserDto;

namespace IdentityUtils.Demos.IdentityServer4.Tests.Setup
{
    internal static class TestServicesFactory
    {
        internal static ServiceProvider ServiceProvider(HttpClient httpClient)
        {
            var services = new ServiceCollection();
            var client = new RestClient(httpClient);

            services.AddSingleton<IApiExtensionsIs4Config, ApiExtensionsIs4Config>();
            services.AddSingleton<IApiExtensionsConfig, ApiExtensionsConfig>();

            var config = new ApiExtensionsConfig();

            services.AddScoped<RestClient, Is4ManagementRestClient>();

            services.AddScoped(b => new RoleManagementApi<RoleDtoMultitenant>(client, config));
            services.AddScoped(b => new TenantManagementApi<TenantDto>(client, config));
            services.AddScoped(b => new UserTenantManagementApi<UserDtoMultitenant>(client, config));

            services.AddScoped(b => new RoleManagementApi<RoleDtoSingletenant>(client, config));
            services.AddScoped(b => new UserManagementApi<UserDtoSingletenant>(client, config));

            return services.BuildServiceProvider();
        }
    }
}