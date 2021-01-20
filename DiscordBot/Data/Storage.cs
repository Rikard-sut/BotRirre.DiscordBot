using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Data
{
    [Serializable]
    public class Storage
    {
        public List<UserData> Users { get; set; } = new List<UserData>();
    }
}
