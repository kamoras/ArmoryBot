using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Text.Json;
using Polly;
using Polly.Extensions.Http;

namespace ArmoryBot
{
    public class BlizzardAPI
    {
        private BlizzardConfig Config;
        private Timer TokenExpTimer;
        private readonly IServiceCollection Services;
        private readonly IHttpClientFactory ClientFactory;
        private long MplusSeasonID = -1; // Stores current M+ Season as obtained by this.GetGameData() 
        private int MplusDungeonCount = -1; // Stores count of M+ eligible dungeons as obtained by this.GetGameData() 
        public BlizzardAPI()
        {
            this.Services = new ServiceCollection();
            this.Services.AddHttpClient("ApiClient", client =>
            {
                client.Timeout = new TimeSpan(0, 0, 15);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            })
            .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError() // HttpRequestException, 5XX and 408
            .OrResult(response => (int)response.StatusCode != 200) // 200 OK
            .WaitAndRetryAsync(3, delay => TimeSpan.FromMilliseconds(250)))
            .ConfigurePrimaryHttpMessageHandler(handler =>
            {
                return new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            });
            this.ClientFactory = this.Services.BuildServiceProvider().GetService<IHttpClientFactory>();
            this.Config = JsonSerializer.Deserialize<BlizzardConfig>(File.ReadAllText(Globals.BlizzardConfigPath)); // Load Config
#pragma warning disable 4014
            this.RequestToken(); // Obtain initial BlizzAPI Token (cannot await in constructor)
#pragma warning restore 4014
        }

        public async Task<ArmoryData> ArmoryLookup(string character, string realm, LookupType type) // Main Armory Lookup Method exposed to ArmoryBot.cs
        {
            try
            {
                ArmoryData info = new ArmoryData(); // This method makes a number of separate API Calls. All the data is stored to this ArmoryData class to easily pass to the calling function.
                Task<CharacterInfo> CharInfo = this.GetCharacter(character, realm); // Gets basic character info (Player name, race, class, spec, etc.)
                Task<string> AvatarInfo = this.GetAvatar(character, realm); // Gets character avatar image URL
                Task<string> AchievInfo = this.GetAchievements(character, realm, type); // Gets Achievements
                switch (type)
                {
                    case LookupType.PVE:
                        Task<RaidData> RaidInfo = this.GetRaids(character, realm); // Gets all raid info from Current Expansion
                        Task<string> MythicPlus = this.GetMythicPlus(character, realm); // Gets all M+ info from Current Season
                        await Task.WhenAll(RaidInfo, MythicPlus); // Wait for all PVE tasks to finish up
                        info.RaidInfo = RaidInfo.Result; info.MythicPlus = MythicPlus.Result; // Move results into class:ArmoryData
                        break;
                    case LookupType.PVP:
                        Task<string> PvpInfo = this.GetPVP(character, realm); // Gets all rated PVP bracket info
                        Task<string> PVPStats = this.GetPvpStats(character, realm); // Gets all PVP Character Stats info (Versatility,etc.)
                        await Task.WhenAll(PvpInfo, PVPStats); // Wait for all PVP tasks to finish up
                        info.PVPRating = PvpInfo.Result; info.PVPStats = PVPStats.Result; // Move results into class:ArmoryData
                        break;
                }
                await Task.WhenAll(CharInfo, AvatarInfo, AchievInfo); // Wait for all other tasks to finish up
                info.CharInfo = CharInfo.Result; info.AvatarUrl = AvatarInfo.Result; info.Achievements = AchievInfo.Result; // Move results into class:ArmoryData
                return info; // Return class:ArmoryData to calling function
            }
            catch
            {
                await this.CheckToken(); // Make sure token is valid
                throw; // Re-throw exception, will be caught in calling function
            }
        }
        public async Task<WoWToken> WoWTokenLookup() // Gets current WoW token price and returns to requestor
        {
            try
            {
                WoWToken info = new WoWToken();
                var json = await this.Call($"https://{this.Config.Region}.api.blizzard.com/data/wow/token/index", Namespace.Dynamic);
                var wowtoken = JsonSerializer.Deserialize<WoWTokenJson>(json); // De-serialize JSON to C# Classes
                info.Price = (wowtoken.Price / 10000).ToString("N0");
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                DateTime lastupdate = origin.AddMilliseconds(wowtoken.LastUpdatedTimestamp).ToUniversalTime();
                TimeSpan span = DateTime.Now.ToUniversalTime() - lastupdate;
                info.Last_Updated = $"{lastupdate} ({span.Minutes} minutes ago)";
                return info;
            }
            catch
            {
                await this.CheckToken(); // Make sure *access* token is valid
                throw; // Re-throw exception, will be caught in calling function
            }
        }
        //
        // Shared/PVE Methods
        //
        private async Task<CharacterInfo> GetCharacter(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            CharacterInfo info = new CharacterInfo();
            var json = await this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}", Namespace.Profile);
            var charinfo = JsonSerializer.Deserialize<CharacterSummary>(json); // De-serialize JSON to C# Classes
            info.Name = $"{charinfo.Name} {charinfo.Level} {charinfo.Race.Name} {charinfo.ActiveSpec.Name} {charinfo.CharacterClass.Name}";
            info.ItemLevel = $"\n**Item Level: {charinfo.EquippedItemLevel}**";
            info.Renown = $"**Renown: {charinfo.CovenantProgress?.RenownLevel} {charinfo.CovenantProgress?.ChosenCovenant.Name}**";
            info.ArmoryUrl = $"https://worldofwarcraft.com/character/{this.Config.Region}/{realm}/{character}";
            return info;
        }
        private async Task<string> GetAvatar(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            var json = await this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/character-media", Namespace.Profile);
            var charmedia = JsonSerializer.Deserialize<CharacterMedia>(json); // De-serialize JSON to C# Classes
            foreach (Asset asset in charmedia.Assets)
            {
                if (asset.Key.ToLower() == "avatar") return asset.Value.ToString();
            }
            throw new NullReferenceException("Avatar value is null."); // Raise exception if unable to get Character Avatar
        }
        private async Task<RaidData> GetRaids(string character, string realm) // Returns class:RaidData to this.ArmoryLookup()
        {
            RaidData data = new RaidData(this.Config.locale);
            var json = await this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/encounters/raids", Namespace.Profile);
            var raidinfo = JsonSerializer.Deserialize<RaidInfo>(json); // De-serialize JSON to C# Classes
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
            return data;
        }

        private async Task<string> GetMythicPlus(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            try // This section will 404 Not found if no M+ completed, use try/catch
            {
                MythicPlusData data = new MythicPlusData(this.MplusDungeonCount);
                var json = await this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/mythic-keystone-profile/season/{this.MplusSeasonID}", Namespace.Profile);
                var mplusseasoninfo = JsonSerializer.Deserialize<MythicPlusSeasonInfo>(json); // De-serialize JSON to C# Classes
                foreach (BestRun run in mplusseasoninfo.BestRuns)
                {
                    data.Add(run);
                }
                return data.ToString();
            }
            catch { return "None"; }
        }
        private async Task<string> GetAchievements(string character, string realm, LookupType type) // Returns a string to this.ArmoryLookup()
        {
            AchievementsList list = new AchievementsList();
            var json = await this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/achievements", Namespace.Profile);
            var achievinfo = JsonSerializer.Deserialize<AchievementSummary>(json); // De-serialize JSON to C# Classes
            switch (type)
            {
                case LookupType.PVE:
                    foreach (Achievement achiev in achievinfo.Achievements)
                    {
                        if (Globals.AchievementsPVE.ContainsKey(achiev.Id)) list.Add(achiev.Id, achiev.AchievementAchievement.Name, type);
                    }
                    break;
                case LookupType.PVP:
                    foreach (Achievement achiev in achievinfo.Achievements)
                    {
                        if (Globals.AchievementsPVP.ContainsKey(achiev.Id)) list.Add(achiev.Id, achiev.AchievementAchievement.Name, type);
                    }
                    break;
            } // End Switch
            return list.ToString();
        }
        //
        // PVP Only Methods
        //
        private async Task<string> GetPVP(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            string output = "";
            Task<string> json2v2 = this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/pvp-bracket/2v2", Namespace.Profile);
            Task<string> json3v3 = this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/pvp-bracket/3v3", Namespace.Profile);
            Task<string> jsonrbg = this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/pvp-bracket/rbg", Namespace.Profile);
            await Task.WhenAll(json2v2, json3v3, jsonrbg); // Allow API calls to run concurrently
            var v2info = JsonSerializer.Deserialize<PvpBracketInfo>(json2v2.Result); // De-serialize JSON to C# Classes
            if (v2info.SeasonMatchStatistics?.Played > 0) // Only list brackets played
            {
                int winpct = 0;
                if (v2info.SeasonMatchStatistics?.Won > 0) winpct = (int)(((double)v2info.SeasonMatchStatistics.Won / (double)v2info.SeasonMatchStatistics.Played) * (double)100);
                output += $"• 2v2 Rating: {v2info.Rating} (Won {winpct}%)\n";
            }
            var v3info = JsonSerializer.Deserialize<PvpBracketInfo>(json3v3.Result); // De-serialize JSON to C# Classes
            if (v3info.SeasonMatchStatistics?.Played > 0) // Only list brackets played
            {
                int winpct = 0;
                if (v3info.SeasonMatchStatistics?.Won > 0) winpct = (int)(((double)v3info.SeasonMatchStatistics.Won / (double)v3info.SeasonMatchStatistics.Played) * (double)100);
                output += $"• 3v3 Rating: {v3info.Rating} (Won {winpct}%)\n";
            }
            var rbginfo = JsonSerializer.Deserialize<PvpBracketInfo>(jsonrbg.Result); // De-serialize JSON to C# Classes
            if (rbginfo.SeasonMatchStatistics?.Played > 0) // Only list brackets played
            {
                int winpct = 0;
                if (rbginfo.SeasonMatchStatistics?.Won > 0) winpct = (int)(((double)rbginfo.SeasonMatchStatistics.Won / (double)rbginfo.SeasonMatchStatistics.Played) * (double)100);
                output += $"• RBG Rating: {rbginfo.Rating} (Won {winpct}%)\n";
            }
            if (output.Length == 0) return "None";
            else return output;
        }
        private async Task<string> GetPvpStats(string character, string realm) // Returns a string to this.ArmoryLookup()
        {
            var json = await this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/statistics", Namespace.Profile);
            var stats = JsonSerializer.Deserialize<CharacterStatsInfo>(json); // De-serialize JSON to C# Classes
            return $"• Health: {stats.Health.ToString("N0")}\n• Versatility: {stats.VersatilityDamageDoneBonus}%";
        }
        //
        // ** Game Data API 
        //
        private async Task GetGameData() // Gets Static/Dynamic assets that can be stored longer term
        {
            Task<string> json_season = this.Call($"https://{this.Config.Region}.api.blizzard.com/data/wow/mythic-keystone/season/index", Namespace.Dynamic);
            Task<string> json_dungeon = this.Call($"https://{this.Config.Region}.api.blizzard.com/data/wow/mythic-keystone/dungeon/index", Namespace.Dynamic);
            await Task.WhenAll(json_season, json_dungeon); // Wait for all tasks to finish up
            var seasonindex = JsonSerializer.Deserialize<MPlusSeasonIndex>(json_season.Result); // De-serialize JSON to C# Classes
            this.MplusSeasonID = seasonindex.CurrentSeason.Id;
            var dungeonindex = JsonSerializer.Deserialize<AllDungeons>(json_dungeon.Result); // De-serialize JSON to C# Classes
            int count = 0;
            foreach (Dungeon dungeon in dungeonindex.Dungeons) count++;
            this.MplusDungeonCount = count;
        }
        //
        // ** Blizz API Core Methods **
        //
        private async Task RequestToken() // https://develop.battle.net/documentation/guides/using-oauth/client-credentials-flow
        {
            try
            {
                await Program.Log("Requesting new BlizzAPI Token...");
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"https://{this.Config.Region}.battle.net/oauth/token"))
                {
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{this.Config.client_id}:{this.Config.client_secret}"));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                    request.Content = new StringContent("grant_type=client_credentials");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var client = this.ClientFactory.CreateClient("ApiClient");
                    using var response = await client.SendAsync(request); // Send HTTP request
                    {
                        var json = await response.Content.ReadAsStringAsync(); // Store json response
                        this.Config.Token = JsonSerializer.Deserialize<BlizzardAccessToken>(json, new JsonSerializerOptions() { IgnoreNullValues = true });
                        if (this.Config.Token.access_token is null) throw new Exception($"Error obtaining token:\n{response}");
                        this.TokenExpTimer_Start(); // Start Auto-Renewing Timer
                        await Program.Log($"BlizzAPI Token obtained! Valid until {this.Config.Token.expire_date} (Auto-Renewing).");
                        await this.GetGameData(); // Update Dynamic Assets
                    }
                }
            }
            catch (Exception ex)
            {
                await Program.Log(ex.ToString());
            }
        } // End RequestToken()

        private void TokenExpTimer_Start()
        {
            this.TokenExpTimer = new Timer(this.Config.Token.expires_in * 1000); // Convert seconds to ms
            this.TokenExpTimer.AutoReset = false;
            this.TokenExpTimer.Elapsed += this.TokenExpTimer_Elapsed; // Set elapsed event method
            this.TokenExpTimer.Start(); // Starts Auto-Renewing Timer for BlizzAPI Token
        }
        private async void TokenExpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Program.Log("BlizzAPI Token expired!");
            await this.RequestToken();
        }

        private async Task CheckToken()
        {
            try
            {
                await Program.Log("Checking BlizzAPI Token...");
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"https://{this.Config.Region}.battle.net/oauth/check_token"))
                {
                    var contentList = new List<string>();
                    contentList.Add($"token={this.Config.Token.access_token}");
                    request.Content = new StringContent(string.Join("&", contentList));
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var client = this.ClientFactory.CreateClient("ApiClient");
                    using var response = await client.SendAsync(request); // Send HTTP Request
                    {
                        var json = await response.Content.ReadAsStringAsync(); // Store JSON
                        var result = JsonSerializer.Deserialize<CheckTokenResult>(json, new JsonSerializerOptions() { IgnoreNullValues = true });
                        if (result.ClientId is null) throw new Exception($"BlizzAPI Token is no longer valid!\n{response}");
                        else
                        {
                            await Program.Log($"BlizzAPI Token is valid! Valid until {this.Config.Token.expire_date} (Auto-Renewing).");
                            if (this.MplusSeasonID == -1 | this.MplusDungeonCount == -1)
                                await this.GetGameData(); // Make sure dynamic assets are set
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Program.Log(ex.ToString());
                await this.RequestToken();
            }
        }
        private async Task<string> Call(string uri, Namespace space) // API Lookup, returns a json string to calling function
        {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), uri + $"?locale={this.Config.locale}"))
            {
                switch (space)
                {
                    case Namespace.Profile:
                        request.Headers.TryAddWithoutValidation("Battlenet-Namespace", $"profile-{this.Config.Region}");
                        break;
                    case Namespace.Static:
                        request.Headers.TryAddWithoutValidation("Battlenet-Namespace", $"static-{this.Config.Region}");
                        break;
                    case Namespace.Dynamic:
                        request.Headers.TryAddWithoutValidation("Battlenet-Namespace", $"dynamic-{this.Config.Region}");
                        break;
                }
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {this.Config.Token.access_token}");
                var client = this.ClientFactory.CreateClient("ApiClient");
                using var response = await client.SendAsync(request); // Send HTTP Request
                {
                    return await response.Content.ReadAsStringAsync(); // Return JSON Stream
                }
            }
        }
    }

}
