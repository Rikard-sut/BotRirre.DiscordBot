﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class LoggingService : ILoggingService
    {
        public LoggingService(DiscordSocketClient client, CommandService command)
        {
            client.Log += LogAsync;
            command.Log += LogAsync;
        }

        public Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
                Console.WriteLine($"[General/{message.Severity}] {message}");

            return Task.CompletedTask;
        }
    }
}
