using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityUtils.Core.Services.Tests.Setup
{
    internal static class ServicesFactory
    {
        private static readonly ServiceProvider services;

        static ServicesFactory()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddDbContext<TestDbContext>();

            servicesCollection.AddIdentity<IdentityManagerUser, IdentityManagerRole>()
             .AddEntityFrameworkStores<TestDbContext>();

            servicesCollection
                .AddLogging()
                .AddAutoMapper(typeof(MapperProfile))
                .AddTransient<IIdentityManagerTenantContext<IdentityManagerTenant>, TestDbContext>()
                .AddTransient<IIdentityManagerUserContext<IdentityManagerUser>, TestDbContext>()
                .AddTransient<IdentityManagerRolesService<IdentityManagerUser, IdentityManagerRole, RoleDto>>()
                .AddTransient<IdentityManagerTenantService<IdentityManagerTenant, TenantDto>>()
                .AddTransient<IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole>>();

            services = servicesCollection.BuildServiceProvider();
        }

        internal static IdentityManagerRolesService<IdentityManagerUser, IdentityManagerRole, RoleDto> RolesService
            => services.GetRequiredService<IdentityManagerRolesService<IdentityManagerUser, IdentityManagerRole, RoleDto>>();

        internal static IdentityManagerTenantService<IdentityManagerTenant, TenantDto> TenantService
            => services.GetRequiredService<IdentityManagerTenantService<IdentityManagerTenant, TenantDto>>();

        internal static IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole> UserService
            => services.GetRequiredService<IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole>>();
    }
}