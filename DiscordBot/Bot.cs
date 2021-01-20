using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Data;
using DiscordBot.Factories;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Registry;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot
{
    public class Bot
    {
        public static void Main(string[] args)
            => new Bot().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private Storage _storage;

        public async Task RunBotAsync()
        {
            //register user secrets
            var config = new ConfigurationBuilder().AddUserSecrets<Bot>().Build();
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

            //load storage
            _storage = new Storage();
            await LoadStorage();

            //register services
            await RegisterServices();

            //setup
            await _services.GetRequiredService<ICommandHandler>().Intialize();
            await _services.GetRequiredService<LavaLinkService>().Initialize();
            await _services.GetRequiredService<UserActivityHandler>().Initialize();
            _services.GetRequiredService<ILoggingService>();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public Task<IServiceProvider> RegisterServices()
        {
            _services = new ServiceCollection()
                .AddMemoryCache()
                .AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>()
                .AddSingleton<IReadOnlyPolicyRegistry<string>, PolicyRegistry>((serviceProvider) =>
                {
                    return new PolicyRegistry
                    {
                        {
                            Constants.CachePolicys.HttpGetCache,
                            Policy.CacheAsync(
                            serviceProvider
                                .GetRequiredService<IAsyncCacheProvider>()
                                .AsyncFor<HttpResponseMessage>(),
                            TimeSpan.FromSeconds(60))
                        }
                    };
                })
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<IHttpClientFactory, HttpClientFactory>()
                .AddSingleton<ICommandHandler, CommandHandler>()
                .AddSingleton<ILoggingService, LoggingService>()
                .AddSingleton<ICryptoCoinService, CryptoCoinService>()
                .AddSingleton<UserActivityHandler>()
                .AddSingleton<UserActivityService>()
                .AddSingleton(_storage)
                .AddSingleton<LavaNode>()
                .AddSingleton<LavaConfig>()
                .AddSingleton<LavaLinkService>()
                .BuildServiceProvider();

            return Task.FromResult(_services);
        }

        public Task LoadStorage()
        {
            if (File.Exists(Constants.File.Path))
            {
                Console.WriteLine("Reading saved file");
                Stream openFileStream = File.OpenRead(Constants.File.Path);
                BinaryFormatter deserializer = new BinaryFormatter();
                _storage = (Storage)deserializer.Deserialize(openFileStream);
                openFileStream.Close();
            }
            return Task.CompletedTask;
        }
    }
}
