using DiscordBot.Dto;
using DiscordBot.Factories;
using Polly;
using Polly.Registry;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class CryptoCoinService : ICryptoCoinService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAsyncPolicy<HttpResponseMessage> _cachePolicy;

        public CryptoCoinService(IHttpClientFactory factory, IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            _clientFactory = factory;
            _cachePolicy = policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>(Constants.CachePolicys.HttpGetCache);
        }

        public async Task<Dictionary<string, BitcoinInfo>> GetBitcoinPrice()
        {
            var client = _clientFactory.CreateClient(Constants.Urls.BitcoinUrl);

            var response = await _cachePolicy.ExecuteAsync(context => client.GetAsync(""), new Context("bitcoin"));

            var result = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Dictionary<string, BitcoinInfo>>(result);
        }
    }
}
