using DiscordBot.Dto;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class CryptoCoinService : ICryptoCoinService
    {
        private readonly HttpClient _client;
        private readonly IAsyncPolicy<HttpResponseMessage> _cachePolicy;

        public CryptoCoinService(HttpClient client, IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            _client = client;
            _cachePolicy = policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>(Constants.CachePolicys.HttpGetCache);
        }

        public async Task<Dictionary<string, BitcoinInfo>> GetBitcoinPrice()
        {
            _client.BaseAddress = new Uri(Constants.Urls.BitcoinUrl);

            var response = await _cachePolicy.ExecuteAsync(context => _client.GetAsync(""), new Context("bitcoin"));

            var result = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Dictionary<string, BitcoinInfo>>(result);
        }
    }
}
