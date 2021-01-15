using Discord;
using Discord.Commands;
using Victoria.Enums;
using Discord.WebSocket;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using System.Linq;

namespace DiscordBot.Modules
{
    [Group("Dj")]
    public class Music : ModuleBase<SocketCommandContext> 
    {
        private readonly LavaNode _lavaNode; //Todo : Inject LavalinkService instead and refactor command methods to be alot cleaner.

        public Music(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Command("Join")]
        [Summary(": joins the voicechannel you are in.")]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice channel!\nUse !Disconnect first to allow me to join another channel.");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("Disconnect")]
        [Summary(": leaves the voicechannel.")]
        public async Task Disconnect()
        {
            var voiceState = Context.User as IVoiceState;

            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
                await ReplyAsync($"Disconnected from {voiceState.VoiceChannel.Name}!");
            }
        }

        [Command("Play")]
        [Summary(": play desired song, example !Dj play Metallica - One")]
        public async Task Play([Remainder] string query)
        {
            var guild = Context.Guild as IGuild;
            var player = _lavaNode.GetPlayer(guild);
            var results = await _lavaNode.SearchYouTubeAsync(query);
            if (results.LoadStatus == LoadStatus.NoMatches || results.LoadStatus == LoadStatus.LoadFailed)
            {
                await ReplyAsync("No matches found.");
            }

            var track = results.Tracks.FirstOrDefault();

            if (player.PlayerState == PlayerState.Playing)
            {
                player.Queue.Enqueue(track);
                await ReplyAsync("Track added to queue.");
            }
            else
            {
                await player.PlayAsync(track);
                await ReplyAsync($"Now playing: {track.Title}");
            }
        }

        [Command("Skip")]
        [Summary(": skip current track.")]
        public async Task Skip()
        {
            var guild = Context.Guild as IGuild;
            var player = _lavaNode.GetPlayer(guild);
            var oldTrack = player.Track.Title;

            if (player != null)
            {
                await player.SkipAsync();
                await ReplyAsync($"Skipped {oldTrack} \nNow playing {player.Track.Title}");
            }
        }

        [Command("Pause")]
        [Summary(": pause current track.")]
        public async Task Pause()
        {
            var guild = Context.Guild as IGuild;
            var player = _lavaNode.GetPlayer(guild);

            if (player != null)
                await player.PauseAsync();
        }

        [Command("Resume")]
        [Summary(": resumes paused track.")]
        public async Task Resume()
        {
            var guild = Context.Guild as IGuild;
            var player = _lavaNode.GetPlayer(guild);

            if (player != null)
                await player.ResumeAsync();
        }

        [Command("Stop")]
        [Summary(": stop track.")]
        public async Task Stop()
        {
            var guild = Context.Guild as IGuild;
            var player = _lavaNode.GetPlayer(guild);

            if (player != null)
                await player.StopAsync();
        }

        [Command("ClearAll")]
        [Summary(": removes all queud songs")]
        public async Task ClearAll()
        {
            var guild = Context.Guild as IGuild;
            var player = _lavaNode.GetPlayer(guild);

            if (player != null)
            {
                player.Queue.Clear();
                await ReplyAsync("Cleared all queued tracks");
            }
        }

        [Command("Clear")]
        [Summary(": remove desired track from queue, example !Dj clear Metallica - One.")]
        public async Task Clear([Remainder]string query)
        {
            var guild = Context.Guild as IGuild;
            var player = _lavaNode.GetPlayer(guild);
            var results = await _lavaNode.SearchYouTubeAsync(query);
            if (results.LoadStatus == LoadStatus.NoMatches || results.LoadStatus == LoadStatus.LoadFailed)
            {
                await ReplyAsync("No matches found.");
            }
            var track = results.Tracks.FirstOrDefault();

            if (player != null)
            {
                player.Queue.Remove(track);
                await ReplyAsync($"Removed {track.Title} from queue");
            }
        }

        [Command("Volume")]
        [Summary(": set the volume. 10-150")]
        public async Task Volume(int volume)
        {
            if (volume > 150 || volume < 10)
               await ReplyAsync("Volume must be between 10 and 150");

            var guild = Context.Guild as IGuild;
            var player = _lavaNode.GetPlayer(guild);

            await player.UpdateVolumeAsync((ushort)volume);
            await ReplyAsync($"Volume set to {volume}");
        }
    }
}
