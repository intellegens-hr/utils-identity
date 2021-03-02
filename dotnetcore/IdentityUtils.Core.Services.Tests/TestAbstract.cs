using AutoMapper;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityUtils.Core.Services.Tests
{
    public abstract class TestAbstract<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        private ServiceProvider serviceProvider;

        protected TestAbstract(TDbContext dbContext)
        {
            DbContext = dbContext;

            ServiceCollection.AddIdentity<UserDb, RoleDb>()
             .AddEntityFrameworkStores<TDbContext>()
             .AddDefaultTokenProviders();

            ServiceCollection
                .AddLogging()
                .AddAutoMapper(typeof(MapperProfile))
                .AddScoped(x => DbContext)
                .AddScoped<RolesService>()
                .AddScoped<TenantsService>()
                .AddScoped<UsersTenantService>()
                .AddScoped<UsersService>();
        }

        protected TDbContext DbContext { get; set; }

        protected ServiceCollection ServiceCollection { get; } = new ServiceCollection();

        public void Dispose()
        {
            DbContext.Database.CloseConnection();
        }

        internal TService GetService<TService>()
           => serviceProvider.GetRequiredService<TService>();

        protected void BuildServiceProvider()
        {
            serviceProvider = ServiceCollection.BuildServiceProvider();
            DbContext.Database.OpenConnection();
        }

        protected string GetUniqueId() => Guid.NewGuid().ToString().Replace(" - ", "");

        protected void Initialize()
        {
            serviceProvider = ServiceCollection.BuildServiceProvider();

            DbContext.Database.OpenConnection();
            DbContext.Database.Migrate();
        }
    }
}