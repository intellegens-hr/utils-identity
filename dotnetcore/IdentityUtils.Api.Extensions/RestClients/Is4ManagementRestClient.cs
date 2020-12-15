using IdentityModel;
using IdentityModel.Client;
using IdentityUtils.Commons;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions.RestClients
{
    /// <summary>
    /// Inherits RestClient and overrides GetHttpRequestMessage to
    /// fetch authentication token from IS4 and add it to header.
    /// </summary>
    public class Is4ManagementRestClient : RestClient
    {
        private readonly IApiExtensionsIs4Config is4Config;

        public Is4ManagementRestClient(IApiExtensionsIs4Config is4Config)
        {
            this.is4Config = is4Config;
        }

        protected override async Task<HttpRequestMessage> GetHttpRequestMessage(HttpMethod method, string url)
        {
            var token = await GetToken();
            var message = new HttpRequestMessage(method, url);
            message.SetBearerToken(token);

            return message;
        }

        private async Task<string> GetToken()
        {
            var disco = await httpClient.GetDiscoveryDocumentAsync(is4Config.Hostname);

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = is4Config.ClientId,
                ClientSecret = is4Config.ClientSecret,
                Scope = is4Config.ClientScope
            });

            return tokenResponse.AccessToken;
        }
    }
}