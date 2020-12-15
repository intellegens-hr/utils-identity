using IdentityUtils.Api.Extensions.RestClients;
using System;

namespace IdentityUtils.Demos.IdentityServer4.Tests.Setup.Configuration
{
    public class ApiExtensionsIs4Config : IApiExtensionsIs4Config
    {
        public string ClientId => "is4management";
        public string ClientScope => "demo-is4-management-api";
        public string ClientSecret => "511536EF-F270-4058-80CA-1C89C192F69A";
        public string Hostname => Environment.GetEnvironmentVariable("Is4Host");
    }
}