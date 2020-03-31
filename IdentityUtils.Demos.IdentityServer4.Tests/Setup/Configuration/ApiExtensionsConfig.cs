using IdentityUtils.Api.Extensions;
using System;

namespace IdentityUtils.Demos.IdentityServer4.Tests.Setup.Configuration
{
    public class ApiExtensionsConfig : IApiExtensionsConfig
    {
        public string Hostname => Environment.GetEnvironmentVariable("Is4Host");

        public string UserManagementBaseRoute => "/api/management/users";

        public string RoleManagementBaseRoute => "/api/management/roles";

        public string TenantManagementBaseRoute => "/api/management/tenants";
    }
}