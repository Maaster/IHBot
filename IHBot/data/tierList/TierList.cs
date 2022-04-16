using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHBot.data.tierList
{
    public class TierList
    {
        public string type;
        public List<TierListRoleData> tiers;

        public Embed ToDiscordMessage()
        {
            EmbedBuilder builder = new EmbedBuilder();
            //.AddField("Huntress", name + "(" + rarity + ")" + " ; " + type + " ; " + attribute)
            //.AddField("Type", type)
            //.AddField("Attr", attribute)
            builder.WithTitle(type + " Tier List")
                //.WithDescription($"{emotes[2]}")
                
                //TODO: Rework Rarity Emotes on Server - maybe seperate emotes, like :S: :S: :R:?
                //.WithDescription($"{emotes[2]}" + $"{emotes[0]}" + " " + $"{emotes[1]}")
                //.WithThumbnailUrl("https://gamewith.akamaized.net/article_tools/dragongirls/gacha/14_c_i.png")
                .WithColor(Color.Purple)
                //.WithThumbnailUrl($"attachment://{iconName}")
                .WithFooter("Bugs? Typos? Suggestions? Contact Maaster#1273!");

            foreach(TierListRoleData tier in tiers)
            {
                builder.AddField(tier.tier, "**Released:**\n" + tier.released + "\n**Unreleased:**\n" + tier.unreleased);
            }



            return builder.Build();
        }
    }

}
