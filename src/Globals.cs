using System;
using System.Collections.Generic;

namespace ArmoryBot
{
    enum ID : long
    {
        CurrentExpansion = 499, // Shadowlands
        WoWToken = 122284 // WoW Token Item
    }
    enum Namespace : int // https://develop.battle.net/documentation/world-of-warcraft/guides/namespaces
    {
        Profile = 0,
        Static = 1,
        Dynamic = 2
    }
    internal static class Globals
    {
        public const string DiscordConfigPath = @"discord.json";
        public const string BlizzardConfigPath = @"blizzard.json";
        /*
         AchievementItems are entered as follows: (int)Group, (int)Value
        The achievement id is the ID as listed on wowhead/blizz api.
        The group is the group ID (as determined by you) of the achievement. This is used to only display the highest achievement of that group. If you want to always display the achievement, use -1.
        The value is the value of the achievement for within it's group. If 4 achievements are in group 10, only the achievement with the highest value will be displayed. This is to reduce clutter in the achievement readout.
                              <key>            <value>
        Dict Entry Format: {(long)id, new AchievementItem((int)group, (int)value) } 

        For example: If a character has earned Gladiator (group 10, value 3), they also have Duelist/Rival/Challenger (also group 10 but have lower values 2 1 0).
        But you would only want to display Gladiator since it's the most prestigious title.
         */
        public static readonly Dictionary<long, AchievementItem> AchievementsPVE = new Dictionary<long, AchievementItem>()
        {
            // Raid
            {14460, new AchievementItem(10, 0) }, // AOTC CN
            {14461, new AchievementItem(10, 1) }, // CE CN
            {14068, new AchievementItem(11, 0) }, // AOTC NY (bfa)
            {14069, new AchievementItem(11, 1) }, // CE NY (bfa)
            // M+
            {14531, new AchievementItem(20, 0) }, // Shadowlands Keystone Conqueror: Season One
            {14532, new AchievementItem(20, 1) }, // Shadowlands Keystone Master: Season One
            {14144, new AchievementItem(21, 0) }, // Battle for Azeroth Keystone Conqueror: Season Four
            {14145, new AchievementItem(21, 1) } // Battle for Azeroth Keystone Master: Season Four
        };
        public static readonly Dictionary<long, AchievementItem> AchievementsPVP = new Dictionary<long, AchievementItem>()
        {
            // PVP Lifetime
            {2090, new AchievementItem(10, 0) }, // Challenger
            {2093, new AchievementItem(10, 1) }, // Rival
            {2092, new AchievementItem(10, 2) }, // Duelist
            {2091, new AchievementItem(10, 3) }, // Gladiator
            // RBG Lifetime
            // Horde
            {6941, new AchievementItem(-1, 0) }, // Hero of the Horde
            {5356, new AchievementItem(20, 4) }, // High Warlord (2400)
            {5355, new AchievementItem(20, 3) }, // General (2200)
            {5353, new AchievementItem(20, 2) }, // Champion (2000)
            {5352, new AchievementItem(20, 1) }, // Legionnaire (1800)
            {5349, new AchievementItem(20, 0) }, // First Sergeant (1500)
            // Alliance
            {6942, new AchievementItem(-1, 0) }, // Hero of the Alliance
            {5343, new AchievementItem(21, 4) }, // Grand Marshal (2400)
            {5341, new AchievementItem(21, 3) }, // Marshal (2200)
            {5339, new AchievementItem(21, 2) }, // Lieutenant Commander (2000)
            {5337, new AchievementItem(21, 1) }, // Knight-Captain (1800)
            {5334, new AchievementItem(21, 0) }, // Sergeant Major (1500)
            // SL PVP Season 1
            {14686, new AchievementItem(100, 0) }, // Challenger: Shadowlands Season 1
            {14687, new AchievementItem(100, 1) }, // Rival: Shadowlands Season 1
            {14688, new AchievementItem(100, 2) }, // Duelist: Shadowlands Season 1
            {14691, new AchievementItem(100, 3) }, // Elite: Shadowlands Season 1
            {14689, new AchievementItem(100, 4) }, // Gladiator: Shadowlands Season 1
            // BFA PVP Season 4
            {13959, new AchievementItem(101, 0) }, // Challenger: BFA S4
            {13963, new AchievementItem(101, 1) }, // Rival: BFA S4
            {13964, new AchievementItem(101, 2) }, // Duelist: BFA S4
            {13989, new AchievementItem(101, 3) }, // Elite: BFA S4
            {13967, new AchievementItem(101, 4) }  // Gladiator: BFA S4
        };
    }
}