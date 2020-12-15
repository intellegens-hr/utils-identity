using IdentityUtils.Api.Extensions;
using IdentityUtils.Api.Extensions.RestClients;
using IdentityUtils.Commons;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Models;
using IdentityUtils.Demos.IdentityServer4.Tests.Setup.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RoleDtoMultitenant = IdentityUtils.Demos.IdentityServer4.MultiTenant.Models.RoleDto;
using UserDtoMultitenant = IdentityUtils.Demos.IdentityServer4.MultiTenant.Models.UserDto;
using RoleDtoSingletenant = IdentityUtils.Demos.IdentityServer4.SingleTenant.Models.RoleDto;
using UserDtoSingletenant = IdentityUtils.Demos.IdentityServer4.SingleTenant.Models.UserDto;

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

            services.AddScoped<RoleManagementApi<RoleDtoMultitenant>>();
            services.AddScoped<TenantManagementApi<TenantDto>>();
            services.AddScoped<UserTenantManagementApi<UserDtoMultitenant>>();

            services.AddScoped<RoleManagementApi<RoleDtoSingletenant>>();
            services.AddScoped<UserManagementApi<UserDtoSingletenant>>();

            return services.BuildServiceProvider();
        }
    }
}