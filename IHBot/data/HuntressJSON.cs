using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHBot.data
{
    public class HuntressJSON
    {
        [JsonProperty("huntresses")]
        public List<Huntress> huntressList { get; set;}
    }
}
