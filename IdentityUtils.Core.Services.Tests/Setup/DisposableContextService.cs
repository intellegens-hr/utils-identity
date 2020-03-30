using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityUtils.Core.Services.Tests.Setup
{
    internal class DisposableContextService : IDisposable
    {
        private readonly ServiceProvider serviceProvider;

        private TestDbContext TestDbContext { get; }

        internal TService GetService<TService>()
            => serviceProvider.GetRequiredService<TService>();

        public DisposableContextService()
        {
            TestDbContext = new TestDbContext();

            var servicesCollection = new ServiceCollection();

            servicesCollection.AddIdentity<IdentityManagerUser, IdentityManagerRole>()
             .AddEntityFrameworkStores<TestDbContext>();

            servicesCollection
                .AddLogging()
                .AddAutoMapper(typeof(MapperProfile))
                .AddScoped<IIdentityManagerTenantContext<IdentityManagerTenant>>(x => TestDbContext)
                .AddScoped<IIdentityManagerUserContext<IdentityManagerUser>, TestDbContext>()
                .AddScoped<TestDbContext>(x => TestDbContext)
                .AddScoped<RolesService>()
                .AddScoped<TenantsService>()
                .AddScoped<IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole>>();

            serviceProvider = servicesCollection.BuildServiceProvider();

            TestDbContext.Database.OpenConnection();
            TestDbContext.Database.Migrate();
        }

        public void Dispose()
        {
            TestDbContext.Database.CloseConnection();
        }
    }
}