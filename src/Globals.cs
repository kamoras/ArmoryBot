using System;
using System.Collections.Generic;

namespace ArmoryBot
{
    public enum ID : long
    {
        CurrentExpansion = 499, // Shadowlands
    }
    public enum Namespace // https://develop.battle.net/documentation/world-of-warcraft/guides/namespaces
    {
        Profile,
        Static,
        Dynamic
    }

    public enum LookupType
    {
        PVE,
        PVP
    }
    internal static class Globals
    {
        public const string QuestionMarkIconUrl = @"https://render-us.worldofwarcraft.com/icons/56/inv_misc_questionmark.jpg";
        public const string TomeIconUrl = @"https://render-us.worldofwarcraft.com/icons/56/inv_misc_book_07.jpg";
        public const string WowTokenIconUrl = @"https://render-us.worldofwarcraft.com/icons/56/wow_token01.jpg";

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
            // Raid Overall
            {14460, new AchievementItem(10, 0) }, // Ahead of the Curve: Sire Denathrius
            {14461, new AchievementItem(10, 1) }, // Cutting Edge: Sire Denathrius
            {15134, new AchievementItem(11, 0) }, // Ahead of the Curve: Sylvanas Windrunner
            {15135, new AchievementItem(11, 1) }, // Cutting Edge: Sylvanas Windrunner
            // Realm Firsts
            {14662, new AchievementItem(-1, 0) }, // Realm First! Shadowlands Keystone Master
            {13078, new AchievementItem(-1, 0) }, // Realm First! Battle for Azeroth Keystone Master
            {11224, new AchievementItem(-1, 0) }, // Realm First! Legion Keystone Master
            // Keystone Hero
            {15051, new AchievementItem(-1, 0) }, // Keystone Hero: De Other Side
            {15048, new AchievementItem(-1, 0) }, // Keystone Hero: Halls of Atonement
            {15047, new AchievementItem(-1, 0) }, // Keystone Hero: Mists of Tirna Scithe
            {15046, new AchievementItem(-1, 0) }, // Keystone Hero: Plaguefall
            {15052, new AchievementItem(-1, 0) }, // Keystone Hero: Sanguine Depths
            {15049, new AchievementItem(-1, 0) }, // Keystone Hero: Spires of Ascension
            {15045, new AchievementItem(-1, 0) }, // Keystone Hero: The Necrotic Wake
            {15050, new AchievementItem(-1, 0) }, // Keystone Hero: Theater of Pain
            // Season 1
            {14531, new AchievementItem(100, 0) }, // Shadowlands Keystone Conqueror: Season One
            {14532, new AchievementItem(100, 1) }, // Shadowlands Keystone Master: Season One
            // Season 2
            {15077, new AchievementItem(101, 0) }, // Shadowlands Keystone Conqueror: Season Two
            {15078, new AchievementItem(101, 1) }, // Shadowlands Keystone Master: Season Two
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
            {6941, new AchievementItem(20, 5) }, // Hero of the Horde (top .5%)
            {5356, new AchievementItem(20, 4) }, // High Warlord (2400)
            {5355, new AchievementItem(20, 3) }, // General (2200)
            {5353, new AchievementItem(20, 2) }, // Champion (2000)
            {5352, new AchievementItem(20, 1) }, // Legionnaire (1800)
            {5349, new AchievementItem(20, 0) }, // First Sergeant (1500)
            // Alliance
            {6942, new AchievementItem(21, 5) }, // Hero of the Alliance (top .5%)
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
            {14690, new AchievementItem(100, 5) }, // Sinful Gladiator: Shadowlands Season 1
            // SL PVP Season 2
            {14969, new AchievementItem(101, 0) }, // Challenger: Shadowlands Season 2
            {14970, new AchievementItem(101, 1) }, // Rival: Shadowlands Season 2
            {14971, new AchievementItem(101, 2) }, // Duelist: Shadowlands Season 2
            {14974, new AchievementItem(101, 3) }, // Elite: Shadowlands Season 2
            {14972, new AchievementItem(101, 4) }, // Gladiator: Shadowlands Season 2
            {14973, new AchievementItem(101, 5) }, // Unchained Gladiator: Shadowlands Season 2
        };
    }
}