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
        private readonly BlizzardConfig Config;
        private BlizzardAccessToken Token;
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
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            })
            .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError() // HttpRequestException, 5XX and 408
            .OrResult(response => (int)response.StatusCode != 200) // Only accept 200 'OK'
            .WaitAndRetryAsync(3, delay => TimeSpan.FromMilliseconds(333)))
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
            var json_summary = await this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/mythic-keystone-profile", Namespace.Profile);
            var summary = JsonSerializer.Deserialize<MythicPlusSummary>(json_summary); // De-serialize JSON to C# Classes
            bool hasCurrentSeason = false;
            if (summary.Seasons is not null) foreach (var season in summary.Seasons)
            {
                if (season.Id == this.MplusSeasonID)
                    {
                        hasCurrentSeason = true;
                        break;
                    }
            }
            if (hasCurrentSeason)
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
            else return "None";
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
            var json_summary = await this.Call($"https://{this.Config.Region}.api.blizzard.com/profile/wow/character/{realm}/{character}/pvp-summary", Namespace.Profile);
            var summary = JsonSerializer.Deserialize<PvpSummary>(json_summary); // De-serialize JSON to C# Classes
            if (summary.Brackets is not null) foreach (var item in summary.Brackets)
            {
                string[] uri = item.Href.ToString().Split('?'); // Strip namespace
                var json = await this.Call(uri[0], Namespace.Profile);
                var bracket = JsonSerializer.Deserialize<PvpBracketInfo>(json); // De-serialize JSON to C# Classes
                switch (bracket.Bracket.Type.ToLower())
                {
                    case "arena_2v2":
                        bracket.Bracket.Type = "2v2";
                        break;
                    case "arena_3v3":
                        bracket.Bracket.Type = "3v3";
                        break;
                    case "battlegrounds":
                        bracket.Bracket.Type = "RBG";
                        break;
                    default:
                        continue;
                }
                if (bracket.SeasonMatchStatistics?.Played > 0) // Only list brackets played
                {
                    int winpct = 0;
                    if (bracket.SeasonMatchStatistics?.Won > 0) winpct = (int)(((double)bracket.SeasonMatchStatistics.Won / (double)bracket.SeasonMatchStatistics.Played) * (double)100);
                    output += $"• {bracket.Bracket.Type} Rating: {bracket.Rating} (Won {winpct}%)\n";
                }
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
                        this.Token = JsonSerializer.Deserialize<BlizzardAccessToken>(json, new JsonSerializerOptions() { IgnoreNullValues = true });
                        if (this.Token?.access_token is null) throw new Exception($"Error obtaining token:\n{response}");
                        this.TokenExpTimer_Start(); // Start Auto-Renewing Timer
                        await Program.Log($"BlizzAPI Token obtained! Valid until {this.Token.expire_date} (Auto-Renewing).");
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
            this.TokenExpTimer = new Timer(this.Token.expires_in * 1000); // Convert seconds to ms
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
                    contentList.Add($"token={this.Token.access_token}");
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
                            await Program.Log($"BlizzAPI Token is valid! Valid until {this.Token.expire_date} (Auto-Renewing).");
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
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {this.Token.access_token}");
                var client = this.ClientFactory.CreateClient("ApiClient");
                using var response = await client.SendAsync(request); // Send HTTP Request
                {
                    return await response.Content.ReadAsStringAsync(); // Return JSON Stream
                }
            }
        }
    }

}
