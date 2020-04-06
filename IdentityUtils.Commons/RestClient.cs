using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IdentityUtils.Commons
{
    public class RestClient : IDisposable
    {
        protected HttpClient httpClient;

        public RestClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public RestClient()
        {
            this.httpClient = new HttpClient();
        }

        protected virtual async Task<HttpRequestMessage> GetHttpRequestMessage(HttpMethod method, string url)
        {
            var message = new HttpRequestMessage(method, url);
            return message;
        }

        private async Task<RestResult<T>> GetResponseResult<T>(HttpResponseMessage responseMessage)
        {
            string content = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                string errorMessage = responseMessage.StatusCode.ToString();
                if (!string.IsNullOrEmpty(content))
                    errorMessage = $"{errorMessage}: {content}";

                return RestResult<T>.ErrorResult(errorMessage);
            }

            return RestResult<T>.SuccessResult(JsonConvert.DeserializeObject<T>(content));
        }

        public virtual async Task<RestResult<T>> Get<T>(string url)
        {
            var message = await GetHttpRequestMessage(HttpMethod.Get, url);
            var response = await httpClient.SendAsync(message);

            return await GetResponseResult<T>(response);
        }

        public virtual async Task<RestResult<T>> Delete<T>(string url)
        {
            var message = await GetHttpRequestMessage(HttpMethod.Delete, url);
            var response = await httpClient.SendAsync(message);

            string content = await response.Content.ReadAsStringAsync();

            return await GetResponseResult<T>(response);
        }

        public virtual async Task<RestResult<T>> Post<T>(string url, object dataToSend = null)
        {
            var message = await GetHttpRequestMessage(HttpMethod.Post, url);

            if (dataToSend != null)
                message.Content = new StringContent(JsonConvert.SerializeObject(dataToSend), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(message);

            return await GetResponseResult<T>(response);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}