using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHBot.Config
{
    internal class BotConfig
    {
        public static string PREFIX = "!";
        public static int LEV_DISTANCE = 1;
        public static List<String> ELEMENTS = new List<String>() {"earth","wind","fire","water", "light", "dark" };
        public static List<String> CC_NAMES = new List<String>() { "freeze", "stun", "vacant", "flying", "burn","hate","silence","pierce","curse","weak","tear","slow","realm","blind","entwined", "shield"};
        public static List<String> ROLES = new List<String>() {"offensive","support","recovery","shield" };
    }
}
