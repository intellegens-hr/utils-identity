using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Commons
{
    public class IntellegensHttpClient : IDisposable
    {
        protected HttpClient httpClient = new HttpClient();

        protected virtual async Task<HttpRequestMessage> GetHttpRequestMessage(HttpMethod method, string url)
        {
            var message = new HttpRequestMessage(method, url);
            return message;
        }

        protected async Task<T> Get<T>(string url)
        {
            var message = await GetHttpRequestMessage(HttpMethod.Get, url);
            var response = await httpClient.SendAsync(message);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        protected async Task<T> Delete<T>(string url)
        {
            var message = await GetHttpRequestMessage(HttpMethod.Delete, url);
            var response = await httpClient.SendAsync(message);

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        protected async Task<T> Post<T>(string url, object dataToSend)
        {
            var message = await GetHttpRequestMessage(HttpMethod.Post, url);
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