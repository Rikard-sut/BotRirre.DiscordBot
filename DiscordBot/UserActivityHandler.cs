using Discord.WebSocket;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class UserActivityHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly UserActivityService _userActivityService;

        public UserActivityHandler(DiscordSocketClient client, IServiceProvider services, UserActivityService userActivityService)
        {
            _client = client;
            _services = services;
            _userActivityService = userActivityService;
        }

        public Task Initialize()
        {
            _client.UserVoiceStateUpdated += OnVoiceStateUpdated;
            return Task.CompletedTask;
        }

        private async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState voiceChannelLeft, SocketVoiceState voiceChannelJoined)
        {
            if (user.IsBot)
                return;

            await _userActivityService.VoiceStateUpdated(user, voiceChannelLeft, voiceChannelJoined);
        }
    }
}
