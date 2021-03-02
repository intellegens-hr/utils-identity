using IdentityUtils.Api.Models.Authentication;

namespace IdentityUtils.Demos.IdentityServer4.SingleTenant.Configuration
{
    public class IdentityUtilsConfiguration : IIdentityUtilsAuthenticationConfig
    {
        public string ClientId => "jsapp";
    }
}