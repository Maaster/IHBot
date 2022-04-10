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
            //Console.WriteLine(data);


        }

        public async Task HandleMessage(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            //If Message isnt a command
            if (!message.Content.StartsWith(BotConfig.PREFIX))
                return;

            //Help command
            if(message.Content.StartsWith(BotConfig.PREFIX+"help"))
            {
                await message.Channel.SendMessageAsync("Test Help Message");
                return;
            }
            //Test all Huntresses command. Just usable by me since it floods channels. Maybe use ID here to future-proof and put it in BotConfig? Im too lazy to look up my ID.
            else if(message.Content.StartsWith(BotConfig.PREFIX+"testall") && message.Author.Username.Equals("Maaster") && message.Author.DiscriminatorValue.Equals(1273))
            {
                //await message.Channel.SendMessageAsync("Test all!");
                await TestPrintAll(message);
                return;
            }
            
            //Get command arguments
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

            //Remove prefix since we already tested for it above.
            switch(cmd[0].Remove(0,1))
            {
                case "huntress":
                    await ProcessHuntressCommand(message);
                    break;
                default:
                    await message.Channel.SendMessageAsync("Command not found!");
                    Console.WriteLine("Command not found");
                    break;
            }

        }

        private async Task TestPrintAll(SocketMessage msg)
        {
            foreach(Huntress hun in huntresses)
            {
                await SendHuntressDataToServer(msg, hun);
                await Task.Delay(2000);
            }

        }

        private async Task ProcessHuntressCommand(SocketMessage msg)
        {
            string huntressName = msg.Content.Substring(msg.Content.IndexOf(' ') + 1).ToLower();

            foreach (Huntress hun in huntresses)
            {
                string hunName = hun.name.ToLower();
                string hunNick = hun.nick.ToLower();

                if(huntressName.Equals(hunName) || huntressName.Equals(hunNick))
                {
                    await SendHuntressDataToServer(msg, hun);
                    return;
                }
            }

            await msg.Channel.SendMessageAsync("Huntress not found!");
            Console.WriteLine("Huntress not found!");
        }

        private async Task SendHuntressDataToServer(SocketMessage msg, Huntress hun)
        {
            //Search for the Huntress' Icon
            DirectoryInfo info = new DirectoryInfo(@".\icons");
            FileInfo[] icons = info.GetFiles();
            string hun_icon = "";

            foreach (FileInfo icon in icons)
            {
                if (icon.Name.Contains(hun.name))
                {
                    hun_icon = icon.FullName;
                }
            }

            //Set Filename for Embed to use
            hun.SetIconName(Path.GetFileName(hun_icon));

            //Get and Set Emotes needed for the Huntress
            hun.setEmotes(GetEmotes(hun.type, hun.attribute, hun.rarity));

            //Generate Embed
            Embed embed = hun.ToDiscordMessage();

            //Send to Channel. If no Icon was found, send it without.
            if (String.IsNullOrEmpty(hun_icon))
                await msg.Channel.SendMessageAsync("", false, embed);
            else
                await msg.Channel.SendFileAsync(hun_icon, "", false, embed);
            
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
            //Rarity
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
