using IdentityModel.Client;
using IdentityUtils.Commons;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions
{
    /// <summary>
    /// Inherits RestClient and overrides GetHttpRequestMessage to
    /// fetch authentication token from IS4 and add it to header.
    /// </summary>
    public abstract class ApiHttpClient : RestClient
    {
        protected abstract string BasePath { get; }
        protected abstract IApiWrapperConfig WrapperConfig { get; }

        private async Task<string> GetToken()
        {
            var client = new HttpClient();
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = $"{WrapperConfig.Is4Hostname}/connect/token",
                ClientId = WrapperConfig.ClientId,
                ClientSecret = WrapperConfig.ClientSecret,
                Scope = WrapperConfig.ClientScope
            });

            return tokenResponse.AccessToken;
        }

        protected override async Task<HttpRequestMessage> GetHttpRequestMessage(HttpMethod method, string url)
        {
            var token = await GetToken();
            var message = new HttpRequestMessage(method, url);
            message.Headers.Add("Authorization", $"Bearer {token}");

            return message;
        }
    }
}