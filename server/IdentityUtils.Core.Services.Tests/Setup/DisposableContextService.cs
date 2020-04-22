using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityUtils.Core.Services.Tests.Setup
{
    /// <summary>
    /// SQLite in memory provider keeps schema and data as long the database connection is open. 
    /// This can be challenging when using DI and using multiple services.
    /// This class is used to fetch required services and keep connection open until disposal.
    /// </summary>
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

            servicesCollection.AddIdentity<UserDb, RoleDb>()
             .AddEntityFrameworkStores<TestDbContext>()
             .AddDefaultTokenProviders();

            servicesCollection
                .AddLogging()
                .AddAutoMapper(typeof(MapperProfile))
                .AddScoped<IIdentityManagerTenantContext<TenantDb>>(x => TestDbContext)
                .AddScoped<IIdentityManagerUserContext<UserDb>>(x => TestDbContext)
                .AddScoped<IdentityManagerDbContext<UserDb, RoleDb, TenantDb>>(x => TestDbContext)
                .AddScoped(x => TestDbContext)
                .AddScoped<RolesService>()
                .AddScoped<TenantsService>()
                .AddScoped<UsersService>();

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