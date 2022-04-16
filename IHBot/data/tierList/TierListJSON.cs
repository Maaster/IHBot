using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHBot.data.tierList
{
    public class TierListJSON
    {
        [JsonProperty("tierList")]
        public List<TierList> list;


    }
}
