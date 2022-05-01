using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHBot.data
{
    public class TierData
    {
        public string name;
        public string pros;
        public string cons;
        public string pve;
        public string pvp;
        public string bossing;
        public string notes;



        public Embed ToDiscordMessage()
        {
            //string strRarity = "`" + rarity + "`";


            EmbedBuilder builder = new EmbedBuilder();
            //.AddField("Huntress", name + "(" + rarity + ")" + " ; " + type + " ; " + attribute)
            //.AddField("Type", type)
            //.AddField("Attr", attribute)
            //builder.WithTitle(name + " " + strRarity + " " + $"{emotes[0]}" + " " + $"{emotes[1]}")
            builder.WithTitle(name)
                .WithDescription("**Unless otherwise noted, these ratings are based on lv200+ with EE30+ and the unit in a decent composition!**")
                .AddField("Pros", pros)
                .AddField("Cons", cons)
                .AddField("PvE Rating", pve,true)
                .AddField("PvP Rating", pvp, true)
                .AddField("Bossing Rating", bossing, true)
                //.AddField("Notes", notes)
                //TODO: Rework Rarity Emotes on Server - maybe seperate emotes, like :S: :S: :R:?
                //.WithDescription($"{emotes[2]}" + $"{emotes[0]}" + " " + $"{emotes[1]}")
                //.WithThumbnailUrl("https://gamewith.akamaized.net/article_tools/dragongirls/gacha/14_c_i.png")
                .WithColor(Color.Blue)
                //.WithThumbnailUrl($"attachment://{iconName}")
                .WithFooter("Disagree with this or have questions? Join the discussion on IHC! discord.gg/mfEjXvUbtd");


            if (!String.IsNullOrEmpty(notes))
                builder.AddField("Notes", notes);

            return builder.Build();
        }
    }
}
