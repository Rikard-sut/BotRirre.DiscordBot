using Discord.WebSocket;
using DiscordBot.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class UserActivityService //Todo: Setup db to track users activities.
    {
        private readonly Storage _storage;

        public UserActivityService(Storage storage)
        {
            _storage = storage;
        }

        public async Task VoiceStateUpdated(SocketUser user, SocketVoiceState channelLeft, SocketVoiceState channelJoined)
        {
            //If a user joined and isnt stored already just return after storing the users channeltimestamp.
            if (channelJoined.VoiceChannel != null && !IsUserStored(user))
            {
                var newUser = new UserData { Username = user.Username.ToLower() };
                newUser.VoiceChannels.Add(new VoiceChannelData { VoiceChannelName = channelJoined.VoiceChannel.Name, Joined = DateTime.Now });
                _storage.Users.Add(newUser);

                return; 
            }

            if(channelJoined.VoiceChannel != null && IsUserStored(user))
            {
                var storedUser = _storage.Users.Find(x => x.Username == user.Username.ToLower());
                if(storedUser.VoiceChannels.Find(x => x.VoiceChannelName == channelJoined.VoiceChannel.Name) != null)
                {
                    var channel = storedUser.VoiceChannels.Find(x => x.VoiceChannelName == channelJoined.VoiceChannel.Name);
                    channel.Joined = DateTime.Now;
                }
                else
                {
                    var newChannel = new VoiceChannelData { VoiceChannelName = channelJoined.VoiceChannel.Name, Joined = DateTime.Now };
                    storedUser.VoiceChannels.Add(newChannel);
                }
            }

            if(channelLeft.VoiceChannel != null && IsUserStored(user))
            {
                var storedUser = _storage.Users.Find(x => x.Username == user.Username.ToLower());
                var channel = storedUser.VoiceChannels.Find(x => x.VoiceChannelName == channelLeft.VoiceChannel.Name);
                var timeSpenCurrentSession = (DateTime.Now - channel.Joined).TotalMinutes;
                channel.TimeSpentInChannel += timeSpenCurrentSession;

                await StoreData();
            }
        }

        public Task<UserData> GetData(string userName)
        {
            try
            {
                var user = _storage.Users.Find(x => x.Username == userName);
                return Task.FromResult(user);
            }
            catch
            {
                return Task.FromResult(new UserData() { Username = "notFound"});
            }
        }

        public bool IsUserStored(SocketUser user)
        {
            return _storage.Users.Where(x => x.Username == user.Username.ToLower()).FirstOrDefault() != null;
        }

        public Task StoreData()
        {
            Stream SaveFileStream = File.Create(Constants.File.Path);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(SaveFileStream, _storage);
            SaveFileStream.Close();
            return Task.CompletedTask;
        }
    }
}
