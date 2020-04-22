using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Services.Tests.Setup;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityUtils.Core.Services.Tests
{
    public abstract class TestAbstract: IDisposable
    {
        protected string GetUniqueId() => Guid.NewGuid().ToString().Replace(" - ", "");
       // protected readonly DisposableContextService serviceProviderDisposable = new DisposableContextService();

        private readonly ServiceProvider serviceProvider;
        private readonly TestDbContext testDbContext;

        protected TestAbstract()
        {
            testDbContext = new TestDbContext();

            testDbContext.Database.OpenConnection();
            testDbContext.Database.Migrate();

            var servicesCollection = new ServiceCollection();

            servicesCollection.AddIdentity<UserDb, RoleDb>()
             .AddEntityFrameworkStores<TestDbContext>()
             .AddDefaultTokenProviders();

            servicesCollection
                .AddLogging()
                .AddAutoMapper(typeof(MapperProfile))
                .AddScoped<IIdentityManagerTenantContext<TenantDb>>(x => testDbContext)
                .AddScoped<IIdentityManagerUserContext<UserDb>>(x => testDbContext)
                .AddScoped<IdentityManagerDbContext<UserDb, RoleDb, TenantDb>>(x => testDbContext)
                .AddScoped(x => testDbContext)
                .AddScoped<RolesService>()
                .AddScoped<TenantsService>()
                .AddScoped<UsersService>();

            serviceProvider = servicesCollection.BuildServiceProvider();

            testDbContext.Database.OpenConnection();
        }

        internal TService GetService<TService>()
           => serviceProvider.GetRequiredService<TService>();

        public void Dispose()
        {
            testDbContext.Database.CloseConnection();
        }
    }
}