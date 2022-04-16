using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHBot.data
{
    internal class TierDataJSON
    {
        [JsonProperty("tierData")]
        public List<TierData> dataList { get; set; }

    }
}
