using Discord;
using Discord.WebSocket;
using IHBot.Config;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IHBot     
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            //TO PUBLISH FOR LINUX, USE THE PUBLISH CONTEXT MENU (RIGHTLICK PROJECT) AND SELECT LINUX-ARM64 AS RUNTIME ON RELEASE.


            _client = new DiscordSocketClient();
            _handler = new CommandHandler(_client);

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
            var token = Credentials.BOT_TOKEN;

            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task MessageReceived(SocketMessage msg)
        {
            _handler.HandleMessage(msg);
            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}