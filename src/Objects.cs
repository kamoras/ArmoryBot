using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ArmoryBot
{
    //
    // ** Custom Classes used in program
    //
    public class ArmoryData // Stores collection of data passed from BlizzardAPI.cs to ArmoryBot.cs
    {
        public CharacterInfo CharInfo; // Stores string returned by BlizzardAPI.GetCharacter()
        public string AvatarUrl; // Stores string returned by BlizzardAPI.GetAvatar()
        public RaidData RaidInfo; // Stores RaidData returned by BlizzardAPI.GetRaids()
        public string MythicPlus; // Stores string returned by BlizzardAPI.GetMythicPlus()
        public string Achievements; // Stores string returned by BlizzardAPI.GetAchievements()
        public string PVPRating; // Stores string returned by BlizzardAPI.GetPVP()
        public string PVPStats; // Stores string returned by BlizzardAPI.GetPvpStats()
    }
    public class WoWToken // Stores collection of data passed from BlizzardAPI.cs to ArmoryBot.cs
    {
        public string Price;
        public string Last_Updated;
    }
    public class DiscordConfig // Stores Discord Config as loaded from Config File
    {
        [JsonPropertyName("token")]
        public string token { get; set; }
        [JsonPropertyName("cmdprefix")]
        public char cmdprefix { get; set; }
    }
    public class BlizzardConfig // Stores Blizzard Config as loaded from Config File
    {
        [JsonPropertyName("client_id")]
        public string client_id { get; set; }
        [JsonPropertyName("client_secret")]
        public string client_secret { get; set; }
        private string _locale; // Backing field
        [JsonPropertyName("locale")]
        public string locale
        {
            get { return this._locale; }
            set
            {
                this._locale = value.ToLower();
                switch (value.ToLower())
                {
                    case "en_us":
                        this.Region = "us";
                        break;
                    case "es_mx":
                        this.Region = "us";
                        break;
                    case "pt_br":
                        this.Region = "us";
                        break;
                    case "de_de":
                        this.Region = "eu";
                        break;
                    case "en_gb":
                        this.Region = "eu";
                        break;
                    case "es_es":
                        this.Region = "eu";
                        break;
                    case "fr_fr":
                        this.Region = "eu";
                        break;
                    case "it_it":
                        this.Region = "eu";
                        break;
                    default:
                        throw new Exception($"Invalid locale specified in {Globals.BlizzardConfigPath}");
                } // End switch
            } // end Private Set
        } // End locale
        public string Region { get; private set; } // https://develop.battle.net/documentation/world-of-warcraft/guides/namespaces
        public BlizzardAccessToken Token { get; set; }
    }
    public class BlizzardAccessToken // Blizzard Access Token Json > C# Class 
    {
        [JsonPropertyName("access_token")]
        public string access_token { get; set; }
        [JsonPropertyName("token_type")]
        public string token_type { get; set; }
        private int _expires_in; // Backing Field
        [JsonPropertyName("expires_in")]
        public int expires_in // Seconds
        {
            get
            {
                return this._expires_in;
            }
            set
            {
                this._expires_in = value;
                DateTime now = DateTime.Now;
                TimeSpan time = new TimeSpan(0, 0, value); // Hours , Minutes, Seconds
                this.expire_date = now.Add(time);
            }
        }
        public DateTime expire_date { get; set; }
        [JsonPropertyName("scope")]
        public string scope { get; set; }
    }

    public class AchievementsList // Stores list of achievements via BlizzardAPI.GetAchievements()
    {
        private Dictionary<int, AchievementItem> List;
        public AchievementsList()
        {
            this.List = new Dictionary<int, AchievementItem>(); // Key: (int)group , Value: (AchievementItem)  |   Uses group as key since there should only be one entry per group in the Dictionary.
        }
        public void Add(long id, string name, LookupType type)
        {
            switch (type)
            {
                case LookupType.PVE:
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
                case LookupType.PVP:
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
                output += $"• {entry.Value.Name}\n";
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
    public class CharacterInfo // sub-class of (ArmoryData)
    {
        public string Name; // Stores string returned by BlizzardAPI.GetCharacter()
        public string ItemLevel; // Stores string returned by BlizzardAPI.GetCharacter()
        public string Renown; // Stores string returned by BlizzardAPI.GetCharacter()
        public string ArmoryUrl; // Stores string returned by BlizzardAPI.GetCharacter()
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
        public string Name { get; set; }
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
                output += $"• {mode.Progress.CompletedCount}/{mode.Progress.TotalCount} {mode.Difficulty.Name}\n"; // ex: 8/10 Normal
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
        private readonly int DungeonCount;
        private Dictionary<long, BestRun> Runs;
        public int HighestRun { get; private set; } // Highest M+ run player has completed
        public int Plus5Count { get; private set; } // Best runs between +5 and +9
        public int Plus10Count { get; private set; } // Best runs between +10 and +14
        public int Plus15Count { get; private set; } // Best runs +15 and higher
        public int ExpiredCount { get; private set; } // Best runs that the timer expired
        public MythicPlusData(int dungeonCount)
        {
            this.DungeonCount = dungeonCount;
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
                if (run.KeystoneLevel > this.Runs[run.Dungeon.Id].KeystoneLevel | (run.KeystoneLevel == this.Runs[run.Dungeon.Id].KeystoneLevel & run.IsCompletedWithinTime)) // Run is higher - add - OR - Run is equal but on-time
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
            return $"• Best Run: +{this.HighestRun}\n• 5+ Runs: {this.Plus5Count}/{this.DungeonCount}\n• 10+ Runs: {this.Plus10Count}/{this.DungeonCount}\n• 15+ Runs: {this.Plus15Count}/{this.DungeonCount}\n• Expired Runs: {this.ExpiredCount}/{this.DungeonCount}";
        }
    }
}
