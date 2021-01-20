using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Data
{
    [Serializable]
    public class VoiceChannelData
    {
        public string VoiceChannelName { get; set; }

        public DateTime Joined { get; set; }

        public DateTime Left { get; set; }

        public double TimeSpentInChannel { get; set; }
    }
}
