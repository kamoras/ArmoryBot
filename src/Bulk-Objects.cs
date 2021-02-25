using Newtonsoft.Json;
using System;
using System.Numerics;

namespace ArmoryBot
{
    //
    // ** Bulk-generated classes used in program
    //

    // Raid JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/encounters/raids?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class RaidInfo
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("character")]
        public Character Character { get; set; }

        [JsonProperty("expansions")]
        public Expansion[] ExpansionsExpansions { get; set; }
    }

    public partial class Character
    {
        [JsonProperty("key")]
        public Self Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("realm", NullValueHandling = NullValueHandling.Ignore)]
        public Realm Realm { get; set; }
    }

    public partial class Self
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }
    }

    public partial class Realm
    {
        [JsonProperty("key")]
        public Self Key { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }
    }

    public partial class Name
    {
        [JsonProperty("en_US")]
        public string EnUs { get; set; }
        [JsonProperty("es_MX")]
        public string EsMx { get; set; }

        [JsonProperty("pt_BR")]
        public string PtBr { get; set; }

        [JsonProperty("de_DE")]
        public string DeDe { get; set; }

        [JsonProperty("en_GB")]
        public string EnGb { get; set; }

        [JsonProperty("es_ES")]
        public string EsEs { get; set; }

        [JsonProperty("fr_FR")]
        public string FrFr { get; set; }

        [JsonProperty("it_IT")]
        public string ItIt { get; set; }

        public string GetLocale(string locale)
        {
            switch (locale)
            {
                case "en_US":
                    return this.EnUs;
                case "es_MX":
                    return this.EsMx;
                case "pt_BR":
                    return this.PtBr;
                case "de_DE":
                    return this.DeDe;
                case "en_GB":
                    return this.EnGb;
                case "es_ES":
                    return this.EsEs;
                case "fr_FR":
                    return this.FrFr;
                case "it_IT":
                    return this.ItIt;
                default:
                    throw new Exception("Invalid locale specified.");
            }
        }

    }

    public partial class Expansion
    {
        [JsonProperty("expansion")]
        public Character ExpansionExpansion { get; set; }

        [JsonProperty("instances")]
        public Instance[] Instances { get; set; }
    }

    public partial class Instance
    {
        [JsonProperty("instance")]
        public Character InstanceInstance { get; set; }

        [JsonProperty("modes")]
        public Mode[] Modes { get; set; }
    }

    public partial class Mode
    {
        [JsonProperty("difficulty")]
        public Difficulty Difficulty { get; set; }

        [JsonProperty("status")]
        public Difficulty Status { get; set; }

        [JsonProperty("progress")]
        public Progress Progress { get; set; }
    }

    public partial class Difficulty
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }
    }

    public partial class Progress
    {
        [JsonProperty("completed_count")]
        public long CompletedCount { get; set; }

        [JsonProperty("total_count")]
        public long TotalCount { get; set; }

        [JsonProperty("encounters")]
        public Encounter[] Encounters { get; set; }
    }

    public partial class Encounter
    {
        [JsonProperty("encounter")]
        public Character EncounterEncounter { get; set; }

        [JsonProperty("completed_count")]
        public long CompletedCount { get; set; }

        [JsonProperty("last_kill_timestamp")]
        public long LastKillTimestamp { get; set; }
    }

    public partial class Links
    {
        [JsonProperty("self")]
        public Self Self { get; set; }
    }
    // Character Summary JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class CharacterSummary
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("gender")]
        public Faction Gender { get; set; }

        [JsonProperty("faction")]
        public Faction Faction { get; set; }

        [JsonProperty("race")]
        public ActiveSpec Race { get; set; }

        [JsonProperty("character_class")]
        public ActiveSpec CharacterClass { get; set; }

        [JsonProperty("active_spec")]
        public ActiveSpec ActiveSpec { get; set; }

        [JsonProperty("realm")]
        public ActiveSpec Realm { get; set; }

        [JsonProperty("guild")]
        public Guild Guild { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("experience")]
        public long Experience { get; set; }

        [JsonProperty("achievement_points")]
        public long AchievementPoints { get; set; }

        [JsonProperty("achievements")]
        public Achievements Achievements { get; set; }

        [JsonProperty("titles")]
        public Achievements Titles { get; set; }

        [JsonProperty("pvp_summary")]
        public Achievements PvpSummary { get; set; }

        [JsonProperty("encounters")]
        public Achievements Encounters { get; set; }

        [JsonProperty("media")]
        public Achievements Media { get; set; }

        [JsonProperty("last_login_timestamp")]
        public long LastLoginTimestamp { get; set; }

        [JsonProperty("average_item_level")]
        public long AverageItemLevel { get; set; }

        [JsonProperty("equipped_item_level")]
        public long EquippedItemLevel { get; set; }

        [JsonProperty("specializations")]
        public Achievements Specializations { get; set; }

        [JsonProperty("statistics")]
        public Achievements Statistics { get; set; }

        [JsonProperty("mythic_keystone_profile")]
        public Achievements MythicKeystoneProfile { get; set; }

        [JsonProperty("equipment")]
        public Achievements Equipment { get; set; }

        [JsonProperty("appearance")]
        public Achievements Appearance { get; set; }

        [JsonProperty("collections")]
        public Achievements Collections { get; set; }

        [JsonProperty("active_title")]
        public ActiveSpec ActiveTitle { get; set; }

        [JsonProperty("reputations")]
        public Achievements Reputations { get; set; }

        [JsonProperty("quests")]
        public Achievements Quests { get; set; }

        [JsonProperty("achievements_statistics")]
        public Achievements AchievementsStatistics { get; set; }

        [JsonProperty("professions")]
        public Achievements Professions { get; set; }

        [JsonProperty("covenant_progress")]
        public CovenantProgress CovenantProgress { get; set; }
    }

    public partial class Achievements
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }
    }

    public partial class ActiveSpec
    {
        [JsonProperty("key")]
        public Achievements Key { get; set; }

        [JsonProperty("name")]
        public DisplayString Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("display_string", NullValueHandling = NullValueHandling.Ignore)]
        public DisplayString DisplayString { get; set; }

        [JsonProperty("slug", NullValueHandling = NullValueHandling.Ignore)]
        public string Slug { get; set; }
    }

    public partial class DisplayString
    {
        [JsonProperty("en_US")]
        public string EnUs { get; set; }
        [JsonProperty("es_MX")]
        public string EsMx { get; set; }

        [JsonProperty("pt_BR")]
        public string PtBr { get; set; }

        [JsonProperty("de_DE")]
        public string DeDe { get; set; }

        [JsonProperty("en_GB")]
        public string EnGb { get; set; }

        [JsonProperty("es_ES")]
        public string EsEs { get; set; }

        [JsonProperty("fr_FR")]
        public string FrFr { get; set; }

        [JsonProperty("it_IT")]
        public string ItIt { get; set; }
        public string GetLocale(string locale)
        {
            switch (locale)
            {
                case "en_US":
                    return this.EnUs;
                case "es_MX":
                    return this.EsMx;
                case "pt_BR":
                    return this.PtBr;
                case "de_DE":
                    return this.DeDe;
                case "en_GB":
                    return this.EnGb;
                case "es_ES":
                    return this.EsEs;
                case "fr_FR":
                    return this.FrFr;
                case "it_IT":
                    return this.ItIt;
                default:
                    throw new Exception("Invalid locale specified.");
            }
        }

    }

    public partial class CovenantProgress
    {
        [JsonProperty("chosen_covenant")]
        public ActiveSpec ChosenCovenant { get; set; }

        [JsonProperty("renown_level")]
        public long RenownLevel { get; set; }

        [JsonProperty("soulbinds")]
        public Achievements Soulbinds { get; set; }
    }

    public partial class Faction
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public DisplayString Name { get; set; }
    }

    public partial class Guild
    {
        [JsonProperty("key")]
        public Achievements Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("realm")]
        public ActiveSpec Realm { get; set; }

        [JsonProperty("faction")]
        public Faction Faction { get; set; }
    }

    public partial class Links
    {
        //[JsonProperty("self")]
        //public Achievements Self { get; set; }
    }

    // Character Media JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/character-media?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class CharacterMedia
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("character")]
        public Character Character { get; set; }

        [JsonProperty("assets")]
        public Asset[] Assets { get; set; }
    }

    public partial class Asset
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public Uri Value { get; set; }
    }

    public partial class Character
    {
        //[JsonProperty("key")]
        //public Self Key { get; set; }

        //[JsonProperty("name")]
        //public string Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("realm")]
        //public Realm Realm { get; set; }
    }

    public partial class Self
    {
        //[JsonProperty("href")]
        //public Uri Href { get; set; }
    }

    public partial class Realm
    {
        //[JsonProperty("key")]
        //public Self Key { get; set; }

        //[JsonProperty("name")]
        //public Name Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("slug")]
        //public string Slug { get; set; }
    }

    public partial class Name
    {
        //[JsonProperty("en_US")]
        //public string EnUs { get; set; }

    }

    public partial class Links
    {
        //[JsonProperty("self")]
        //public Self Self { get; set; }
    }
    // Mythic+ JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/mythic-keystone-profile?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class MythicPlusInfo
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("current_period")]
        public CurrentPeriod CurrentPeriod { get; set; }

        [JsonProperty("seasons")]
        public Period[] Seasons { get; set; }

        [JsonProperty("character")]
        public MythicPlusInfoCharacter Character { get; set; }
    }

    public partial class MythicPlusInfoCharacter
    {
        [JsonProperty("key")]
        public Self Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("realm")]
        public Realm Realm { get; set; }
    }

    public partial class Self
    {
        //[JsonProperty("href")]
        //public Uri Href { get; set; }
    }

    public partial class Realm
    {
        //[JsonProperty("key")]
        //public Self Key { get; set; }

        //[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        //public Name Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("slug", NullValueHandling = NullValueHandling.Ignore)]
        //public string Slug { get; set; }
    }

    public partial class Name
    {
        //[JsonProperty("en_US")]
        //public string EnUs { get; set; }

    }

    public partial class CurrentPeriod
    {
        [JsonProperty("period")]
        public Period Period { get; set; }

        [JsonProperty("best_runs")]
        public BestRun[] BestRuns { get; set; }
    }

    public partial class BestRun
    {
        [JsonProperty("completed_timestamp")]
        public long CompletedTimestamp { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("keystone_level")]
        public long KeystoneLevel { get; set; }

        [JsonProperty("keystone_affixes")]
        public Realm[] KeystoneAffixes { get; set; }

        [JsonProperty("members")]
        public Member[] Members { get; set; }

        [JsonProperty("dungeon")]
        public Realm Dungeon { get; set; }

        [JsonProperty("is_completed_within_time")]
        public bool IsCompletedWithinTime { get; set; }
    }

    public partial class Member
    {
        [JsonProperty("character")]
        public MemberCharacter Character { get; set; }

        [JsonProperty("specialization")]
        public Realm Specialization { get; set; }

        [JsonProperty("race")]
        public Realm Race { get; set; }

        [JsonProperty("equipped_item_level")]
        public long EquippedItemLevel { get; set; }
    }

    public partial class MemberCharacter
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("realm")]
        public Realm Realm { get; set; }
    }

    public partial class Period
    {
        [JsonProperty("key")]
        public Self Key { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class Links
    {
        //[JsonProperty("self")]
        //public Self Self { get; set; }
    }
    // Mythic+ Seasonal JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/mythic-keystone-profile/season/{seasonid}?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class MythicPlusSeasonInfo
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("season")]
        public Season Season { get; set; }

        [JsonProperty("best_runs")]
        public BestRun[] BestRuns { get; set; }

        [JsonProperty("character")]
        public MythicPlusSeasonInfoCharacter Character { get; set; }
    }

    public partial class BestRun
    {
        //[JsonProperty("completed_timestamp")]
        //public long CompletedTimestamp { get; set; }

        //[JsonProperty("duration")]
        //public long Duration { get; set; }

        //[JsonProperty("keystone_level")]
        //public long KeystoneLevel { get; set; }

        //[JsonProperty("keystone_affixes")]
        //public Dungeon[] KeystoneAffixes { get; set; }

        //[JsonProperty("members")]
        //public Member[] Members { get; set; }

        //[JsonProperty("dungeon")]
        //public Dungeon Dungeon { get; set; }

        //[JsonProperty("is_completed_within_time")]
        //public bool IsCompletedWithinTime { get; set; }
    }

    public partial class Dungeon
    {
        [JsonProperty("key")]
        public Self Key { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class Self
    {
        //[JsonProperty("href")]
        //public Uri Href { get; set; }
    }

    public partial class Name
    {
        //[JsonProperty("en_US")]
        //public string EnUs { get; set; }
    }

    public partial class Member
    {
        //[JsonProperty("character")]
        //public MemberCharacter Character { get; set; }

        //[JsonProperty("specialization")]
        //public Dungeon Specialization { get; set; }

        //[JsonProperty("race")]
        //public Dungeon Race { get; set; }

        //[JsonProperty("equipped_item_level")]
        //public long EquippedItemLevel { get; set; }
    }

    public partial class MemberCharacter
    {
        //[JsonProperty("name")]
        //public string Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("realm")]
        //public Realm Realm { get; set; }
    }

    public partial class Realm
    {
        //[JsonProperty("key")]
        //public Self Key { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("slug")]
        //public string Slug { get; set; }

        //[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        //public Name Name { get; set; }
    }

    public partial class MythicPlusSeasonInfoCharacter
    {
        [JsonProperty("key")]
        public Self Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("realm")]
        public Realm Realm { get; set; }
    }

    public partial class Links
    {
        //[JsonProperty("self")]
        //public Self Self { get; set; }
    }

    public partial class Season
    {
        [JsonProperty("key")]
        public Self Key { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }
    // PVP Bracket JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/pvp-bracket/{bracket}?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class PvpBracketInfo
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("character")]
        public Character Character { get; set; }

        [JsonProperty("faction")]
        public Faction Faction { get; set; }

        [JsonProperty("bracket")]
        public Bracket Bracket { get; set; }

        [JsonProperty("rating")]
        public long Rating { get; set; }

        [JsonProperty("season")]
        public Season Season { get; set; }

        [JsonProperty("tier")]
        public Season Tier { get; set; }

        [JsonProperty("season_match_statistics")]
        public MatchStatistics SeasonMatchStatistics { get; set; }

        [JsonProperty("weekly_match_statistics")]
        public MatchStatistics WeeklyMatchStatistics { get; set; }
    }

    public partial class Bracket
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Character
    {
        //[JsonProperty("key")]
        //public Self Key { get; set; }

        //[JsonProperty("name")]
        //public string Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("realm")]
        //public Realm Realm { get; set; }
    }

    public partial class Self
    {
        //[JsonProperty("href")]
        //public Uri Href { get; set; }
    }

    public partial class Realm
    {
        //[JsonProperty("key")]
        //public Self Key { get; set; }

        //[JsonProperty("name")]
        //public Name Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("slug")]
        //public string Slug { get; set; }
    }

    public partial class Name
    {
        //[JsonProperty("en_US")]
        //public string EnUs { get; set; }
    }

    public partial class Faction
    {
        //[JsonProperty("type")]
        //public string Type { get; set; }

        //[JsonProperty("name")]
        //public Name Name { get; set; }
    }

    public partial class Links
    {
        //[JsonProperty("self")]
        //public Self Self { get; set; }
    }

    public partial class Season
    {
        //[JsonProperty("key")]
        //public Self Key { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }
    }

    public partial class MatchStatistics
    {
        [JsonProperty("played")]
        public long Played { get; set; }

        [JsonProperty("won")]
        public long Won { get; set; }

        [JsonProperty("lost")]
        public long Lost { get; set; }
    }
    // Achievement Summary JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/achievements?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class AchievementSummary
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("total_quantity")]
        public long TotalQuantity { get; set; }

        [JsonProperty("total_points")]
        public long TotalPoints { get; set; }

        [JsonProperty("achievements")]
        public Achievement[] Achievements { get; set; }

        [JsonProperty("category_progress")]
        public CategoryProgress[] CategoryProgress { get; set; }

        [JsonProperty("recent_events")]
        public RecentEvent[] RecentEvents { get; set; }

        [JsonProperty("character")]
        public Character Character { get; set; }

        [JsonProperty("statistics")]
        public Statistics Statistics { get; set; }
    }

    public partial class Achievement
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("achievement")]
        public Character AchievementAchievement { get; set; }

        [JsonProperty("criteria", NullValueHandling = NullValueHandling.Ignore)]
        public Criteria Criteria { get; set; }

        [JsonProperty("completed_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public long? CompletedTimestamp { get; set; }
    }

    public partial class Character
    {
        //[JsonProperty("key")]
        //public Statistics Key { get; set; }

        //[JsonProperty("name")]
        //public string Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("realm", NullValueHandling = NullValueHandling.Ignore)]
        //public Realm Realm { get; set; }
    }

    public partial class Statistics
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }
    }

    public partial class Realm
    {
        //[JsonProperty("key")]
        //public Statistics Key { get; set; }

        //[JsonProperty("name")]
        //public Name Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("slug")]
        //public string Slug { get; set; }
    }

    public partial class Name
    {
        //[JsonProperty("en_US")]
        //public string EnUs { get; set; }
    }

    public partial class Criteria
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_completed")]
        public bool IsCompleted { get; set; }

        [JsonProperty("child_criteria", NullValueHandling = NullValueHandling.Ignore)]
        public Criteria[] ChildCriteria { get; set; }

        [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
        public BigInteger Amount { get; set; }
    }

    public partial class CategoryProgress
    {
        [JsonProperty("category")]
        public Character Category { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("points")]
        public long Points { get; set; }
    }

    public partial class Links
    {
        //[JsonProperty("self")]
        //public Statistics Self { get; set; }
    }

    public partial class RecentEvent
    {
        [JsonProperty("achievement")]
        public Character Achievement { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }
    // Mythic Plus Season Index JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/data/wow/mythic-keystone/season/index?namespace=dynamic-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class MPlusSeasonIndex
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("seasons")]
        public Season[] Seasons { get; set; }

        [JsonProperty("current_season")]
        public Season CurrentSeason { get; set; }
    }

    public partial class Season
    {
        //[JsonProperty("key")]
        //public Self Key { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }
    }

    public partial class Self
    {
        //[JsonProperty("href")]
        //public Uri Href { get; set; }
    }

    public partial class Links
    {
        //[JsonProperty("self")]
        //public Self Self { get; set; }
    }
    // Character Stats JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/statistics?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class CharacterStatsInfo
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("health")]
        public long Health { get; set; }

        [JsonProperty("power")]
        public long Power { get; set; }

        [JsonProperty("power_type")]
        public PowerType PowerType { get; set; }

        [JsonProperty("speed")]
        public Avoidance Speed { get; set; }

        [JsonProperty("strength")]
        public Agility Strength { get; set; }

        [JsonProperty("agility")]
        public Agility Agility { get; set; }

        [JsonProperty("intellect")]
        public Agility Intellect { get; set; }

        [JsonProperty("stamina")]
        public Agility Stamina { get; set; }

        [JsonProperty("melee_crit")]
        public Block MeleeCrit { get; set; }

        [JsonProperty("melee_haste")]
        public Block MeleeHaste { get; set; }

        [JsonProperty("mastery")]
        public Block Mastery { get; set; }

        [JsonProperty("bonus_armor")]
        public long BonusArmor { get; set; }

        [JsonProperty("lifesteal")]
        public Block Lifesteal { get; set; }

        [JsonProperty("versatility")]
        public long Versatility { get; set; }

        [JsonProperty("versatility_damage_done_bonus")]
        public double VersatilityDamageDoneBonus { get; set; }

        [JsonProperty("versatility_healing_done_bonus")]
        public double VersatilityHealingDoneBonus { get; set; }

        [JsonProperty("versatility_damage_taken_bonus")]
        public double VersatilityDamageTakenBonus { get; set; }

        [JsonProperty("avoidance")]
        public Avoidance Avoidance { get; set; }

        [JsonProperty("attack_power")]
        public long AttackPower { get; set; }

        [JsonProperty("main_hand_damage_min")]
        public double MainHandDamageMin { get; set; }

        [JsonProperty("main_hand_damage_max")]
        public double MainHandDamageMax { get; set; }

        [JsonProperty("main_hand_speed")]
        public double MainHandSpeed { get; set; }

        [JsonProperty("main_hand_dps")]
        public double MainHandDps { get; set; }

        [JsonProperty("off_hand_damage_min")]
        public long OffHandDamageMin { get; set; }

        [JsonProperty("off_hand_damage_max")]
        public long OffHandDamageMax { get; set; }

        [JsonProperty("off_hand_speed")]
        public double OffHandSpeed { get; set; }

        [JsonProperty("off_hand_dps")]
        public long OffHandDps { get; set; }

        [JsonProperty("spell_power")]
        public long SpellPower { get; set; }

        [JsonProperty("spell_penetration")]
        public long SpellPenetration { get; set; }

        [JsonProperty("spell_crit")]
        public Block SpellCrit { get; set; }

        [JsonProperty("mana_regen")]
        public long ManaRegen { get; set; }

        [JsonProperty("mana_regen_combat")]
        public long ManaRegenCombat { get; set; }

        [JsonProperty("armor")]
        public Agility Armor { get; set; }

        [JsonProperty("dodge")]
        public Block Dodge { get; set; }

        [JsonProperty("parry")]
        public Block Parry { get; set; }

        [JsonProperty("block")]
        public Block Block { get; set; }

        [JsonProperty("ranged_crit")]
        public Block RangedCrit { get; set; }

        [JsonProperty("ranged_haste")]
        public Block RangedHaste { get; set; }

        [JsonProperty("spell_haste")]
        public Block SpellHaste { get; set; }

        [JsonProperty("character")]
        public Character Character { get; set; }
    }

    public partial class Agility
    {
        [JsonProperty("base")]
        public long Base { get; set; }

        [JsonProperty("effective")]
        public long Effective { get; set; }
    }

    public partial class Avoidance
    {
        [JsonProperty("rating")]
        public long Rating { get; set; }

        [JsonProperty("rating_bonus")]
        public long RatingBonus { get; set; }
    }

    public partial class Block
    {
        [JsonProperty("rating")]
        public long Rating { get; set; }

        [JsonProperty("rating_bonus")]
        public double RatingBonus { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }

    public partial class Character
    {
        //[JsonProperty("key")]
        //public Self Key { get; set; }

        //[JsonProperty("name")]
        //public string Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("realm")]
        //public PowerType Realm { get; set; }
    }

    public partial class Self
    {
        //[JsonProperty("href")]
        //public Uri Href { get; set; }
    }

    public partial class PowerType
    {
        [JsonProperty("key")]
        public Self Key { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("slug", NullValueHandling = NullValueHandling.Ignore)]
        public string Slug { get; set; }
    }

    public partial class Name
    {
        //[JsonProperty("en_US")]
        //public string EnUs { get; set; }

        //[JsonProperty("es_MX")]
        //public string EsMx { get; set; }

        //[JsonProperty("pt_BR")]
        //public string PtBr { get; set; }

        //[JsonProperty("de_DE")]
        //public string DeDe { get; set; }

        //[JsonProperty("en_GB")]
        //public string EnGb { get; set; }

        //[JsonProperty("es_ES")]
        //public string EsEs { get; set; }

        //[JsonProperty("fr_FR")]
        //public string FrFr { get; set; }

        //[JsonProperty("it_IT")]
        //public string ItIt { get; set; }

    }

    public partial class Links
    {
        //[JsonProperty("self")]
        //public Self Self { get; set; }
    }
}
