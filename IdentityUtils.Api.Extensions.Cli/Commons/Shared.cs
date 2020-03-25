using IdentityManagement.Models.ModelsDto;
using IdentityUtils.Api.Extensions.Cli.Models;
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

        internal static TenantManagementApi<TenantDto> GetTenantManagementApi(IConsole console)
            => new TenantManagementApi<TenantDto>(GetServicesConfiguration(console));

        internal static UserManagementApi<UserDto> GetUserManagementApi(IConsole console)
            => new UserManagementApi<UserDto>(GetServicesConfiguration(console));

        internal static RoleManagementApi<RoleDto> GetRoleManagementApi(IConsole console)
            => new RoleManagementApi<RoleDto>(GetServicesConfiguration(console));
    }
}