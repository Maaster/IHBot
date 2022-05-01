using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IHBot.Config;
using IHBot.data;
using IHBot.data.tierList;
using IHBot.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IHBot
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private List<Huntress> huntresses;
        private List<TierData> tierDataList;
        private List<TierList> tierListsRoles;


        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            huntresses = new List<Huntress>();

            LoadHuntressData();
            LoadTierListData();
            LoadRolesTierList();

            SetupStatus();

        }

        private void SetupStatus()
        {
            System.Timers.Timer myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new ElapsedEventHandler(UpdateStatus);
            myTimer.Interval = 1000 * BotConfig.STATUS_UPDATE_INTERVAL; // 1000 ms is one second
            myTimer.Start();
            UpdateStatus(null,null);
        }

        private void UpdateStatus(object? sender, ElapsedEventArgs e)
        {
            //Get both now and Server Update time, then set difference as status
            DateTime now = DateTime.Now;
            DateTime resetTime = now;

            //Account for next day
            if (now.Hour >= 12)
            {
                //Set time to resetTime
                TimeSpan ts = new TimeSpan(12, 0, 0);
                resetTime = resetTime.Date + ts;
                resetTime = resetTime.AddDays(1);
            }
            else
            {
                resetTime = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
            }

            TimeSpan diff = resetTime - now;

            _client.SetGameAsync("Server Reset in: " + diff.ToString(@"hh\hmm\m"));
        }



        #region Loading JSONs
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

        private void LoadTierListData()
        {
            string path = Directory.GetCurrentDirectory();
            //path = Path.Combine(path, "data\\huntresses.json");
            path = Path.Combine(path, "data");//.huntresses.json");
            path = Path.Combine(path, "tierData.json");
            string data = File.ReadAllText(path);
            //data = data.Substring(1, data.Length - 2);
            TierDataJSON jsonObj = JsonConvert.DeserializeObject<TierDataJSON>(data);
            tierDataList = jsonObj.dataList;
            //Console.WriteLine(data);


        }

        private void LoadRolesTierList()
        {
            string path = Directory.GetCurrentDirectory();
            //path = Path.Combine(path, "data\\huntresses.json");
            path = Path.Combine(path, "data");//.huntresses.json");
            path = Path.Combine(path, "tierList.json");
            string data = File.ReadAllText(path);
            //data = data.Substring(1, data.Length - 2);
            TierListJSON jsonObj = JsonConvert.DeserializeObject<TierListJSON>(data);
            tierListsRoles = jsonObj.list;

        }
        #endregion

        #region Stat Rankings
        private int CalculateStatRanking(string stat, Huntress huntress)
        {
            int ranking = 0;
            //Copy Huntress List
            List <Huntress> orderedList = new List<Huntress>(huntresses);

            //Then order it by the stat given - descending order because we want highest
            switch(stat)
            {
                case "might":
                    orderedList = orderedList.OrderByDescending(h => h.might).ToList();
                    break;
                case "atk":
                    orderedList = orderedList.OrderByDescending(h => h.atk).ToList();
                    break;
                case "def":
                    orderedList = orderedList.OrderByDescending(h => h.def).ToList();
                    break;
                case "hp":
                    orderedList = orderedList.OrderByDescending(h => h.hp).ToList();
                    break;
                case "spd":
                    orderedList = orderedList.OrderByDescending(h => h.spd).ToList();
                    break;


            }

            //Find Ranking of Huntress searched for
            ranking = orderedList.IndexOf(huntress);

            //Shouldnt happen. Ever.
            if(ranking == -1)
            {
                Console.WriteLine("Huntress not found in orderedList: " + huntress.name);
            }

            
            return ranking + 1;
        }

        private async void GetStatRanking(SocketMessage msg)
        {
            string stat = msg.Content.Substring(msg.Content.IndexOf(' ') + 1).ToLower();

            //Check if empty - if no name entered, IndexOf returns -1 and Substring returns the same string, thus queriedName will be the original message.
            if (String.IsNullOrEmpty(stat) || msg.Content.ToLower().Equals(stat))
            {
                await msg.Channel.SendMessageAsync("No stat name entered! Please select from `Might`,`Atk`,`Def`,`HP` or `Spd`");
                return;
            }

            //Copy Huntress List
            List<Huntress> orderedList = new List<Huntress>(huntresses);


            string answer = "**Highest/Lowest 5 for " + stat + ":**\n";

            //Then order it by the stat given - descending order because we want highest
            switch (stat)
            {
                case "might":
                    orderedList = orderedList.OrderByDescending(h => h.might).ToList();
                    break;
                case "atk":
                    orderedList = orderedList.OrderByDescending(h => h.atk).ToList();
                    break;
                case "def":
                    orderedList = orderedList.OrderByDescending(h => h.def).ToList();
                    break;
                case "hp":
                    orderedList = orderedList.OrderByDescending(h => h.hp).ToList();
                    break;
                case "spd":
                    orderedList = orderedList.OrderByDescending(h => h.spd).ToList();
                    break;
                default:
                    await msg.Channel.SendMessageAsync("No valid stat name entered! Please select from `Might`,`Atk`,`Def`,`HP` or `Spd`");
                    return;
            }

            answer += "1st: " + orderedList.ElementAt(0).name + "\n";
            answer += "2nd: " + orderedList.ElementAt(1).name + "\n";
            answer += "3rd: " + orderedList.ElementAt(2).name + "\n";
            answer += "4th: " + orderedList.ElementAt(3).name + "\n";
            answer += "5th: " + orderedList.ElementAt(4).name + "\n";


            await msg.Channel.SendMessageAsync(answer);


        }

        #endregion

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
                await message.Channel.SendMessageAsync("Search for Huntress Info via `!huntress <NAME>`.\n" +
                    "Search for Status Info via `!list <STATUS>`.\n" +
                    "Search for a list of huntresses of an element via `!list <ELEMENT>`.\n" +
                    "Search for quick Tier List of a role via `!tier <ROLE>`. Valid roles are `Support`,`Offensive`,`Recovery`,`Shield`\n" +
                    "Search for a detailed Tier List view of one Huntress via `!tier <NAME>`." +
                    "Search for top huntresses in each given stat by `!top <STAT>`. Valid stat name are `Might`,`Atk`,`Def`,`HP` and `Spd`.");
                return;
            }
            //Test all Huntresses command. Just usable by me since it floods channels. Maybe use ID here to future-proof and put it in BotConfig? Im too lazy to look up my ID.
            else if(message.Content.StartsWith(BotConfig.PREFIX+"testall") && message.Author.Username.Equals("Maaster") && message.Author.DiscriminatorValue.Equals(1273))
            {
                //await message.Channel.SendMessageAsync("Test all!");
                await TestPrintAll(message);
                return;
            }
            else if (message.Content.StartsWith(BotConfig.PREFIX + "tier testall") && message.Author.Username.Equals("Maaster") && message.Author.DiscriminatorValue.Equals(1273))
            {
                await TestPrintAllTier(message);
                return;
            }

            //Get command arguments
            string[] cmd = message.Content.Split(' ');
            //Check for empty. Shouldnt happen as we test for it beforehand, but just to be sure.
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
                case "tier":
                    await ProcessTierCommand(message);
                    //await message.Channel.SendMessageAsync("Tier Command disabled for now! We are looking over the last changes, it will be enabled very soon, promise! <3");
                    break;
                case "top":
                    GetStatRanking(message);
                    break;
                default:
                    await message.Channel.SendMessageAsync("Command not found!");
                    //Console.WriteLine("Command not found");
                    break;
            }

        }

        #region Tier List Commands

        private async Task ProcessTierCommand(SocketMessage msg)
        {
            //Check if Role was queried
            string queriedName = msg.Content.Substring(msg.Content.IndexOf(' ') + 1).ToLower();

            if(BotConfig.ROLES.Contains(queriedName.ToLower()))
            {
                foreach(TierList currList in tierListsRoles)
                {
                    if(currList.type.Equals(queriedName, StringComparison.OrdinalIgnoreCase))
                    {
                        await msg.Channel.SendMessageAsync("", false, currList.ToDiscordMessage());
                    }
                }

                return;
            }

            //Get correct huntress by name
            Huntress huntress = await GetNameFromQueryAsync(msg);

            //If not found, user is already informed.
            if (huntress == null)
                return;

            TierData list = null;

            //Find TierData by given name
            foreach (TierData data in tierDataList)
            {
                if (huntress.name.Equals(data.name))
                {
                    list = data;
                    break;
                }
            }

            if(list == null)
            {
                Console.WriteLine("Huntress not found in ProcessTier");
                await msg.Channel.SendMessageAsync("Huntress wasnt found in Tier Data yet! Most likely, this one is not out yet in Global, so we have no knowledge ourselves about her!");
                return;
            }
            await msg.Channel.SendMessageAsync("", false, list.ToDiscordMessage());

        }
        #endregion

        #region List Commands
        private async Task ProcessListCommand(SocketMessage msg)
        {
            string queriedName = msg.Content.Substring(msg.Content.IndexOf(' ') + 1).ToLower();

            if(queriedName.Equals("dispel buff"))
            {
                List<Huntress> matches = new List<Huntress>();
                string ccInfo = "";
                //Search all skills for the status and add them to list
                foreach (Huntress match in huntresses)
                {
                    if ((match.skill1.Contains("buff", StringComparison.OrdinalIgnoreCase) && match.skill1.Contains("dispel", StringComparison.OrdinalIgnoreCase) && !match.skill1.Contains("debuff", StringComparison.OrdinalIgnoreCase))
                        || (match.skill2.Contains("buff", StringComparison.OrdinalIgnoreCase) && match.skill2.Contains("dispel", StringComparison.OrdinalIgnoreCase) && !match.skill2.Contains("debuff", StringComparison.OrdinalIgnoreCase))
                        || (match.passive1.Contains("buff", StringComparison.OrdinalIgnoreCase) && match.passive1.Contains("dispel", StringComparison.OrdinalIgnoreCase) && !match.passive1.Contains("debuff", StringComparison.OrdinalIgnoreCase))
                        || (match.passive2.Contains("buff", StringComparison.OrdinalIgnoreCase) && match.passive2.Contains("dispel", StringComparison.OrdinalIgnoreCase) && !match.passive2.Contains("debuff", StringComparison.OrdinalIgnoreCase))
                        || (match.ee30.Contains("buff", StringComparison.OrdinalIgnoreCase) && match.ee30.Contains("dispel", StringComparison.OrdinalIgnoreCase) && !match.ee30.Contains("debuff", StringComparison.OrdinalIgnoreCase)))
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
                return;
            }

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
                case "shield":
                    ccInfo = StatusInfo.SHIELD;
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
                    || match.passive2.Contains(queriedName, StringComparison.OrdinalIgnoreCase)
                    || match.ee30.Contains(queriedName, StringComparison.OrdinalIgnoreCase))
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
        #endregion

        #region Huntress Info
        private async Task ProcessHuntressCommand(SocketMessage msg)
        {
            Huntress huntress = await GetNameFromQueryAsync(msg);
            if(huntress == null)
                return;



            await SendHuntressDataToServer(msg, huntress);

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

            //Get and set stat rankings
            int mightRank, atkRank, defRank, hpRank, spdRank;

            mightRank = CalculateStatRanking("might", hun);
            atkRank = CalculateStatRanking("atk", hun);
            defRank = CalculateStatRanking("def", hun);
            hpRank = CalculateStatRanking("hp", hun);
            spdRank = CalculateStatRanking("spd", hun);

            hun.SetRanks(mightRank, atkRank, defRank,hpRank,spdRank, huntresses.Count);

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
        #endregion

        #region Name Processing
        private async Task<Huntress> GetNameFromQueryAsync(SocketMessage msg)
        {
            List<Huntress> matches = new List<Huntress>();

            //Get Huntress Name entered
            string queriedName = msg.Content.Substring(msg.Content.IndexOf(' ') + 1).ToLower();

            //Check if empty - if no name entered, IndexOf returns -1 and Substring returns the same string, thus queriedName will be the original message.
            if (String.IsNullOrEmpty(queriedName) || msg.Content.ToLower().Equals(queriedName))
            {
                await msg.Channel.SendMessageAsync("No huntress name entered!");
                return null;
            }

            //Search for Huntress by entered name - complete match of name or nickname
            foreach (Huntress hun in huntresses)
            {
                string hunName = hun.name.ToLower();
                string[] hunNick = hun.nick.ToLower().Split(';');

                //If the name is a complete match or starts with it, use it.
                if (queriedName.Equals(hunName) || hunName.StartsWith(queriedName))
                {
                    matches.Add(hun);
                }
                //Try full name for a simple typo, done by Levenshtein Distance. TODO: Switch to Damerau-Levenshtein Distance.
                else if (LevenshteinDistance.EditDistance(hunName, queriedName) <= BotConfig.LEV_DISTANCE)
                {
                    matches.Add(hun);
                }
                //Full name wasnt a match, try all the nicks, seperated by ;
                else
                {
                    foreach (string nick in hunNick)
                    {
                        if (queriedName.Equals(nick))
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
            else if (matches.Count == 1)
            {
                //await SendHuntressDataToServer(msg, matches.First());
                return matches.First();
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
            return null;
        }

        #endregion

        #region Debug Commands
        private async Task TestPrintAll(SocketMessage msg)
        {
            foreach (Huntress hun in huntresses)
            {
                await SendHuntressDataToServer(msg, hun);
                await Task.Delay(2000);
            }

        }
        private async Task TestPrintAllTier(SocketMessage msg)
        {
            foreach (TierData hun in tierDataList)
            {
                await msg.Channel.SendMessageAsync("", false, hun.ToDiscordMessage());
                await Task.Delay(2000);
            }
        }
        #endregion
    }
}
