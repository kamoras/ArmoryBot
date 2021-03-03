using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ArmoryBot
{
    //
    // ** Custom Classes used in program
    //
    public class ArmoryData // Stores collection of data passed from BlizzardAPI.cs to ArmoryBot.cs
    {
        public string CharacterInfo; // Stores string returned by BlizzardAPI.GetCharacter()
        public string AvatarUrl; // Stores string returned by BlizzardAPI.GetAvatar()
        public RaidData RaidInfo; // Stores RaidData returned by BlizzardAPI.GetRaids()
        public string MythicPlus; // Stores string returned by BlizzardAPI.GetMythicPlus()
        public string Achievements; // Stores string returned by BlizzardAPI.GetAchievements()
        public string PVPRating; // Stores string returned by BlizzardAPI.GetPVP()
        public string PVPStats; // Stores string returned by BlizzardAPI.GetPvpStats()
    }
    public class DiscordConfig // Stores Discord Config as loaded from Config File
    {
        [JsonProperty("token")]
        public string token { get; private set; }
        private char _cmdprefix; // Backing field
        [JsonProperty("cmdprefix")]
        public char cmdprefix
        {
            get { return this._cmdprefix; }
            private set { this._cmdprefix = value; this.Prefix = value.ToString(); }
        }
        public string Prefix { get; private set; }
    }
    public class BlizzardConfig // Stores Blizzard Config as loaded from Config File
    {
        [JsonProperty("client_id")]
        public string client_id { get; private set; }
        [JsonProperty("client_secret")]
        public string client_secret { get; private set; }
        private string _locale; // Backing field
        [JsonProperty("locale")]
        public string locale
        {
            get { return this._locale; }
            private set
            {
                this._locale = value;
                switch (value)
                {
                    case "en_US":
                        this.TOKENroot = "https://us.battle.net";
                        this.APIroot = "https://us.api.blizzard.com";
                        this.PROFILEnamespace = "?namespace=profile-us";
                        this.DYNAMICnamespace = "?namespace=dynamic-us";
                        break;
                    case "es_MX":
                        this.TOKENroot = "https://us.battle.net";
                        this.APIroot = "https://us.api.blizzard.com";
                        this.PROFILEnamespace = "?namespace=profile-us";
                        this.DYNAMICnamespace = "?namespace=dynamic-us";
                        break;
                    case "pt_BR":
                        this.TOKENroot = "https://us.battle.net";
                        this.APIroot = "https://us.api.blizzard.com";
                        this.PROFILEnamespace = "?namespace=profile-us";
                        this.DYNAMICnamespace = "?namespace=dynamic-us";
                        break;
                    case "de_DE":
                        this.TOKENroot = "https://eu.battle.net";
                        this.APIroot = "https://eu.api.blizzard.com";
                        this.PROFILEnamespace = "?namespace=profile-eu";
                        this.DYNAMICnamespace = "?namespace=dynamic-eu";
                        break;
                    case "en_GB":
                        this.TOKENroot = "https://eu.battle.net";
                        this.APIroot = "https://eu.api.blizzard.com";
                        this.PROFILEnamespace = "?namespace=profile-eu";
                        this.DYNAMICnamespace = "?namespace=dynamic-eu";
                        break;
                    case "es_ES":
                        this.TOKENroot = "https://eu.battle.net";
                        this.APIroot = "https://eu.api.blizzard.com";
                        this.PROFILEnamespace = "?namespace=profile-eu";
                        this.DYNAMICnamespace = "?namespace=dynamic-eu";
                        break;
                    case "fr_FR":
                        this.TOKENroot = "https://eu.battle.net";
                        this.APIroot = "https://eu.api.blizzard.com";
                        this.PROFILEnamespace = "?namespace=profile-eu";
                        this.DYNAMICnamespace = "?namespace=dynamic-eu";
                        break;
                    case "it_IT":
                        this.TOKENroot = "https://eu.battle.net";
                        this.APIroot = "https://eu.api.blizzard.com";
                        this.PROFILEnamespace = "?namespace=profile-eu";
                        this.DYNAMICnamespace = "?namespace=dynamic-eu";
                        break;
                    default:
                        throw new Exception($"Invalid locale specified in {Globals.BlizzardConfigPath}");
                } // End switch
            } // end Private Set
        } // End locale
        public string TOKENroot { get; private set; }
        public string APIroot { get; private set; }
        public string PROFILEnamespace { get; private set; }
        public string DYNAMICnamespace { get; private set; }
        public BlizzardAccessToken Token { get; set; }
    }
    public class BlizzardAccessToken // Blizzard Access Token Json > C# Class 
    {
        [JsonProperty("access_token")]
        public string access_token { get; private set; }
        [JsonProperty("token_type")]
        public string token_type { get; private set; }
        [JsonProperty("expires_in")]
        public int expires_in { get; private set; } // Seconds
        [JsonProperty("scope")]
        public string scope { get; private set; }
    }
    public class AchievementsList // Stores list of achievements via BlizzardAPI.GetAchievements()
    {
        private Dictionary<int, AchievementItem> List;
        public AchievementsList()
        {
            this.List = new Dictionary<int, AchievementItem>(); // Key: (int)group , Value: (AchievementItem)  |   Uses group as key since there should only be one entry per group in the Dictionary.
        }
        public void Add(long id, string name, string type)
        {
            switch (type)
            {
                case "pve":
                    if (Globals.AchievementsPVE[id].Group == -1) // No Group (-1) - Always Add
                    {
                        this.List.Add((int)id * -1, new AchievementItem(Globals.AchievementsPVE[id].Group, Globals.AchievementsPVE[id].Value, name));
                        return;
                    }
                    if (this.List.ContainsKey(Globals.AchievementsPVE[id].Group)) // Group is already in list, check if achievement value is higher
                    {
                        if (Globals.AchievementsPVE[id].Value > this.List[Globals.AchievementsPVE[id].Group].Value) // Value is higher
                        {
                            this.List.Remove(Globals.AchievementsPVE[id].Group);
                            this.List.Add(Globals.AchievementsPVE[id].Group, new AchievementItem(Globals.AchievementsPVE[id].Group, Globals.AchievementsPVE[id].Value, name));
                            return;
                        }
                        else return; // Value is not higher
                    }
                    else // Not in list, go ahead and add
                    {
                        this.List.Add(Globals.AchievementsPVE[id].Group, new AchievementItem(Globals.AchievementsPVE[id].Group, Globals.AchievementsPVE[id].Value, name));
                        return;
                    }
                case "pvp":
                    if (Globals.AchievementsPVP[id].Group == -1) // No Group (-1) - Always Add
                    {
                        this.List.Add((int)id * -1, new AchievementItem(Globals.AchievementsPVP[id].Group, Globals.AchievementsPVP[id].Value, name));
                        return;
                    }
                    if (this.List.ContainsKey(Globals.AchievementsPVP[id].Group)) // Group is already in list, check if achievement value is higher
                    {
                        if (Globals.AchievementsPVP[id].Value > this.List[Globals.AchievementsPVP[id].Group].Value) // Value is higher
                        {
                            this.List.Remove(Globals.AchievementsPVP[id].Group);
                            this.List.Add(Globals.AchievementsPVP[id].Group, new AchievementItem(Globals.AchievementsPVP[id].Group, Globals.AchievementsPVP[id].Value, name));
                            return;
                        }
                        else return; // Value is not higher
                    }
                    else // Not in list, go ahead and add
                    {
                        this.List.Add(Globals.AchievementsPVP[id].Group, new AchievementItem(Globals.AchievementsPVP[id].Group, Globals.AchievementsPVP[id].Value, name));
                        return;
                    }
                default:
                    throw new Exception("Invalid type specified!");
            }
        }
        public override string ToString()
        {
            string output = "";
            foreach (KeyValuePair<int, AchievementItem> entry in this.List)
            {
                output += $"* {entry.Value.Name}\n";
            }
            if (output.Length == 0) return "None";
            else return output;
        }
    } // End AchievementList
    public class AchievementItem // Defines a single achievement, used by class:AchievementList and Globals.cs
    {
        public int Group { get; private set; } // Must be unique, use -1 if you want the achievement to always be displayed
        public int Value { get; private set; } // Value of achievement (0 = lowest). Higher value means it will be displayed over lower value achievements in the same group.
        public string Name { get; private set; } // Name of achievement as obtained by the API
        public AchievementItem(int group, int value, string name = null)
        {
            this.Group = group;
            this.Value = value;
            this.Name = name;
        }
    }
    public class RaidData // Stores all current expansion raids via BlizzardAPI.GetRaids()
    {
        public List<RaidItem> Raids { get; private set; }
        private string Locale;
        public RaidData(string locale)
        {
            this.Raids = new List<RaidItem>();
            this.Locale = locale;
        }
        public void Add(Instance raid)
        {
            this.Raids.Add(new RaidItem(raid, this.Locale));
        }
    }
    public class RaidItem // Child class for RaidData
    {
        public string Name { get; private set; }
        private Instance Raid;
        private string Locale;
        public RaidItem(Instance raid, string locale)
        {
            this.Name = raid.InstanceInstance.Name;
            this.Raid = raid;
            this.Locale = locale;
        }
        public override string ToString() // Displays output
        {
            string output = "";
            foreach (Mode mode in this.Raid.Modes) // Check each difficulty
            {
                output += $"* {mode.Progress.CompletedCount}/{mode.Progress.TotalCount} {mode.Difficulty.Name.GetLocale(this.Locale)}\n"; // ex: 8/10 Normal
            }
            return output;
        }
    }

    /* NOTE: The MythicPlusData class was created because I noticed players were returning over 8 dungeons of M+ Data (there are only 8 dungeons in SL). It turned out that the API keeps track of the
 * Best Runs for both On Time/Expired runs, so a particular dungeon ID could have two results. This class will keep track of the best keystone level run for each unique dungeon ID. 
 * So if you did a +4 in Plaguefall that expired, but did a +10 ON TIME, the +4 is discarded. This will give a better indication if a player is completing their Best Runs on time or not.
    */
    public class MythicPlusData // Sorts and returns the best runs per Dungeon ID
    {
        private Dictionary<long, BestRun> Runs;
        public int HighestRun { get; private set; } // Highest M+ run player has completed
        public int Plus5Count { get; private set; } // Best runs between +5 and +9
        public int Plus10Count { get; private set; } // Best runs between +10 and +14
        public int Plus15Count { get; private set; } // Best runs +15 and higher
        public int ExpiredCount { get; private set; } // Best runs that the timer expired
        public MythicPlusData()
        {
            this.Runs = new Dictionary<long, BestRun>();
            this.HighestRun = 0;
            this.Plus5Count = 0;
            this.Plus10Count = 0;
            this.Plus15Count = 0;
            this.ExpiredCount = 0;
        }
        public void Add(BestRun run)
        {
            if (!this.Runs.ContainsKey(run.Dungeon.Id)) // Not in list yet - go ahead and add
            {
                this.Runs.Add(run.Dungeon.Id, run);
                // Increment counters
                if (!run.IsCompletedWithinTime) this.ExpiredCount += 1;
                if (run.KeystoneLevel > this.HighestRun) this.HighestRun = (int)run.KeystoneLevel;
                if (run.KeystoneLevel >= 15) this.Plus15Count += 1;
                else if (run.KeystoneLevel >= 10) this.Plus10Count += 1;
                else if (run.KeystoneLevel >= 5) this.Plus5Count += 1;
            }
            else // Already in list, see if run is higher
            {
                if (run.KeystoneLevel > this.Runs[run.Dungeon.Id].KeystoneLevel) // Run is higher - add
                {
                    // De-increment old run
                    if (!this.Runs[run.Dungeon.Id].IsCompletedWithinTime) this.ExpiredCount -= 1;
                    if (this.Runs[run.Dungeon.Id].KeystoneLevel >= 5 & this.Runs[run.Dungeon.Id].KeystoneLevel < 10) this.Plus5Count -= 1;
                    else if (this.Runs[run.Dungeon.Id].KeystoneLevel >= 10 & this.Runs[run.Dungeon.Id].KeystoneLevel < 15) this.Plus10Count -= 1;
                    else if (this.Runs[run.Dungeon.Id].KeystoneLevel >= 15) this.Plus15Count -= 1;
                    this.Runs.Remove(run.Dungeon.Id); // Remove old run
                    this.Runs.Add(run.Dungeon.Id, run); // Add new run
                    // Increment new run
                    if (!run.IsCompletedWithinTime) this.ExpiredCount += 1;
                    if (run.KeystoneLevel > this.HighestRun) this.HighestRun = (int)run.KeystoneLevel;
                    if (run.KeystoneLevel >= 15) this.Plus15Count += 1;
                    else if (run.KeystoneLevel >= 10) this.Plus10Count += 1;
                    else if (run.KeystoneLevel >= 5) this.Plus5Count += 1;
                }
            }
        }
        public override string ToString() // Displays output 
        {
            return $"**Best Run:** +{this.HighestRun}\n* 5+ Runs: {this.Plus5Count}\n* 10+ Runs: {this.Plus10Count}\n* 15+ Runs: {this.Plus15Count}\n* Time Expired Runs: {this.ExpiredCount}";
        }
    }
}
