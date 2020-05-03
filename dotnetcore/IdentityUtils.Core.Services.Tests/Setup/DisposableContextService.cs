//using AutoMapper;
//using IdentityUtils.Core.Contracts.Context;
//using IdentityUtils.Core.Services.Tests.Setup.DbModels;
//using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
//using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.IO;

//namespace IdentityUtils.Core.Services.Tests.Setup
//{
//    /// <summary>
//    /// SQLite in memory provider keeps schema and data as long the database connection is open.
//    /// This can be challenging when using DI and using multiple services.
//    /// This class is used to fetch required services and keep connection open until disposal.
//    /// </summary>
//    public class DisposableContextService : IDisposable
//    {
//        private readonly ServiceProvider serviceProvider;

//        private static bool databaseInitialized = false;
//        private TestDbContext TestDbContext { get; set; } = new TestDbContext();

//        internal TService GetService<TService>()
//            => serviceProvider.GetRequiredService<TService>();

//        public DisposableContextService()
//        {
//            lock (TestDbContext)
//            {
//                if (!databaseInitialized) {
//                    File.WriteAllText("testdb.db", "");
//                    TestDbContext.Database.Migrate();
//                    databaseInitialized = true;
//                }
//            }

//            var servicesCollection = new ServiceCollection();

//            servicesCollection.AddIdentity<UserDb, RoleDb>()
//             .AddEntityFrameworkStores<TestDbContext>()
//             .AddDefaultTokenProviders();

//            servicesCollection
//                .AddLogging()
//                .AddAutoMapper(typeof(MapperProfile))
//                .AddScoped<IIdentityManagerTenantContext<TenantDb>>(x => TestDbContext)
//                .AddScoped<IIdentityManagerUserContext<UserDb>>(x => TestDbContext)
//                .AddScoped<IdentityManagerDbContext<UserDb, RoleDb, TenantDb>>(x => TestDbContext)
//                .AddScoped(x => TestDbContext)
//                .AddScoped<RolesService>()
//                .AddScoped<TenantsService>()
//                .AddScoped<UsersService>();

//            serviceProvider = servicesCollection.BuildServiceProvider();

//            //TestDbContext.Database.OpenConnection();
//        }

//        public void Dispose()
//        {
//            //TestDbContext.Database.CloseConnection();
//        }
//    }
//}