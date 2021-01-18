using System.Net.Http;

namespace DiscordBot.Factories
{
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(string baseAddress);
    }
}