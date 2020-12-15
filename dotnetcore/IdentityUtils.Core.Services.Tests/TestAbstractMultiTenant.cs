using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Services.Tests.Setup;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityUtils.Core.Services.Tests
{
    public abstract class TestAbstractMultiTenant : TestAbstract<TestTenantDbContext>
    {
        protected TestAbstractMultiTenant() : base(new TestTenantDbContext())
        {
            ServiceCollection
                .AddScoped<IIdentityManagerTenantContext<TenantDb>>(x => DbContext)
                .AddScoped<IIdentityManagerUserContext<UserDb>>(x => DbContext)
                .AddScoped<IdentityManagerTenantDbContext<UserDb, RoleDb, TenantDb>>(x => DbContext);

            Initialize();
        }
    }
}