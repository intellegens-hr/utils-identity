using IdentityUtils.Demos.IdentityServer4.SingleTenant;
using Intellegens.Tests.Commons.WebApps;
using Microsoft.AspNetCore.Hosting;

namespace IdentityUtils.Demos.IdentityServer4.Tests.Setup
{
    public class SingleTenantWebApplicationFactory : CustomWebApplicationFactoryBase<TestSingleTenantStartup, Startup>
    {
        public SingleTenantWebApplicationFactory() : base()
        {
        }

        protected override TestSingleTenantStartup ConstructStartup(IWebHostEnvironment webHostEnvironment)
        {
            return new TestSingleTenantStartup(webHostEnvironment);
        }
    }
}