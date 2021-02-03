using IdentityUtils.Api.Models.Authentication;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant.Configuration
{
    public class IdentityUtilsConfiguration : IIdentityUtilsAuthenticationConfig
    {
        public string ClientId => "jsapp";
    }
}