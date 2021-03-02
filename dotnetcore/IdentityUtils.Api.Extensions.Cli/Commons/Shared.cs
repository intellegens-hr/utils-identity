using IdentityUtils.Api.Extensions.Cli.Commands;
using IdentityUtils.Api.Extensions.Cli.Models;
using IdentityUtils.Api.Extensions.RestClients;
using McMaster.Extensions.CommandLineUtils;
using System;

namespace IdentityUtils.Api.Extensions.Cli.Commons
{
    internal static class Shared
    {
        private static ServicesConfiguration GetServicesConfiguration(IConsole console)
        {
            var authParamsResult = ServicesConfigurationLoader.GetServicesConfigurationCommandParams();
            authParamsResult.WriteMessages(console);

            if (authParamsResult.Data == null)
            {
                authParamsResult = ServicesConfigurationLoader.GetServicesConfigurationFromFiles();
                authParamsResult.WriteMessages(console);
            }

            if (authParamsResult.Data == null)
                Environment.Exit(-1);

            return authParamsResult.Data;
        }

        private static Is4ManagementRestClient GetRestClient(IApiExtensionsIs4Config is4Config)
            => new Is4ManagementRestClient(is4Config);

        internal static TenantManagementApi<TenantDto> GetTenantManagementApi(IConsole console)
        {
            var config = GetServicesConfiguration(console);

            if (!string.IsNullOrEmpty(Tenants.ApiBaseRoute))
                config.TenantManagementBaseRoute = Tenants.ApiBaseRoute;

            var client = GetRestClient(config);
            return new TenantManagementApi<TenantDto>(client, config);
        }

        internal static UserTenantManagementApi<UserDto> GetUserTenantManagementApi(IConsole console)
        {
            var config = GetServicesConfiguration(console);

            if (!string.IsNullOrEmpty(UsersTenant.ApiBaseRoute))
                config.UserManagementBaseRoute = UsersTenant.ApiBaseRoute;

            var client = GetRestClient(config);
            return new UserTenantManagementApi<UserDto>(client, config);
        }

        internal static UserManagementApi<UserDto> GetUserManagementApi(IConsole console)
        {
            var config = GetServicesConfiguration(console);

            if (!string.IsNullOrEmpty(UsersTenant.ApiBaseRoute))
                config.UserManagementBaseRoute = UsersTenant.ApiBaseRoute;

            var client = GetRestClient(config);
            return new UserManagementApi<UserDto>(client, config);
        }

        internal static RoleManagementApi<RoleDto> GetRoleManagementApi(IConsole console)
        {
            var config = GetServicesConfiguration(console);

            if (!string.IsNullOrEmpty(Roles.ApiBaseRoute))
                config.RoleManagementBaseRoute = Roles.ApiBaseRoute;

            var client = GetRestClient(config);
            return new RoleManagementApi<RoleDto>(client, config);
        }
    }
}