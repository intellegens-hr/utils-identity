using IdentityUtils.Api.Extensions;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Models;
using IdentityUtils.Demos.IdentityServer4.Tests.Setup;
using Intellegens.Tests.Commons.WebApps;
using Microsoft.Extensions.DependencyInjection;
using System;
using RoleDtoSingleTenant = IdentityUtils.Demos.IdentityServer4.SingleTenant.Models.RoleDto;
using UserDtoSingleTenant = IdentityUtils.Demos.IdentityServer4.SingleTenant.Models.UserDto;

namespace IdentityUtils.Demos.IdentityServer4.Tests
{
    public abstract class TestAbstract<TStartup, TStartupBase, TFactory> : ControllerIntegrationTestBase<TStartup, TStartupBase, TFactory>
        where TStartup : class, ITestStartup
        where TStartupBase : class
        where TFactory : CustomWebApplicationFactoryBase<TStartup, TStartupBase>
    {
        public TestAbstract(
            CustomWebApplicationFactoryBase<TStartup, TStartupBase> factory,
            string solutionRelativeDirectory = "IdentityUtils.Demos.IdentityServer4.MultiTenant")
            : base(factory, solutionRelativeDirectory: solutionRelativeDirectory)
        {
        }

        private ServiceProvider ServiceProvider
            => TestServicesFactory.ServiceProvider(GetHttpClient(GetAccessTokenFromSecret().Result));

        protected virtual string DatabaseName => "IntegrationTestDatabase.db";

        protected RoleManagementApi<RoleDto> RoleManagementApi
            => ServiceProvider.GetRequiredService<RoleManagementApi<RoleDto>>();

        protected RoleManagementApi<RoleDtoSingleTenant> RoleManagementApiSingleTenant
            => ServiceProvider.GetRequiredService<RoleManagementApi<RoleDtoSingleTenant>>();

        protected TenantManagementApi<TenantDto> TenantManagementApi
            => ServiceProvider.GetRequiredService<TenantManagementApi<TenantDto>>();

        protected UserManagementApi<UserDtoSingleTenant> UserManagementApi
            => ServiceProvider.GetRequiredService<UserManagementApi<UserDtoSingleTenant>>();

        protected UserTenantManagementApi<UserDto> UserTenantManagementApi
            => ServiceProvider.GetRequiredService<UserTenantManagementApi<UserDto>>();

        protected override void OnPreInit()
        {
            Environment.SetEnvironmentVariable("Is4Host", Factory.BaseUrl);
            Environment.SetEnvironmentVariable("DatabaseName", DatabaseName);

            DefaultClientId = "is4management";
            DefaultClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A";
            DefaultScope = "demo-is4-management-api";

            base.OnPreInit();
        }
    }
}