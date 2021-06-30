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
    public class ArmoryBotConfig // Stores Blizzard Config as loaded from Config File
    {
        public ArmoryBotConfig() { }
        public string discordtoken { get; set; }
        public char cmdprefix { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        private string _locale; // Backing field
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
                        throw new Exception("Invalid locale specified in appsettings.json");
                } // End switch
            } // end Private Set
        } // End locale
        public string Region { get; set; } // https://develop.battle.net/documentation/world-of-warcraft/guides/namespaces
    }
    public class BlizzardAccessToken // Blizzard Access Token Json > C# Class 
    {
        [JsonInclude]
        [JsonPropertyName("access_token")]
        public string access_token { get; private set; }
        [JsonInclude]
        [JsonPropertyName("token_type")]
        public string token_type { get; private set; }
        private int _expires_in; // Backing Field
        [JsonInclude]
        [JsonPropertyName("expires_in")]
        public int expires_in // Seconds
        {
            get
            {
                return this._expires_in;
            }
            private set
            {
                this._expires_in = value;
                TimeSpan exptime = new TimeSpan(0, 0, value); // Hours , Minutes, Seconds
                this.expire_date = DateTime.Now.Add(exptime);
            }
        }
        public DateTime expire_date { get; private set; }
        [JsonInclude]
        [JsonPropertyName("scope")]
        public string scope { get; private set; }
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
                output += $"• {mode.Progress.CompletedCount}/{mode.Progress.TotalCount} {mode.Difficulty.Name}\n"; // ex: 8/10 Normal
            }
            return output;
        }
    }

    public class MythicPlusData
    {
        private readonly int DungeonCount;
        public int Rating { get; private set; } // M+ Rating Introduced in 9.1 Chains of Domination
        public int HighestRun { get; private set; } // Highest M+ run player has completed
        public MythicPlusData(int dungeonCount)
        {
            this.DungeonCount = dungeonCount;
            this.Rating = 0;
            this.HighestRun = 0;
        }

        public void Parse(MPlusSummaryJson summary, MPlusSeasonInfoJson season)
        {
            if (summary.CurrentMythicRating is not null)
            {
                this.Rating = (int)summary.CurrentMythicRating.Rating;
            }
            foreach (BestRun run in season.BestRuns)
            {
                if (run.KeystoneLevel > this.HighestRun) this.HighestRun = (int)run.KeystoneLevel;
            }
        }

        public override string ToString() // Displays output 
        {
            return $"• Rating: {this.Rating}\n• Highest Key: +{this.HighestRun}";
        }
    }
}
