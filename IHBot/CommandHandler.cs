using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IHBot.Config;
using IHBot.data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IHBot
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private List<Huntress> huntresses;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            huntresses = new List<Huntress>();

            LoadHuntressData();
        }

        private void LoadHuntressData()
        {
            string path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "data\\huntresses.json");
            string data = File.ReadAllText(path);
            //data = data.Substring(1, data.Length - 2);
            HuntressJSON jsonObj = JsonConvert.DeserializeObject<HuntressJSON>(data);
            huntresses = jsonObj.huntressList;
            Console.WriteLine(data);


        }

        public async Task HandleMessage(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            //If Message isnt a command
            if (!message.Content.StartsWith(BotConfig.PREFIX))
                return;

            if(message.Content.StartsWith(BotConfig.PREFIX+"help"))
            {
                await message.Channel.SendMessageAsync("Test Help Message");
            }
            
            string[] cmd = message.Content.Split(' ');
            /*
            string command = "";
            command = message.Content.Substring(1, message.Content.IndexOf(' '));
            */
            //if(String.IsNullOrEmpty(command))
            if(cmd == null || cmd.Length == 0)
            {
                Console.WriteLine("Command is empty");
                return;
            }

            switch(cmd[0])
            {
                case "!huntress":
                    await ProcessHuntressCommand(message);
                    break;
                default:
                    Console.WriteLine("command not found");
                    break;
            }

        }

        private async Task ProcessHuntressCommand(SocketMessage msg)
        {
            string huntressName = msg.Content.Substring(msg.Content.IndexOf(' ') + 1);

            foreach (Huntress hun in huntresses)
            {
                string hunName = hun.name;
                string hunNick = hun.nick;

                if(huntressName.Equals(hunName) || huntressName.Equals(hunNick))
                {
                    await SendHuntressDataToServer(msg, hun);
                    return;
                }
            }

            Console.WriteLine("Huntress not found!");
        }

        private async Task SendHuntressDataToServer(SocketMessage msg, Huntress hun)
        {
            hun.setEmotes(GetEmotes(hun.type, hun.attribute, hun.rarity));

            Embed embed = hun.ToDiscordMessage();

            await msg.Channel.SendMessageAsync("",false,embed);
            
        }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        private Emote[] GetEmotes(string type, string attribute, string rarity)
        {
            Emote[] emotes = new Emote[3];

            //Type
            Emote emote = _client.Guilds
            .SelectMany(x => x.Emotes)
            .FirstOrDefault(x => x.Name.IndexOf(
                type, StringComparison.OrdinalIgnoreCase) != -1);
            if (emote == null)
            {
                Console.WriteLine("Couldnt find Emote by name: " + type);
            }
            else
                emotes[0] = emote;

            //Attribute
            emote = _client.Guilds
            .SelectMany(x => x.Emotes)
            .FirstOrDefault(x => x.Name.IndexOf(
                attribute, StringComparison.OrdinalIgnoreCase) != -1);
            if (emote == null)
            {
                Console.WriteLine("Couldnt find Emote by name: " + attribute);
            }
            else
                emotes[1] = emote;
            /*
            //Attribute
            emote = _client.Guilds
            .SelectMany(x => x.Emotes)
            .FirstOrDefault(x => x.Name.IndexOf(
                rarity, StringComparison.OrdinalIgnoreCase) != -1);
            if (emote == null)
            {
                Console.WriteLine("Couldnt find Emote by name: " + rarity);
            }
            else
                emotes[2] = emote;
            */


            return emotes;


#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }
    }
}
