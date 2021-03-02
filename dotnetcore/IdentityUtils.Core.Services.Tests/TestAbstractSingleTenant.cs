using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Services.Tests.Setup;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityUtils.Core.Services.Tests
{
    public abstract class TestAbstractSingleTenant : TestAbstract<TestDbContext>
    {
        protected TestAbstractSingleTenant() : base(new TestDbContext())
        {
            ServiceCollection
                .AddScoped<IIdentityManagerUserContext<UserDb>>(x => DbContext)
                .AddScoped<IdentityManagerDbContext<UserDb, RoleDb>>(x => DbContext);

            Initialize();
        }
    }
}