using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Victoria;
using Victoria.EventArgs;

namespace DiscordBot.Services
{
    public class LavaLinkService
    {
        private readonly LavaNode _lavaNode;
        private readonly LavaConfig _config;
        private readonly DiscordSocketClient _client;
        private readonly ILoggingService _logger;

        public LavaLinkService(
            LavaNode node,
            LavaConfig config,
            DiscordSocketClient client,
            ILoggingService logger)
        {
            _lavaNode = node;
            _config = config;
            _client = client;
            _logger = logger;
        }

        public Task Initialize()
        {
            _client.Ready += ClientReady;
            _lavaNode.OnLog += _logger.LogAsync;
            _lavaNode.OnTrackEnded += TrackEnded;
            return Task.CompletedTask;
        }
        private async Task ClientReady()
        {
            // Avoid calling ConnectAsync again if it's already connected 
            // (It throws InvalidOperationException if it's already connected).
            if (!_lavaNode.IsConnected)
            {
                await _lavaNode.ConnectAsync();
            }
        }

        private async Task TrackEnded(TrackEndedEventArgs arg)
        {
            if (!arg.Reason.ShouldPlayNext())
                return;

            if (!arg.Player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await arg.Player.TextChannel.SendMessageAsync("There are no more queued tracks");
                return;
            }

            await arg.Player.PlayAsync(nextTrack);
        }
    }
}
