using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHBot.data
{
    public class Huntress
    {
        //JSON Fields
        public string name;
        public string nick;
        public string type;
        public string attribute;
        public string skill1Name;
        public string skill1;
        public string skill2Name;
        public string skill2;
        public string passive1Name;
        public string passive1;
        public string passive2Name;
        public string passive2;
        public string ee1;
        public string ee10;
        public string ee20;
        public string ee30;
        public bool released;
        public bool limited;
        public string rarity;
        public int might;
        public int hp;
        public int atk;
        public int def;
        public int spd;

        //Private properties
        private Emote[] emotes;
        private string iconName;
        private int mightRank, atkRank, defRank, hpRank, spdRank;
        private int huntressAmount;

        public Embed ToDiscordMessage()
        {
            string strRarity = "`" + rarity + "`";

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle(name + " " + strRarity + " " + $"{emotes[0]}" + " " + $"{emotes[1]}")
                //.WithDescription($"{emotes[2]}")
                .AddField(skill1Name + " (S1)", skill1)
                .AddField(skill2Name + " (US)", skill2)
                .AddField(passive1Name + " (P1)", passive1)
                .AddField(passive2Name + " (P2)", passive2)
                .AddField("EE Level 1", ee1,true)
                .AddField("EE Level 10", ee10,true)
                .AddField("EE Level 20", ee20,true)
                .AddField("EE Level 30-50", ee30)
                //.WithDescription($"{emotes[2]}" + $"{emotes[0]}" + " " + $"{emotes[1]}")
                //.WithThumbnailUrl("https://gamewith.akamaized.net/article_tools/dragongirls/gacha/14_c_i.png")
                .WithColor(Color.Red)
                .WithThumbnailUrl($"attachment://{iconName}")
                .AddField("Stats (at Lv200)", 
                "**Might** " + might + " (" + mightRank + "/" + huntressAmount + ")\n"
                + "**Attack** " + atk + " (" + atkRank + "/" + huntressAmount + ")\n"
                + "**Defense** " + def + " (" + defRank + "/" + huntressAmount + ")\n"
                + "**HP** " + hp + " (" + hpRank + "/" + huntressAmount + ")\n"
                + "**Speed** " + spd + " (" + spdRank + "/" + huntressAmount + ")")
                //.AddField("Might","" + mightRank + "/" + huntressAmount,true)
                //.AddField("Attack", "" + atkRank + "/" + huntressAmount, true)
                //.AddField("Defense", "" + defRank + "/" + huntressAmount, true)
                //.AddField("HP", "" + hpRank + "/" + huntressAmount, true)
                //.AddField("Speed", "" + spdRank + "/" + huntressAmount, true)
                .WithFooter("Bugs? Typos? Suggestions? Contact Maaster#1273!");



            return builder.Build();
        }

        public void setEmotes(Emote[] emotes)
        {
            this.emotes = emotes;
        }

        internal void SetIconName(string iconName)
        {
            this.iconName = iconName.Replace(' ', '_');
        }

        public void SetRanks(int mightRank, int atkRank, int defRank, int hpRank, int spdRank, int amount)
        {
            this.hpRank = hpRank;
            this.mightRank = mightRank;
            this.defRank = defRank;
            this.atkRank = atkRank;
            this.spdRank = spdRank;
            this.huntressAmount = amount;

        }
    }
}