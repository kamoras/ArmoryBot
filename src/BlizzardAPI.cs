using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ArmoryBot
{
    public class BlizzardAPI
    {
        private BlizzardConfig Config;
        private Timer CheckToken_RateLimit = new Timer(); // Allow check token API call once every 15 minutes.
        private Timer _TokenExpTimer; // Backing field
        private Timer TokenExpTimer
        {
            set 
            {
                this._TokenExpTimer = new Timer(this.Config.Token.expires_in * 1000); // Token usually lasts 24 hours (convert seconds to ms)
                this._TokenExpTimer.AutoReset = false;
                this._TokenExpTimer.Elapsed += TokenExpTimer_Elapsed; // Set event
                this._TokenExpTimer.Start(); // Start timer
            }
        }
        public BlizzardAPI()
        {
            using (StreamReader json = File.OpenText(Globals.BlizzardConfigPath)) // Load Config
            {
                this.Config = (BlizzardConfig)Program.jsonSerializer.Deserialize(json, typeof(BlizzardConfig));
            }
            this.RequestToken();
        }
        public async Task<ArmoryData> ArmoryLookup(string character, string realm, string type) // Main Armory Lookup Method exposed to ArmoryBot.cs
        {
            try
            {
                ArmoryData info = new ArmoryData(); // This method makes a number of separate API Calls. All the data is stored to this ArmoryData class to easily pass to the calling function.
                if (type != "pve" & type != "pvp") throw new Exception($"Type must be either pve or pvp. '{type}' is invalid."); // Make sure lookup type is correct
                Task<string> CharInfo = this.GetCharacter(character, realm); // Gets basic character info (Player name, race, class, spec, etc.)
                Task<string> AvatarInfo = this.GetAvatar(character, realm); // Gets character avatar image URL
                switch (type)
                {
                    case "pve":
                        Task<RaidData> RaidInfo = this.GetRaids(character, realm); // Gets all raid info from Current Expansion
                        Task<string> MythicPlus = this.GetMythicPlus(character, realm); // Gets all M+ info from Current Season
                        Task<string> PVEAchiev = this.GetAchievements(character, realm, type); // Gets all PVE-Centric Achievements
                        await Task.WhenAll(RaidInfo, MythicPlus, PVEAchiev); // Wait for all PVE tasks to finish up
                        info.RaidInfo = RaidInfo.Result; info.MythicPlus = MythicPlus.Result; info.Achievements = PVEAchiev.Result; // Move results into class:ArmoryData
                        break;
                    case "pvp":
                        Task<string> PvpInfo = this.GetPVP(character, realm); // Gets all rated PVP bracket info
                        Task<string> PVPStats = this.GetPvpStats(character, realm); // Gets all PVP Character Stats info (Versatility,etc.)
                        Task<string> PVPAchiev = this.GetAchievements(character, realm, type); // Gets all PVP-Centric Achievements
                        await Task.WhenAll(PvpInfo, PVPStats, PVPAchiev); // Wait for all PVP tasks to finish up
                        info.PVPRating = PvpInfo.Result; info.PVPStats = PVPStats.Result; info.Achievements = PVPAchiev.Result; // Move results into class:ArmoryData
                        break;
                }
                await Task.WhenAll(CharInfo, AvatarInfo); // Wait for all other tasks to finish up
                info.CharacterInfo = CharInfo.Result; info.AvatarUrl = AvatarInfo.Result; // Move results into class:ArmoryData
                return info; // Return class:ArmoryData to calling function
            }
            catch
            {
                this.CheckToken(); // Make sure token is not expired
                throw; // Re-throw exception, will be caught in calling function
            }
        }
        private async Task<string> GetCharacter(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            string output = "";
            string json = await this.Call($"{this.Config.APIroot}/profile/wow/character/{realm}/{character}{this.Config.PROFILEnamespace}");
            using (TextReader sr = new StringReader(json))
            {
                var charinfo = (CharacterSummary)Program.jsonSerializer.Deserialize(sr, typeof(CharacterSummary)); // De-serialize JSON to C# Classes
                output += $"{charinfo.Name} {charinfo.Level} {charinfo.Race.Name.GetLocale(this.Config.locale)} {charinfo.ActiveSpec.Name.GetLocale(this.Config.locale)} {charinfo.CharacterClass.Name.GetLocale(this.Config.locale)}                                 \n**Item Level: {charinfo.EquippedItemLevel}     Renown: {charinfo.CovenantProgress?.RenownLevel} {charinfo.CovenantProgress?.ChosenCovenant.Name.GetLocale(this.Config.locale)}**";
            }
            return output;
        }
        private async Task<string> GetAvatar(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            string json = await this.Call($"{this.Config.APIroot}/profile/wow/character/{realm}/{character}/character-media{this.Config.PROFILEnamespace}");
            using (TextReader sr = new StringReader(json))
            {
                var charmedia = (CharacterMedia)Program.jsonSerializer.Deserialize(sr, typeof(CharacterMedia)); // De-serialize JSON to C# Classes
                foreach (Asset asset in charmedia.Assets)
                {
                    if (asset.Key.ToLower() == "avatar") return asset.Value.ToString();
                }
            }
            throw new NullReferenceException("Avatar value is null."); // Raise exception if unable to get Character Avatar
        }
        private async Task<RaidData> GetRaids(string character, string realm) // Returns class:RaidData to this.ArmoryLookup()
        {
            RaidData data = new RaidData(this.Config.locale);
            string json = await this.Call($"{this.Config.APIroot}/profile/wow/character/{realm}/{character}/encounters/raids{this.Config.PROFILEnamespace}");
            using (TextReader sr = new StringReader(json))
            {
                var raidinfo = (RaidInfo)Program.jsonSerializer.Deserialize(sr, typeof(RaidInfo)); // De-serialize JSON to C# Classes
                if (raidinfo.ExpansionsExpansions != null) foreach (Expansion expansion in raidinfo.ExpansionsExpansions)
                    {
                        switch (expansion.ExpansionExpansion.Id)
                        {
                            case (long)ID.CurrentExpansion: // Check current expansion
                                foreach (Instance raid in expansion.Instances) // Check all raids
                                {
                                    data.Add(raid); // Add to RaidData
                                }
                                break;
                            default:
                                break;
                        }
                    }
            }
            return data;
        }
        private async Task<string> GetMythicPlus(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            try // This section will 404 Not found if no M+ completed, use try/catch
            {
                MythicPlusData data = new MythicPlusData();
                string season = null;
                string json_seasoninfo = await this.Call($"{this.Config.APIroot}/data/wow/mythic-keystone/season/index{this.Config.DYNAMICnamespace}");
                using (TextReader sr = new StringReader(json_seasoninfo))
                {
                    var seasonindex = (MPlusSeasonIndex)Program.jsonSerializer.Deserialize(sr, typeof(MPlusSeasonIndex)); // De-serialize JSON to C# Classes
                    season = seasonindex.CurrentSeason.Id.ToString(); // Gets current season, used in the next API call
                }
                string json_characterinfo = await this.Call($"{this.Config.APIroot}/profile/wow/character/{realm}/{character}/mythic-keystone-profile/season/{season}{this.Config.PROFILEnamespace}"); // REMEMBER TO UPDATE SEASON
                using (TextReader sr = new StringReader(json_characterinfo))
                {
                    var mplusseasoninfo = (MythicPlusSeasonInfo)Program.jsonSerializer.Deserialize(sr, typeof(MythicPlusSeasonInfo)); // De-serialize JSON to C# Classes
                    foreach (BestRun run in mplusseasoninfo.BestRuns)
                    {
                        data.Add(run);
                    }
                }
                return data.ToString();
            }
            catch { return "None"; }
        }
        private async Task<string> GetAchievements(string character, string realm, string type) // Returns a string to this.ArmoryLookup()
        {
            AchievementsList list = new AchievementsList();
            string json = await this.Call($"{this.Config.APIroot}/profile/wow/character/{realm}/{character}/achievements{this.Config.PROFILEnamespace}");
            using (TextReader sr = new StringReader(json))
            {
                var achievinfo = (AchievementSummary)Program.jsonSerializer.Deserialize(sr, typeof(AchievementSummary)); // De-serialize JSON to C# Classes
                switch (type)
                {
                    case "pve":
                        foreach (Achievement achiev in achievinfo.Achievements)
                        {
                            if (Globals.AchievementsPVE.ContainsKey(achiev.Id)) list.Add(achiev.Id, achiev.AchievementAchievement.Name, type);
                        }
                        break;
                    case "pvp":
                        foreach (Achievement achiev in achievinfo.Achievements)
                        {
                            if (Globals.AchievementsPVP.ContainsKey(achiev.Id)) list.Add(achiev.Id, achiev.AchievementAchievement.Name, type);
                        }
                        break;
                    default:
                        throw new Exception("Invalid type specified.");
                } // End Switch
            }
            return list.ToString();
        }
        //
        // PVP Only Methods
        //
        private async Task<string> GetPVP(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            string output = "";
            Task<string> json2v2 = this.Call($"{this.Config.APIroot}/profile/wow/character/{realm}/{character}/pvp-bracket/2v2{this.Config.PROFILEnamespace}");
            Task<string> json3v3 = this.Call($"{this.Config.APIroot}/profile/wow/character/{realm}/{character}/pvp-bracket/3v3{this.Config.PROFILEnamespace}");
            Task<string> jsonrbg = this.Call($"{this.Config.APIroot}/profile/wow/character/{realm}/{character}/pvp-bracket/rbg{this.Config.PROFILEnamespace}");
            await Task.WhenAll(json2v2, json3v3, jsonrbg); // Allow API calls to run concurrently
            using (TextReader sr = new StringReader(json2v2.Result)) // 2v2
            {
                var v2info = (PvpBracketInfo)Program.jsonSerializer.Deserialize(sr, typeof(PvpBracketInfo)); // De-serialize JSON to C# Classes
                if (v2info.SeasonMatchStatistics?.Played > 0) // Only list brackets played
                {
                    int winpct = 0;
                    if (v2info.SeasonMatchStatistics?.Won > 0) winpct = (int)(((double)v2info.SeasonMatchStatistics.Won / (double)v2info.SeasonMatchStatistics.Played) * (double)100);
                    output += $"* 2v2 Rating: {v2info.Rating} (Won {winpct}%)\n";
                }
            }
            using (TextReader sr = new StringReader(json3v3.Result)) // 3v3
            {
                var v3info = (PvpBracketInfo)Program.jsonSerializer.Deserialize(sr, typeof(PvpBracketInfo)); // De-serialize JSON to C# Classes
                if (v3info.SeasonMatchStatistics?.Played > 0) // Only list brackets played
                {
                    int winpct = 0;
                    if (v3info.SeasonMatchStatistics?.Won > 0) winpct = (int)(((double)v3info.SeasonMatchStatistics.Won / (double)v3info.SeasonMatchStatistics.Played) * (double)100);
                    output += $"* 3v3 Rating: {v3info.Rating} (Won {winpct}%)\n";
                }
            }
            using (TextReader sr = new StringReader(jsonrbg.Result)) // RBG
            {
                var rbginfo = (PvpBracketInfo)Program.jsonSerializer.Deserialize(sr, typeof(PvpBracketInfo)); // De-serialize JSON to C# Classes
                if (rbginfo.SeasonMatchStatistics?.Played > 0) // Only list brackets played
                {
                    int winpct = 0;
                    if (rbginfo.SeasonMatchStatistics?.Won > 0) winpct = (int)(((double)rbginfo.SeasonMatchStatistics.Won / (double)rbginfo.SeasonMatchStatistics.Played) * (double)100);
                    output += $"* RBG Rating: {rbginfo.Rating} (Won {winpct}%)\n";
                }
            }
            if (output.Length == 0) return "None";
            else return output;
        }
        private async Task<string> GetPvpStats(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            string json = await this.Call($"{this.Config.APIroot}/profile/wow/character/{realm}/{character}/statistics{this.Config.PROFILEnamespace}");
            using (TextReader sr = new StringReader(json))
            {
                var stats = (CharacterStatsInfo)Program.jsonSerializer.Deserialize(sr, typeof(CharacterStatsInfo)); // De-serialize JSON to C# Classes
                return $"* Health: {stats.Health}\n* Versatility: {stats.VersatilityDamageDoneBonus}%";
            }
        }
        //
        // ** Blizz API Core Methods **
        //
        private async Task RequestToken() // https://develop.battle.net/documentation/guides/using-oauth/client-credentials-flow
        {
            try
            {
                Program.Log("Requesting new BlizzAPI Token...");
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{this.Config.TOKENroot}/oauth/token"))
                {
                    request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate"); // Request compression
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{this.Config.client_id}:{this.Config.client_secret}"));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                    request.Content = new StringContent("grant_type=client_credentials");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await Program.httpClient.SendAsync(request);
                    using (HttpContent content = response.Content)
                    {
                        var json = content.ReadAsStringAsync().Result;
                        if (!json.Contains("access_token")) throw new Exception($"Error obtaining token:\n{json}\n{response}");
                        else // Load token information
                        {
                            using (var sr = new StringReader(json))
                            {
                                this.Config.Token = (BlizzardAccessToken)Program.jsonSerializer.Deserialize(sr, typeof(BlizzardAccessToken));
                                Program.Log($"BlizzAPI Token obtained! Valid until {this.Config.Token.expire_date} (Auto-Renewing).");
                                this.TokenExpTimer = new Timer();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex.ToString());
            }
        } // End RequestToken()
        private async void TokenExpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Program.Log("BlizzAPI Token expired!");
            await this.RequestToken();
        }
        private async Task CheckToken()
        {
            try
            {
                if (this.CheckToken_RateLimit.Enabled) return;
                this.CheckToken_RateLimit = new Timer(900000); // Rate Limit once every 15 minutes
                this.CheckToken_RateLimit.AutoReset = false;
                this.CheckToken_RateLimit.Start();
                Program.Log("Checking BlizzAPI Token...");
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{this.Config.TOKENroot}/oauth/check_token"))
                {
                    request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate"); // Request compression
                    var contentList = new List<string>();
                    contentList.Add($"token={this.Config.Token.access_token}");
                    request.Content = new StringContent(string.Join("&", contentList));
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await Program.httpClient.SendAsync(request);
                    using (HttpContent content = response.Content)
                    {
                        var json = content.ReadAsStringAsync().Result;
                        if (json.Contains("invalid_token")) throw new Exception($"BlizzAPI Token is no longer valid:\n{json}");
                        else Program.Log($"BlizzAPI Token is valid! Valid until {this.Config.Token.expire_date} (Auto-Renewing).");
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex.ToString());
                await this.RequestToken();
            }
        }
        private async Task<string> Call(string uri) // API Lookup, returns a json string to calling function
        {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), uri))
            {
                request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate"); // Request compression
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {this.Config.Token.access_token}");
                var response = await Program.httpClient.SendAsync(request); // Send GET request, await response
                using (HttpContent content = response.Content) // Parse response
                {
                    return content.ReadAsStringAsync().Result; // return JSON string
                }
            }
        }
    }

}
