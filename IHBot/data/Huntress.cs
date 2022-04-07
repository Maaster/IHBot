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

        private Emote[] emotes;

        public Embed ToDiscordMessage()
        {

            EmbedBuilder builder = new EmbedBuilder();
            //.AddField("Huntress", name + "(" + rarity + ")" + " ; " + type + " ; " + attribute)
                //.AddField("Type", type)
                //.AddField("Attr", attribute)
            builder.AddField(skill1Name + "(S1)", skill1)
                .AddField(skill2Name + "(US)", skill2)
                .AddField(passive1Name + "(P1)", passive1)
                .AddField(passive2Name + "(P2)", passive2)
                .AddField("EE Level 1", ee1)
                .AddField("EE Level 10", ee10)
                .AddField("EE Level 20", ee20)
                .AddField("EE Level 30-50", ee30)
                .WithTitle(name)
                .WithDescription(rarity + $"{emotes[0]}" + " " + $"{emotes[1]}")
                //.WithThumbnailUrl("https://gamewith.akamaized.net/article_tools/dragongirls/gacha/14_c_i.png")
                .WithFooter("Bugs? Typos? Suggestions? Contact Maaster#1273");

            

            return builder.Build();
        }

        public void setEmotes(Emote[] emotes)
        {
            this.emotes = emotes;
        }
    }
}
