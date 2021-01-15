using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Registry;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            //register user secrets
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var secretProvider = config.Providers.First();
            if (!secretProvider.TryGet(Constants.Token.RirreBotToken, out var botToken)) return;

            //Setup Discord Websocket
            var clientConfig = new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Verbose
            };
            _client = new DiscordSocketClient(clientConfig);

            var commandConfig = new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Verbose
            };
            _commands = new CommandService(commandConfig);


            //register services
            _services = new ServiceCollection()
                .AddMemoryCache()
                .AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>()
                .AddSingleton<IReadOnlyPolicyRegistry<string>, PolicyRegistry>((serviceProvider) =>
                {
                    PolicyRegistry registry = new PolicyRegistry();
                    registry.Add(Constants.CachePolicys.HttpGetCache,
                        Policy.CacheAsync(
                            serviceProvider
                                .GetRequiredService<IAsyncCacheProvider>()
                                .AsyncFor<HttpResponseMessage>(),
                            TimeSpan.FromSeconds(60)));
                    return registry;
                })
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddTransient<HttpClient>()
                .AddSingleton<ICommandHandler, CommandHandler>()
                .AddSingleton<ILoggingService, LoggingService>()
                .AddSingleton<ICryptoCoinService, CryptoCoinService>()
                .AddSingleton<LavaNode>()
                .AddSingleton<LavaConfig>()
                .AddSingleton<LavaLinkService>()
                .BuildServiceProvider();

            //setup
            await _services.GetRequiredService<ICommandHandler>().Intialize();
            await _services.GetRequiredService<LavaLinkService>().Initialize();
            _services.GetRequiredService<ILoggingService>();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
