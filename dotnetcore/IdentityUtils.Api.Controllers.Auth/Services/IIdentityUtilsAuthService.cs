using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;

namespace IdentityUtils.Api.Controllers.Authentication.Services
{
    public interface IIdentityUtilsAuthService
    {
        public IClientStore ClientStore { get; set; }
        public IEventService Events { get; set; }
        public ITokenRequestValidator RequestValidator { get; set; }
        public ITokenResponseGenerator ResponseGenerator { get; set; }
    }
}