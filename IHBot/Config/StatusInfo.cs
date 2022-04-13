using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHBot.Config
{
    internal class StatusInfo
    {
        public static string FREEZE = "<Frost Mark> When the same target 3 stacks of Frost Mark, they will be frozen. Frozen targets are unable to take any action or trigger some of their Passive Skills for 2 turns.";
        public static string STUN = "<Stun> Unable to take any action or trigger some Passive Skills.";
        public static string VACANT = "<Vacant Position> When there aren't any units or dead huntresses in a given position on your side.";
        public static string FLYING = "<Send Flying> Targets sent flying take an additional 70% of the caster's ATK in True DMG.";
        public static string BURN = "<Burn Mark> Each stack of Burn Mark makes the target lose 3% of their Max HP at the start of each turn. Stacks up to 10 times, last until the end of battle.";
        public static string HATE = "<Hate Mark> Each stack of Hate Mark lowers the targets Tenacity by 5% for 2 turns. Each target can bear up to 5 stacks of Hate Mark.";
        public static string SILENCE = "<Silence> Unable to us Active or Ultimate Skills.";
        public static string PIERCE = "<Armor Pierce> Lowers the target's DEF by 15%."; //"armor-piercing"
        public static string CURSE = "<Curse Mark> After 3 turns, each Curse stack deals 90% of the caster's ATK in damage.";
        public static string WEAK = "<Weaken> Lowers the target's ATK by 15%.";
        public static string TEAR = "<Tear> Before each action, an additional 4% of the caster's ATK in taken in damage.";
        public static string SLOW = "<Slowed> Speed decreased by 10.";
        public static string REALM = "<Realm Skill> When multiple realms have been cast in the battle, the realm that will occupy the battlefield will be based on the Might of the two teams vying to expand the realms. The team with the lower Might will have their realm covered by the team with the higher Might.";
        public static string BLIND = "<Blinded> Decreases the target's Hit by 30%.";
        public static string ENTWINED = "<Entwined> Unable to any action or dodge for 2 turns.";
        public static string SHIELD = "Gives the Huntress (or all allies) a shield, the strength being a percentage of her ATK.";

    }
}
