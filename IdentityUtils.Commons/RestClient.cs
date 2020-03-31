using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IdentityUtils.Commons
{
    public class RestClient : IDisposable
    {
        protected HttpClient httpClient = new HttpClient();

        protected virtual async Task<HttpRequestMessage> GetHttpRequestMessage(HttpMethod method, string url)
        {
            var message = new HttpRequestMessage(method, url);
            return message;
        }

        public virtual async Task<T> Get<T>(string url)
        {
            var message = await GetHttpRequestMessage(HttpMethod.Get, url);
            var response = await httpClient.SendAsync(message);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public virtual async Task<T> Delete<T>(string url)
        {
            var message = await GetHttpRequestMessage(HttpMethod.Delete, url);
            var response = await httpClient.SendAsync(message);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public virtual async Task<T> Post<T>(string url, object dataToSend = null)
        {
            var message = await GetHttpRequestMessage(HttpMethod.Post, url);

            if (dataToSend != null)
                message.Content = new StringContent(JsonConvert.SerializeObject(dataToSend), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(message);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}