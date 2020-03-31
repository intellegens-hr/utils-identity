using IdentityUtils.Api.Extensions;
using IdentityUtils.Api.Extensions.RestClients;
using IdentityUtils.Commons;
using IdentityUtils.Demos.IdentityServer4.Models;
using IdentityUtils.Demos.IdentityServer4.Tests.Setup.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace IdentityUtils.Demos.IdentityServer4.Tests.Setup
{
    internal static class ServicesFactory
    {
        internal static ServiceProvider ServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IApiExtensionsIs4Config, ApiExtensionsIs4Config>();
            services.AddSingleton<IApiExtensionsConfig, ApiExtensionsConfig>();

            services.AddScoped<RestClient, Is4ManagementRestClient>();

            services.AddScoped<RoleManagementApi<RoleDto>>();
            services.AddScoped<TenantManagementApi<TenantDto>>();
            services.AddScoped<UserManagementApi<UserDto>>();

            return services.BuildServiceProvider();
        }
    }
}