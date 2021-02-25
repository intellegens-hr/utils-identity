using IdentityUtils.Api.Models.Authentication;
using IdentityUtils.Core.Contracts.Configuration;
using Microsoft.Extensions.Configuration;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant.Configuration
{
    public class IdentityUtilsConfiguration : IIdentityUtilsAuthenticationConfig, IIdentityUtilsMultitenantConfiguration
    {
        private readonly IConfiguration configuration;

        public IdentityUtilsConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string ClientId => "jsapp";

        public string Hostname => configuration.GetValue<string>("Is4Host");
    }
}