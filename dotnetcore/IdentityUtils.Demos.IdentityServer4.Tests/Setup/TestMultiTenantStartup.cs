using IdentityUtils.Demos.IdentityServer4.MultiTenant;
using Intellegens.Tests.Commons.WebApps;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;

namespace IdentityUtils.Demos.IdentityServer4.Tests.Setup
{
    public class TestMultiTenantStartup : Startup, ITestStartup
    {
        public TestMultiTenantStartup(IWebHostEnvironment env) : base(env)
        {
        }

        public HttpMessageHandler BackchannelHttpHandler { get; set; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        protected override void JwtBearerOptionsCallback(JwtBearerOptions options)
        {
            options.BackchannelHttpHandler = BackchannelHttpHandler;
            base.JwtBearerOptionsCallback(options);
        }
    }
}