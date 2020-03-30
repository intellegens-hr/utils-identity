using IdentityUtils.Commons.Mailing.Google;

namespace IdentityUtils.Demos.IdentityServer4.Configuration
{
    public class MasterConfig : IGoogleMailingProviderConfig
    {
        public string Username => "intellegensdemo@gmail.com";
        public string Password => "sysgegmqfyvxldyl";
    }
}