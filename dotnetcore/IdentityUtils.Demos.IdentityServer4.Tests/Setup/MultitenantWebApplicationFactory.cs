using IdentityUtils.Demos.IdentityServer4.MultiTenant;
using Intellegens.Tests.Commons.WebApps;
using Microsoft.AspNetCore.Hosting;

namespace IdentityUtils.Demos.IdentityServer4.Tests.Setup
{
    public class MultitenantWebApplicationFactory : CustomWebApplicationFactoryBase<TestMultiTenantStartup, Startup>
    {
        public MultitenantWebApplicationFactory() : base()
        {
        }

        protected override TestMultiTenantStartup ConstructStartup(IWebHostEnvironment webHostEnvironment)
        {
            return new TestMultiTenantStartup(webHostEnvironment);
        }
    }
}