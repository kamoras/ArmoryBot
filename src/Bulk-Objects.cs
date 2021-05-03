using System;
using System.Numerics;
using System.Text.Json.Serialization;

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
    public partial class RaidJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("character")]
        public Character Character { get; set; }

        [JsonPropertyName("expansions")]
        public Expansion[] ExpansionsExpansions { get; set; }
    }

    public partial class Character
    {
        [JsonPropertyName("key")]
        public Self Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        //[JsonPropertyName("realm")]
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //public Realm Realm { get; set; }
    }

    public partial class Self
    {
        [JsonPropertyName("href")]
        public Uri Href { get; set; }
    }

    public partial class Realm
    {
        [JsonPropertyName("key")]
        public Self Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }

    public partial class Expansion
    {
        [JsonPropertyName("expansion")]
        public Character ExpansionExpansion { get; set; }

        [JsonPropertyName("instances")]
        public Instance[] Instances { get; set; }
    }

    public partial class Instance
    {
        [JsonPropertyName("instance")]
        public Character InstanceInstance { get; set; }

        [JsonPropertyName("modes")]
        public Mode[] Modes { get; set; }
    }

    public partial class Mode
    {
        [JsonPropertyName("difficulty")]
        public Difficulty Difficulty { get; set; }

        [JsonPropertyName("status")]
        public Difficulty Status { get; set; }

        [JsonPropertyName("progress")]
        public Progress Progress { get; set; }
    }

    public partial class Difficulty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public partial class Progress
    {
        [JsonPropertyName("completed_count")]
        public long CompletedCount { get; set; }

        [JsonPropertyName("total_count")]
        public long TotalCount { get; set; }

        [JsonPropertyName("encounters")]
        public Encounter[] Encounters { get; set; }
    }

    public partial class Encounter
    {
        [JsonPropertyName("encounter")]
        public Character EncounterEncounter { get; set; }

        [JsonPropertyName("completed_count")]
        public long CompletedCount { get; set; }

        [JsonPropertyName("last_kill_timestamp")]
        public long LastKillTimestamp { get; set; }
    }

    public partial class Links
    {
        [JsonPropertyName("self")]
        public Self Self { get; set; }
    }
    // Character Summary JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class CharacterSummaryJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        //[JsonPropertyName("gender")]
        //public Faction Gender { get; set; }

        //[JsonPropertyName("faction")]
        //public Faction Faction { get; set; }

        [JsonPropertyName("race")]
        public ActiveSpec Race { get; set; }

        [JsonPropertyName("character_class")]
        public ActiveSpec CharacterClass { get; set; }

        [JsonPropertyName("active_spec")]
        public ActiveSpec ActiveSpec { get; set; }

        //[JsonPropertyName("realm")]
        //public ActiveSpec Realm { get; set; }

        //[JsonPropertyName("guild")]
        //public Guild Guild { get; set; }

        [JsonPropertyName("level")]
        public long Level { get; set; }

        //[JsonPropertyName("experience")]
        //public long Experience { get; set; }

        //[JsonPropertyName("achievement_points")]
        //public long AchievementPoints { get; set; }

        //[JsonPropertyName("achievements")]
        //public Achievements Achievements { get; set; }

        //[JsonPropertyName("titles")]
        //public Achievements Titles { get; set; }

        //[JsonPropertyName("pvp_summary")]
        //public Achievements PvpSummary { get; set; }

        //[JsonPropertyName("encounters")]
        //public Achievements Encounters { get; set; }

        //[JsonPropertyName("media")]
        //public Achievements Media { get; set; }

        //[JsonPropertyName("last_login_timestamp")]
        //public long LastLoginTimestamp { get; set; }

        //[JsonPropertyName("average_item_level")]
        //public long AverageItemLevel { get; set; }

        [JsonPropertyName("equipped_item_level")]
        public long EquippedItemLevel { get; set; }

        //[JsonPropertyName("specializations")]
        //public Achievements Specializations { get; set; }

        //[JsonPropertyName("statistics")]
        //public Achievements Statistics { get; set; }

        //[JsonPropertyName("mythic_keystone_profile")]
        //public Achievements MythicKeystoneProfile { get; set; }

        //[JsonPropertyName("equipment")]
        //public Achievements Equipment { get; set; }

        //[JsonPropertyName("appearance")]
        //public Achievements Appearance { get; set; }

        //[JsonPropertyName("collections")]
        //public Achievements Collections { get; set; }

        //[JsonPropertyName("active_title")]
        //public ActiveSpec ActiveTitle { get; set; }

        //[JsonPropertyName("reputations")]
        //public Achievements Reputations { get; set; }

        //[JsonPropertyName("quests")]
        //public Achievements Quests { get; set; }

        //[JsonPropertyName("achievements_statistics")]
        //public Achievements AchievementsStatistics { get; set; }

        //[JsonPropertyName("professions")]
        //public Achievements Professions { get; set; }

        [JsonPropertyName("covenant_progress")]
        public CovenantProgress CovenantProgress { get; set; }
    }

    public partial class Achievements
    {
        [JsonPropertyName("href")]
        public Uri Href { get; set; }
    }

    public partial class ActiveSpec
    {
        [JsonPropertyName("key")]
        public Achievements Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("display_string")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string DisplayString { get; set; }

        [JsonPropertyName("slug")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Slug { get; set; }
    }

    public partial class CovenantProgress
    {
        [JsonPropertyName("chosen_covenant")]
        public ActiveSpec ChosenCovenant { get; set; }

        [JsonPropertyName("renown_level")]
        public long RenownLevel { get; set; }

        [JsonPropertyName("soulbinds")]
        public Achievements Soulbinds { get; set; }
    }

    public partial class Faction
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public partial class Guild
    {
        [JsonPropertyName("key")]
        public Achievements Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("realm")]
        public ActiveSpec Realm { get; set; }

        [JsonPropertyName("faction")]
        public Faction Faction { get; set; }
    }

    public partial class Links
    {
        //[JsonPropertyName("self")]
        //public Achievements Self { get; set; }
    }

    // Character Media JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/character-media?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class CharacterMediaJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("character")]
        public Character Character { get; set; }

        [JsonPropertyName("assets")]
        public Asset[] Assets { get; set; }
    }

    public partial class Asset
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public Uri Value { get; set; }
    }

    public partial class Character
    {
        //[JsonPropertyName("key")]
        //public Self Key { get; set; }

        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("realm")]
        //public Realm Realm { get; set; }
    }

    public partial class Self
    {
        //[JsonPropertyName("href")]
        //public Uri Href { get; set; }
    }

    public partial class Realm
    {
        //[JsonPropertyName("key")]
        //public Self Key { get; set; }

        //[JsonPropertyName("name")]
        //public Name Name { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("slug")]
        //public string Slug { get; set; }
    }

    public partial class Links
    {
        //[JsonPropertyName("self")]
        //public Self Self { get; set; }
    }
    // Mythic+ JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/mythic-keystone-profile?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class MythicPlusJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("current_period")]
        public CurrentPeriod CurrentPeriod { get; set; }

        [JsonPropertyName("seasons")]
        public Period[] Seasons { get; set; }

        [JsonPropertyName("character")]
        public MythicPlusInfoCharacter Character { get; set; }
    }

    public partial class MythicPlusInfoCharacter
    {
        [JsonPropertyName("key")]
        public Self Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("realm")]
        public Realm Realm { get; set; }
    }

    public partial class Self
    {
        //[JsonPropertyName("href")]
        //public Uri Href { get; set; }
    }

    public partial class Realm
    {
        //[JsonPropertyName("key")]
        //public Self Key { get; set; }

        //[JsonPropertyName("name", NullValueHandling = NullValueHandling.Ignore)]
        //public Name Name { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("slug", NullValueHandling = NullValueHandling.Ignore)]
        //public string Slug { get; set; }
    }


    public partial class CurrentPeriod
    {
        [JsonPropertyName("period")]
        public Period Period { get; set; }

        [JsonPropertyName("best_runs")]
        public BestRun[] BestRuns { get; set; }
    }

    public partial class BestRun
    {
        [JsonPropertyName("completed_timestamp")]
        public long CompletedTimestamp { get; set; }

        [JsonPropertyName("duration")]
        public long Duration { get; set; }

        [JsonPropertyName("keystone_level")]
        public long KeystoneLevel { get; set; }

        [JsonPropertyName("keystone_affixes")]
        public Realm[] KeystoneAffixes { get; set; }

        [JsonPropertyName("members")]
        public Member[] Members { get; set; }

        [JsonPropertyName("dungeon")]
        public Realm Dungeon { get; set; }

        [JsonPropertyName("is_completed_within_time")]
        public bool IsCompletedWithinTime { get; set; }
    }

    public partial class Member
    {
        [JsonPropertyName("character")]
        public MemberCharacter Character { get; set; }

        [JsonPropertyName("specialization")]
        public Realm Specialization { get; set; }

        [JsonPropertyName("race")]
        public Realm Race { get; set; }

        [JsonPropertyName("equipped_item_level")]
        public long EquippedItemLevel { get; set; }
    }

    public partial class MemberCharacter
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("realm")]
        public Realm Realm { get; set; }
    }

    public partial class Period
    {
        [JsonPropertyName("key")]
        public Self Key { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    public partial class Links
    {
        //[JsonPropertyName("self")]
        //public Self Self { get; set; }
    }
    // Mythic+ Seasonal JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/mythic-keystone-profile/season/{seasonid}?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class MPlusSeasonInfoJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("season")]
        public Season Season { get; set; }

        [JsonPropertyName("best_runs")]
        public BestRun[] BestRuns { get; set; }

        [JsonPropertyName("character")]
        public MythicPlusSeasonInfoCharacter Character { get; set; }
    }

    public partial class BestRun
    {
        //[JsonPropertyName("completed_timestamp")]
        //public long CompletedTimestamp { get; set; }

        //[JsonPropertyName("duration")]
        //public long Duration { get; set; }

        //[JsonPropertyName("keystone_level")]
        //public long KeystoneLevel { get; set; }

        //[JsonPropertyName("keystone_affixes")]
        //public Dungeon[] KeystoneAffixes { get; set; }

        //[JsonPropertyName("members")]
        //public Member[] Members { get; set; }

        //[JsonPropertyName("dungeon")]
        //public Dungeon Dungeon { get; set; }

        //[JsonPropertyName("is_completed_within_time")]
        //public bool IsCompletedWithinTime { get; set; }
    }

    public partial class Dungeon
    {
        [JsonPropertyName("key")]
        public Self Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    public partial class Self
    {
        //[JsonPropertyName("href")]
        //public Uri Href { get; set; }
    }


    public partial class Member
    {
        //[JsonPropertyName("character")]
        //public MemberCharacter Character { get; set; }

        //[JsonPropertyName("specialization")]
        //public Dungeon Specialization { get; set; }

        //[JsonPropertyName("race")]
        //public Dungeon Race { get; set; }

        //[JsonPropertyName("equipped_item_level")]
        //public long EquippedItemLevel { get; set; }
    }

    public partial class MemberCharacter
    {
        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("realm")]
        //public Realm Realm { get; set; }
    }

    public partial class Realm
    {
        //[JsonPropertyName("key")]
        //public Self Key { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("slug")]
        //public string Slug { get; set; }

        //[JsonPropertyName("name", NullValueHandling = NullValueHandling.Ignore)]
        //public Name Name { get; set; }
    }

    public partial class MythicPlusSeasonInfoCharacter
    {
        [JsonPropertyName("key")]
        public Self Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("realm")]
        public Realm Realm { get; set; }
    }

    public partial class Links
    {
        //[JsonPropertyName("self")]
        //public Self Self { get; set; }
    }

    public partial class Season
    {
        [JsonPropertyName("key")]
        public Self Key { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
    // PVP Bracket JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/pvp-bracket/{bracket}?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class PvpBracketInfo
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("character")]
        public Character Character { get; set; }

        [JsonPropertyName("faction")]
        public Faction Faction { get; set; }

        [JsonPropertyName("bracket")]
        public Bracket Bracket { get; set; }

        [JsonPropertyName("rating")]
        public long Rating { get; set; }

        [JsonPropertyName("season")]
        public Season Season { get; set; }

        [JsonPropertyName("tier")]
        public Season Tier { get; set; }

        [JsonPropertyName("season_match_statistics")]
        public MatchStatistics SeasonMatchStatistics { get; set; }

        [JsonPropertyName("weekly_match_statistics")]
        public MatchStatistics WeeklyMatchStatistics { get; set; }
    }

    public partial class Bracket
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public partial class Character
    {
        //[JsonPropertyName("key")]
        //public Self Key { get; set; }

        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("realm")]
        //public Realm Realm { get; set; }
    }

    public partial class Self
    {
        //[JsonPropertyName("href")]
        //public Uri Href { get; set; }
    }

    public partial class Realm
    {
        //[JsonPropertyName("key")]
        //public Self Key { get; set; }

        //[JsonPropertyName("name")]
        //public Name Name { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("slug")]
        //public string Slug { get; set; }
    }


    public partial class Faction
    {
        //[JsonPropertyName("type")]
        //public string Type { get; set; }

        //[JsonPropertyName("name")]
        //public Name Name { get; set; }
    }

    public partial class Links
    {
        //[JsonPropertyName("self")]
        //public Self Self { get; set; }
    }

    public partial class Season
    {
        //[JsonPropertyName("key")]
        //public Self Key { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }
    }

    public partial class MatchStatistics
    {
        [JsonPropertyName("played")]
        public long Played { get; set; }

        [JsonPropertyName("won")]
        public long Won { get; set; }

        [JsonPropertyName("lost")]
        public long Lost { get; set; }
    }
    // Achievement Summary JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/achievements?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class AchievementSummaryJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        //[JsonPropertyName("total_quantity")]
        //public long TotalQuantity { get; set; }

        //[JsonPropertyName("total_points")]
        //public long TotalPoints { get; set; }

        [JsonPropertyName("achievements")]
        public Achievement[] Achievements { get; set; }

        //[JsonPropertyName("category_progress")]
        //public CategoryProgress[] CategoryProgress { get; set; }

        //[JsonPropertyName("recent_events")]
        //public RecentEvent[] RecentEvents { get; set; }

        //[JsonPropertyName("character")]
        //public Character Character { get; set; }

        //[JsonPropertyName("statistics")]
        //public Statistics Statistics { get; set; }
    }

    public partial class Achievement
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("achievement")]
        public Character AchievementAchievement { get; set; }

        //[JsonPropertyName("criteria")]
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //public Criteria Criteria { get; set; }

        //[JsonPropertyName("completed_timestamp")]
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //public long? CompletedTimestamp { get; set; }
    }

    public partial class Character
    {
        //[JsonPropertyName("key")]
        //public Statistics Key { get; set; }

        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("realm", NullValueHandling = NullValueHandling.Ignore)]
        //public Realm Realm { get; set; }
    }

    //public partial class Statistics
    //{
    //    [JsonPropertyName("href")]
    //    public Uri Href { get; set; }
    //}

    public partial class Realm
    {
        //[JsonPropertyName("key")]
        //public Statistics Key { get; set; }

        //[JsonPropertyName("name")]
        //public Name Name { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("slug")]
        //public string Slug { get; set; }
    }


    //public partial class Criteria
    //{
    //    [JsonPropertyName("id")]
    //    public long Id { get; set; }

    //    [JsonPropertyName("is_completed")]
    //    public bool IsCompleted { get; set; }

    //    [JsonPropertyName("child_criteria")]
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    //    public Criteria[] ChildCriteria { get; set; }

    //    [JsonPropertyName("amount")]
    //    [JsonIgnore]
    //    public BigInteger Amount { get; set; }
    //}

    //public partial class CategoryProgress
    //{
    //    [JsonPropertyName("category")]
    //    public Character Category { get; set; }

    //    [JsonPropertyName("quantity")]
    //    public long Quantity { get; set; }

    //    [JsonPropertyName("points")]
    //    public long Points { get; set; }
    //}

    public partial class Links
    {
        //[JsonPropertyName("self")]
        //public Statistics Self { get; set; }
    }

    //public partial class RecentEvent
    //{
    //    [JsonPropertyName("achievement")]
    //    public Character Achievement { get; set; }

    //    [JsonPropertyName("timestamp")]
    //    public long Timestamp { get; set; }
    //}


    // Mythic Plus Season Index JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/data/wow/mythic-keystone/season/index?namespace=dynamic-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class MPlusSeasonIndexJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("seasons")]
        public Season[] Seasons { get; set; }

        [JsonPropertyName("current_season")]
        public Season CurrentSeason { get; set; }
    }

    //public partial class Season
    //{
    //    [JsonPropertyName("key")]
    //    public Self Key { get; set; }

    //    [JsonPropertyName("id")]
    //    public long Id { get; set; }
    //}

    //public partial class Self
    //{
    //    [JsonPropertyName("href")]
    //    public Uri Href { get; set; }
    //}

    //public partial class Links
    //{
    //    [JsonPropertyName("self")]
    //    public Self Self { get; set; }
    //}

    // Character Stats JSON Objects
    /// <summary>
    /// https://us.api.blizzard.com/profile/wow/character/{realm}/{character}/statistics?namespace=profile-us
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class CharacterStatsJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("health")]
        public long Health { get; set; }

        //[JsonPropertyName("power")]
        //public long Power { get; set; }

        //[JsonPropertyName("power_type")]
        //public PowerType PowerType { get; set; }

        //[JsonPropertyName("speed")]
        //public Avoidance Speed { get; set; }

        //[JsonPropertyName("strength")]
        //public Agility Strength { get; set; }

        //[JsonPropertyName("agility")]
        //public Agility Agility { get; set; }

        //[JsonPropertyName("intellect")]
        //public Agility Intellect { get; set; }

        //[JsonPropertyName("stamina")]
        //public Agility Stamina { get; set; }

        //[JsonPropertyName("melee_crit")]
        //public Block MeleeCrit { get; set; }

        //[JsonPropertyName("melee_haste")]
        //public Block MeleeHaste { get; set; }

        //[JsonPropertyName("mastery")]
        //public Block Mastery { get; set; }

        //[JsonPropertyName("bonus_armor")]
        //public double BonusArmor { get; set; }

        //[JsonPropertyName("lifesteal")]
        //public Block Lifesteal { get; set; }

        [JsonPropertyName("versatility")]
        public double Versatility { get; set; }

        [JsonPropertyName("versatility_damage_done_bonus")]
        public double VersatilityDamageDoneBonus { get; set; }

        [JsonPropertyName("versatility_healing_done_bonus")]
        public double VersatilityHealingDoneBonus { get; set; }

        [JsonPropertyName("versatility_damage_taken_bonus")]
        public double VersatilityDamageTakenBonus { get; set; }

        //[JsonPropertyName("avoidance")]
        //public Avoidance Avoidance { get; set; }

        //[JsonPropertyName("attack_power")]
        //public double AttackPower { get; set; }

        //[JsonPropertyName("main_hand_damage_min")]
        //public double MainHandDamageMin { get; set; }

        //[JsonPropertyName("main_hand_damage_max")]
        //public double MainHandDamageMax { get; set; }

        //[JsonPropertyName("main_hand_speed")]
        //public double MainHandSpeed { get; set; }

        //[JsonPropertyName("main_hand_dps")]
        //public double MainHandDps { get; set; }

        //[JsonPropertyName("off_hand_damage_min")]
        //public double OffHandDamageMin { get; set; }

        //[JsonPropertyName("off_hand_damage_max")]
        //public double OffHandDamageMax { get; set; }

        //[JsonPropertyName("off_hand_speed")]
        //public double OffHandSpeed { get; set; }

        //[JsonPropertyName("off_hand_dps")]
        //public double OffHandDps { get; set; }

        //[JsonPropertyName("spell_power")]
        //public double SpellPower { get; set; }

        //[JsonPropertyName("spell_penetration")]
        //public double SpellPenetration { get; set; }

        //[JsonPropertyName("spell_crit")]
        //public Block SpellCrit { get; set; }

        //[JsonPropertyName("mana_regen")]
        //public double ManaRegen { get; set; }

        //[JsonPropertyName("mana_regen_combat")]
        //public double ManaRegenCombat { get; set; }

        //[JsonPropertyName("armor")]
        //public Agility Armor { get; set; }

        //[JsonPropertyName("dodge")]
        //public Block Dodge { get; set; }

        //[JsonPropertyName("parry")]
        //public Block Parry { get; set; }

        //[JsonPropertyName("block")]
        //public Block Block { get; set; }

        //[JsonPropertyName("ranged_crit")]
        //public Block RangedCrit { get; set; }

        //[JsonPropertyName("ranged_haste")]
        //public Block RangedHaste { get; set; }

        //[JsonPropertyName("spell_haste")]
        //public Block SpellHaste { get; set; }

        //[JsonPropertyName("character")]
        //public Character Character { get; set; }
    }

    //public partial class Agility
    //{
    //    [JsonPropertyName("base")]
    //    public double Base { get; set; }

    //    [JsonPropertyName("effective")]
    //    public double Effective { get; set; }
    //}

    //public partial class Avoidance
    //{
    //    [JsonPropertyName("rating")]
    //    public double Rating { get; set; }

    //    [JsonPropertyName("rating_bonus")]
    //    public double RatingBonus { get; set; }
    //}

    //public partial class Block
    //{
    //    [JsonPropertyName("rating")]
    //    public double Rating { get; set; }

    //    [JsonPropertyName("rating_bonus")]
    //    public double RatingBonus { get; set; }

    //    [JsonPropertyName("value")]
    //    public double Value { get; set; }
    //}

    public partial class Character
    {
        //[JsonPropertyName("key")]
        //public Self Key { get; set; }

        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        //[JsonPropertyName("id")]
        //public long Id { get; set; }

        //[JsonPropertyName("realm")]
        //public PowerType Realm { get; set; }
    }

    public partial class Self
    {
        //[JsonPropertyName("href")]
        //public Uri Href { get; set; }
    }

    //public partial class PowerType
    //{
    //    [JsonPropertyName("key")]
    //    public Self Key { get; set; }

    //    [JsonPropertyName("name")]
    //    public string Name { get; set; }

    //    [JsonPropertyName("id")]
    //    public long Id { get; set; }

    //    [JsonPropertyName("slug")]
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    //    public string Slug { get; set; }
    //}


    public partial class Links
    {
        //[JsonPropertyName("self")]
        //public Self Self { get; set; }
    }
    // Mythic Keystone Dungeons Index
    /// <summary>
    /// https://us.api.blizzard.com/data/wow/mythic-keystone/dungeon/index
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class AllDungeonsJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("dungeons")]
        public Dungeon[] Dungeons { get; set; }
    }

    //public partial class Dungeon
    //{
    //    [JsonPropertyName("key")]
    //    public Self Key { get; set; }

    //    [JsonPropertyName("name")]
    //    public Name Name { get; set; }

    //    [JsonPropertyName("id")]
    //    public long Id { get; set; }
    //}

    //public partial class Self
    //{
    //    [JsonPropertyName("href")]
    //    public Uri Href { get; set; }
    //}


    //public partial class Links
    //{
    //    [JsonPropertyName("self")]
    //    public Self Self { get; set; }
    //}

    // WoW Token Index (US, EU, KR, TW)
    /// <summary>
    /// https://{this.Config.Region}.api.blizzard.com/data/wow/token/index
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class WoWTokenJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("last_updated_timestamp")]
        public long LastUpdatedTimestamp { get; set; }

        [JsonPropertyName("price")]
        public long Price { get; set; }
    }

    //public partial class Links
    //{
    //    [JsonPropertyName("self")]
    //    public Self Self { get; set; }
    //}

    //public partial class Self
    //{
    //    [JsonPropertyName("href")]
    //    public Uri Href { get; set; }
    //}


    ///
    /// Check Token
    ///
    public partial class CheckTokenJson // Used by BlizzardAPI.CheckToken()
    {
        [JsonPropertyName("scope")]
        public object[] Scope { get; set; }

        [JsonPropertyName("account_authorities")]
        public object[] AccountAuthorities { get; set; }

        [JsonPropertyName("exp")]
        public long Exp { get; set; }

        [JsonPropertyName("client_authorities")]
        public object[] ClientAuthorities { get; set; }

        [JsonPropertyName("authorities")]
        public Authority[] Authorities { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
    }

    public partial class Authority
    {
        [JsonPropertyName("authority")]
        public string AuthorityAuthority { get; set; }
    }

    // Character PVP Summary
    /// <summary>
    /// /profile/wow/character/{realmSlug}/{characterName}/pvp-summary
    /// Added via https://app.quicktype.io/
    /// </summary>
    public partial class PvpSummaryJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("brackets")]
        public Self[] Brackets { get; set; }

        //[JsonPropertyName("honor_level")]
        //public long HonorLevel { get; set; }

        //[JsonPropertyName("pvp_map_statistics")]
        //public PvpMapStatistic[] PvpMapStatistics { get; set; }

        //[JsonPropertyName("honorable_kills")]
        //public long HonorableKills { get; set; }

        //[JsonPropertyName("character")]
        //public Character Character { get; set; }
    }

    //public partial class Self
    //{
    //    [JsonPropertyName("href")]
    //    public Uri Href { get; set; }
    //}

    //public partial class Character
    //{
    //    [JsonPropertyName("key")]
    //    public Self Key { get; set; }

    //    [JsonPropertyName("name")]
    //    public string Name { get; set; }

    //    [JsonPropertyName("id")]
    //    public long Id { get; set; }

    //    [JsonPropertyName("realm", NullValueHandling = NullValueHandling.Ignore)]
    //    public Character Realm { get; set; }

    //    [JsonPropertyName("slug", NullValueHandling = NullValueHandling.Ignore)]
    //    public string Slug { get; set; }
    //}

    //public partial class Links
    //{
    //    [JsonPropertyName("self")]
    //    public Self Self { get; set; }
    //}

    //public partial class PvpMapStatistic
    //{
    //    [JsonPropertyName("world_map")]
    //    public WorldMap WorldMap { get; set; }

    //    [JsonPropertyName("match_statistics")]
    //    public MatchStatistics MatchStatistics { get; set; }
    //}

    //public partial class MatchStatistics
    //{
    //    [JsonPropertyName("played")]
    //    public long Played { get; set; }

    //    [JsonPropertyName("won")]
    //    public long Won { get; set; }

    //    [JsonPropertyName("lost")]
    //    public long Lost { get; set; }
    //}

    //public partial class WorldMap
    //{
    //    [JsonPropertyName("name")]
    //    public string Name { get; set; }

    //    [JsonPropertyName("id")]
    //    public long Id { get; set; }
    //}

    // Character Mythic Keystone Profile Index
    /// <summary>
    /// /profile/wow/character/{realmSlug}/{characterName}/mythic-keystone-profile
    /// Added via https://app.quicktype.io/
    /// </summary>

    public partial class MPlusSummaryJson
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("current_period")]
        public CurrentPeriod CurrentPeriod { get; set; }

        [JsonPropertyName("seasons")]
        public Period[] Seasons { get; set; }

        [JsonPropertyName("character")]
        public Character Character { get; set; }
    }

    //public partial class Character
    //{
    //    [JsonPropertyName("key")]
    //    public Self Key { get; set; }

    //    [JsonPropertyName("name")]
    //    public string Name { get; set; }

    //    [JsonPropertyName("id")]
    //    public long Id { get; set; }

    //    [JsonPropertyName("realm", NullValueHandling = NullValueHandling.Ignore)]
    //    public Character Realm { get; set; }

    //    [JsonPropertyName("slug", NullValueHandling = NullValueHandling.Ignore)]
    //    public string Slug { get; set; }
    //}

    //public partial class Self
    //{
    //    [JsonPropertyName("href")]
    //    public Uri Href { get; set; }
    //}

    //public partial class CurrentPeriod
    //{
    //    [JsonPropertyName("period")]
    //    public Period Period { get; set; }
    //}

    //public partial class Period
    //{
    //    [JsonPropertyName("key")]
    //    public Self Key { get; set; }

    //    [JsonPropertyName("id")]
    //    public long Id { get; set; }
    //}

    //public partial class Links
    //{
    //    [JsonPropertyName("self")]
    //    public Self Self { get; set; }
    //}
}
