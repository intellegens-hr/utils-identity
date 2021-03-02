using IdentityUtils.Api.Controllers;
using IdentityUtils.Api.Controllers.Authentication.Services;
using IdentityUtils.Api.Models.Authentication;
using IdentityUtils.Core.Contracts.Commons;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdentityUtils.Demos.IdentityServer4.SingleTenant.ControllersApi
{
    public class AuthDemoControllerApi : AuthenticationControllerApi
    {
        public AuthDemoControllerApi(IIdentityUtilsAuthService identityUtilsAuthService, ILogger<AuthenticationControllerApi> logger) : base(identityUtilsAuthService, logger)
        {
        }

        public override Task<IdentityUtilsResult<Api.Models.Authentication.TokenResponse>> GetToken(TokenRequest request)
        {
            return base.GetToken(request);
        }

        public override Task<IdentityUtilsResult<UserProfile>> ProfileInit()
        {
            return base.ProfileInit();
        }
    }
}