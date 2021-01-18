using System;
using System.Net.Http;

namespace DiscordBot.Factories
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string baseAddress)
        {
            var client = new HttpClient();
            SetupClient(client, baseAddress);
            return client;
        }

        protected virtual void SetupClient(HttpClient client, string baseAddress)
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.BaseAddress = new Uri(baseAddress);
        }
    }
}
