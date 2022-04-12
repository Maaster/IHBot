using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IHBot.Config;
using IHBot.data;
using IHBot.Tools;
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
            //path = Path.Combine(path, "data\\huntresses.json");
            path = Path.Combine(path, "data");//.huntresses.json");
            path = Path.Combine(path, "huntresses.json");
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
            if (!message.Content.StartsWith(BotConfig.PREFIX) || message.Content.Length < 3)
                return;

            //Help command
            if(message.Content.StartsWith(BotConfig.PREFIX+"help"))
            {
                await message.Channel.SendMessageAsync("Search for Huntress Info via !huntress <NAME>.");
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
                //Console.WriteLine("Command is empty");
                return;
            }

            //Remove prefix since we already tested for it above.
            switch(cmd[0].Remove(0,1).ToLower())
            {
                case "huntress":
                    await ProcessHuntressCommand(message);
                    break;
                case "list":
                    await ProcessListCommand(message);
                    break;
                default:
                    await message.Channel.SendMessageAsync("Command not found!");
                    //Console.WriteLine("Command not found");
                    break;
            }

        }

        private async Task ProcessListCommand(SocketMessage msg)
        {
            string queriedName = msg.Content.Substring(msg.Content.IndexOf(' ') + 1).ToLower();

            //Check if empty - if no name entered, IndexOf returns -1 and Substring returns the same string, thus queriedName will be the original message.
            if (String.IsNullOrEmpty(queriedName) || msg.Content.ToLower().Equals(queriedName))
            {
                await msg.Channel.SendMessageAsync("No huntress name entered!");
                return;
            }
            
            //If searching for Elements
            if(BotConfig.ELEMENTS.Contains(queriedName))
            {
                SendElementInfo(msg, queriedName);
            }
            //If searching for CC
            else if(BotConfig.CC_NAMES.Contains(queriedName))
            {
                SendCCInfo(msg, queriedName);
            }
            else
            {
                await msg.Channel.SendMessageAsync("No valid input detected! Valid inputs are: `" + String.Join(", ", BotConfig.ELEMENTS) + "` for elements or `" + String.Join(", ", BotConfig.CC_NAMES) + "` for CC");
            }


            return;
        }

        private async void SendCCInfo(SocketMessage msg, string queriedName)
        {
            List<Huntress> matches = new List<Huntress>();

            string ccInfo = "";

            //Looks like /r/shittyprogramming but eh, I cant be arsed to find a better solution.
            switch (queriedName)
            {
                case "freeze":
                    ccInfo = StatusInfo.FREEZE;
                    queriedName = "Frost Mark";
                    break;
                case "stun":
                    ccInfo = StatusInfo.STUN;
                    break;
                case "vacant":
                    ccInfo = StatusInfo.VACANT;
                    break;
                case "flying":
                    ccInfo = StatusInfo.FLYING;
                    break;
                case "burn":
                    ccInfo = StatusInfo.BURN;
                    //queriedName = "Burn Mark";
                    break;
                case "hate":
                    ccInfo = StatusInfo.HATE;
                    break;
                case "silence":
                    ccInfo = StatusInfo.SILENCE;
                    break;
                case "pierce":
                    ccInfo = StatusInfo.PIERCE;
                    queriedName = "armor-piercing";
                    break;
                case "curse":
                    ccInfo = StatusInfo.CURSE;
                    break;
                case "weak":
                    ccInfo = StatusInfo.WEAK;
                    break;
                case "tear":
                    ccInfo = StatusInfo.TEAR;
                    break;
                case "slow":
                    ccInfo = StatusInfo.SLOW;
                    break;
                case "realm":
                    ccInfo = StatusInfo.REALM;
                    break;
                case "blind":
                    ccInfo = StatusInfo.BLIND;
                    break;
                case "entwined":
                    ccInfo = StatusInfo.ENTWINED;
                    queriedName = "entwining";
                    break;
                default:
                    await msg.Channel.SendMessageAsync("Status not found! This shouldnt happen lol, please ping Maaster#1273!");
                    break;
            }

            //Search all skills for the status and add them to list
            foreach(Huntress match in huntresses)
            {
                if(match.skill1.Contains(queriedName, StringComparison.OrdinalIgnoreCase)
                    || match.skill2.Contains(queriedName, StringComparison.OrdinalIgnoreCase)
                    || match.passive1.Contains(queriedName, StringComparison.OrdinalIgnoreCase)
                    || match.passive2.Contains(queriedName, StringComparison.OrdinalIgnoreCase))
                    matches.Add(match);
            }

            string names = "";

            //Fail-safe - shouldnt happen.
            if (matches.Count > 0)
            {
                //Concat all names for display in Discord
                foreach (Huntress match in matches)
                {
                    names += "`" + match.name + "` , ";
                }
                //Trim end so it looks nice.
                names = names.Remove(names.Length - 3);
            }
            else
            {
                Console.WriteLine("No matches found in CCInfo for " + queriedName);
            }
            await msg.Channel.SendMessageAsync(ccInfo + "\n\n The following Huntresses deal with this Status:\n" + names);
        }

        private async void SendElementInfo(SocketMessage msg, string queriedName)
        {
            List<Huntress> matches = new List<Huntress>();

            //Go through all huntresses, looking to match attribute
            foreach(Huntress huntress in huntresses)
            {
                if(huntress.attribute.ToLower().Equals(queriedName))
                    matches.Add(huntress);
            }

            string names = "";

            //Concat all names for display in Discord
            foreach(Huntress match in matches)
            {
                names += "`" + match.name + "` , ";
            }

            //Trim end so it looks nice.
            names = names.Remove(names.Length - 3);


            await msg.Channel.SendMessageAsync("The following Huntresses are " + queriedName + ": " + names);
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
            List<Huntress> matches = new List<Huntress>();

            //Get Huntress Name entered
            string queriedName = msg.Content.Substring(msg.Content.IndexOf(' ') + 1).ToLower();

            //Check if empty - if no name entered, IndexOf returns -1 and Substring returns the same string, thus queriedName will be the original message.
            if(String.IsNullOrEmpty(queriedName) || msg.Content.ToLower().Equals(queriedName))
            {
                await msg.Channel.SendMessageAsync("No huntress name entered!");
                return;
            }

            //Search for Huntress by entered name - complete match of name or nickname
            foreach (Huntress hun in huntresses)
            {
                string hunName = hun.name.ToLower();
                string[] hunNick = hun.nick.ToLower().Split(';');

                //If the name is a complete match or starts with it, use it.
                if(queriedName.Equals(hunName) || hunName.StartsWith(queriedName))
                {
                    matches.Add(hun);
                }
                //Try full name for a simple typo, done by Levenshtein Distance. TODO: Switch to Damerau-Levenshtein Distance.
                else if(LevenshteinDistance.EditDistance(hunName, queriedName) <= BotConfig.LEV_DISTANCE)
                {
                    matches.Add(hun);
                }
                //Full name wasnt a match, try all the nicks, seperated by ;
                else
                {
                    foreach(string nick in hunNick)
                    {
                        if(queriedName.Equals(nick))
                        {
                            matches.Add(hun);
                        }
                    }
                }
            }

            //If no match was found, tell the user
            if (matches.Count == 0)
            {
                await msg.Channel.SendMessageAsync("Huntress not found!");
                //Console.WriteLine("Huntress not found!");
            }
            //If only one match was found, just send it
            else if(matches.Count == 1)
            {
                await SendHuntressDataToServer(msg, matches.First());
            }
            //Multiple matches found, tell the user.
            else
            {
                string names = "Did you mean: `";
                foreach (Huntress hun in matches)
                {
                    names += hun.name + "`,`";
                }
                names = names.Substring(0, names.Length - 2) + "?";
                await msg.Channel.SendMessageAsync(names);
            }
        }

        private async Task SendHuntressDataToServer(SocketMessage msg, Huntress hun)
        {
            //Search for the Huntress' Icon
            string path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "icons");
            DirectoryInfo info = new DirectoryInfo(path);
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
