using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Data
{
    [Serializable]
    public class UserData
    {
        public string Username { get; set; }

        public List<VoiceChannelData> VoiceChannels { get; set; } = new List<VoiceChannelData>();
    }
}
