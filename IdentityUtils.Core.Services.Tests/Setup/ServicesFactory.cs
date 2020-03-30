using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityUtils.Core.Services.Tests.Setup
{
    internal static class ServicesFactory
    {
        private static readonly ServiceProvider services;

        static ServicesFactory()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddScoped<TestDbContext>();

            servicesCollection.AddIdentity<IdentityManagerUser, IdentityManagerRole>()
             .AddEntityFrameworkStores<TestDbContext>();

            servicesCollection
                .AddLogging()
                .AddAutoMapper(typeof(MapperProfile))
                .AddScoped<IIdentityManagerTenantContext<IdentityManagerTenant>, TestDbContext>()
                .AddScoped<IIdentityManagerUserContext<IdentityManagerUser>, TestDbContext>()
                .AddScoped<IdentityManagerRolesService<IdentityManagerUser, IdentityManagerRole, RoleDto>>()
                .AddScoped<TenantService>()
                .AddScoped<IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole>>();

            services = servicesCollection.BuildServiceProvider();
        }

        internal static IdentityManagerRolesService<IdentityManagerUser, IdentityManagerRole, RoleDto> GetRolesService()
        {
            //var context = services.GetRequiredService<TestDbContext>();
            //var a = context.Tenants.ToListAsync().Result;
            //context.Database.Migrate();
            //a = context.Tenants.ToListAsync().Result;

            return services.GetRequiredService<IdentityManagerRolesService<IdentityManagerUser, IdentityManagerRole, RoleDto>>();
        }

        internal static DisposableContextService<TenantService> GetTenantService()
        {
            var mapper = services.GetRequiredService<IMapper>();

            TenantService TenantServiceBuilder(TestDbContext dbContext)
            {
                return new TenantService(dbContext, mapper);
            }

            return new DisposableContextService<TenantService>(TenantServiceBuilder);
        }

        internal static IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole> UserService
            => services.GetRequiredService<IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole>>();
    }
}