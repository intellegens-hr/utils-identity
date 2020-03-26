using IdentityModel.Client;
using IdentityUtils.Demos.Api.Configuration;
using IdentityUtils.Demos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityUtils.Demos.Api.ControllersApi
{
    [Route("/api/login")]
    public class AuthenticationControllerApi : ControllerBase
    {
        private readonly AppSettings appSettings;

        public AuthenticationControllerApi(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        [HttpPost("token/id")]
        public async Task<JsonResult> GetIdToken([FromBody]LoginModel loginModel)
        {
            using var client = new HttpClient();
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = $"{appSettings.Is4Host}/connect/token",
                ClientId = appSettings.Is4JsClientId,
                UserName = loginModel.Username,
                Password = loginModel.Password
            });

            return new JsonResult(new { tokenData = tokenResponse.Raw });
        }

        [HttpPost("token/revoke")]
        public async Task<JsonResult> RevokeRefreshToken([FromBody]TokenRevokeModel tokenModel)
        {
            using var client = new HttpClient();

            await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = $"{appSettings.Is4Host}/connect/revocation",
                TokenTypeHint = "refresh_token",
                ClientId = appSettings.Is4JsClientId,
                Token = tokenModel.RefreshToken
            });

            await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = $"{appSettings.Is4Host}/connect/revocation",
                TokenTypeHint = "access_token",
                ClientId = appSettings.Is4JsClientId,
                Token = tokenModel.AccessToken
            });

            return new JsonResult(new { message = "revoked" });
        }
    }
}