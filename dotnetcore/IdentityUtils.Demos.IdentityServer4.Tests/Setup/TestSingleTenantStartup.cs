using IdentityUtils.Demos.IdentityServer4.SingleTenant;
using Intellegens.Tests.Commons.WebApps;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;

namespace IdentityUtils.Demos.IdentityServer4.Tests.Setup
{
    public class TestSingleTenantStartup : Startup, ITestStartup
    {
        public TestSingleTenantStartup(IWebHostEnvironment env) : base(env)
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