using IdentityUtils.Api.Controllers.Authentication.Services;
using IdentityUtils.Api.Models.Authentication;
using Microsoft.Extensions.Logging;

namespace IdentityUtils.Api.Controllers
{
    public class AuthenticationControllerApi : AuthenticationControllerApiBase<UserProfile>
    {
        public AuthenticationControllerApi(
            IIdentityUtilsAuthService identityUtilsAuthService,
            ILogger<AuthenticationControllerApi> logger) : base(identityUtilsAuthService, logger)
        {
        }
    }
}