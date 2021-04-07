using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
#pragma warning disable 4014
#pragma warning disable 1998

namespace ArmoryBot
{
    public class DiscordBot
    {
        private readonly DiscordConfig Config;
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        public DiscordBot()
        {
            using (StreamReader json = File.OpenText(Globals.DiscordConfigPath)) // Load Config
            {
                this.Config = (DiscordConfig)Program.jsonSerializer.Deserialize(json, typeof(DiscordConfig));
            }
        }
        public async Task StartupAsync() // Discord bot startup
        {
            this._client = new DiscordSocketClient();
            this._commands = new CommandService();
            this._services = new ServiceCollection().AddSingleton(this._client).AddSingleton(this._commands).BuildServiceProvider();
            this._client.Log += this.Discord_Log; // Set logging method
            this._client.MessageReceived += this.Discord_HandleCommandAsync; // Set msg received method
            await this._commands.AddModulesAsync(Assembly.GetEntryAssembly(), this._services);
            await this._client.LoginAsync(TokenType.Bot, this.Config.token);
            await this._client.StartAsync();
            await this._client.SetGameAsync($"{this.Config.cmdprefix}armory help", null, ActivityType.Listening); // Set Discord Status
        }
        private async Task Discord_HandleCommandAsync(SocketMessage msgParam) // Triggered when a message is received in a channel the bot has visible
        {
            if (msgParam is null) return; // Null check
            if (!(msgParam is SocketUserMessage msg)) return; // Ignore System/Other messages
            if (msg.Source != MessageSource.User) return; // Only process user messages
            int argPos = 0;
            if (!msg.HasCharPrefix(this.Config.cmdprefix, ref argPos)) return; // Check for cmd prefix
            this._commands.ExecuteAsync(new SocketCommandContext(this._client, msg), argPos, this._services); // Do not await
        }

        private async Task Discord_Log(LogMessage msg) // Discord Logging Method
        {
            Program.Log(msg.ToString());
        }
    }
    [Group("armory")] // Armory Base Command
    public class ArmoryModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task HandleCMD(params string[] args)
        {
            char prefix = this.Context.Message.ToString()[0]; // Get prefix char (ex: !)
            for (int i = 0; i < args.Length; i++) { args[i] = args[i].Trim().ToLower(); } // Trim cmd string
            if (args.Length < 1) return; // Args Length Check
            if (args[0] == "help")
            {
                await this.CMD_Help(prefix);
                return;
            }
            else if (args[0] == "token")
            {
                await this.CMD_Token(prefix);
                return;
            }
            if (args.Length < 2) return;
            await this.CMD_Armory(args, prefix);
        }
        private async Task CMD_Armory(string[] args, char prefix) // Main Armory Lookup: [0] = user-realm , [1] = pve/pvp
        {
            try
            {
                Program.Log($"Armory Command requested by {this.Context.Message.Author}");
                if (args[1] != "pve" & args[1] != "pvp") throw new Exception($"Type must be either pve or pvp. '{args[2]}' is invalid."); // Make sure lookup type is valid
                string[] character = args[0].Split(new[] { '-' }, 2); // Split CharacterName-Realm. Example: splits Frostchiji-Wyrmrest-Accord into [0]Frostchiji [1]Wyrmrest-Accord (keeps second dash).
                ArmoryData info = await Program.blizzardAPI.ArmoryLookup(character[0], character[1], args[1]); // Main Blizzard API Lookup
                var eb = new EmbedBuilder(); // Build embedded discord msg
                eb.WithTitle(info.CharInfo.Name);
                eb.WithDescription($"{info.CharInfo.ItemLevel} | {info.CharInfo.Renown}");
                switch (args[1])
                {
                    case "pve":
                        if (info.RaidInfo.Raids.Count == 0) eb.AddField("Raids", "None", true); // None placeholder if no raids logged
                        else foreach (RaidItem raid in info.RaidInfo.Raids) // Add a field for each raid
                            {
                                eb.AddField(raid.Name, raid.ToString(), true); // inline, up to 3 columns per row
                            }
                        eb.AddField("Mythic+", info.MythicPlus, false);
                        eb.AddField("PVE Achievements", info.Achievements, false);
                        break;
                    case "pvp":
                        eb.AddField("Rated PVP", info.PVPRating, false);
                        eb.AddField("PVP Stats", info.PVPStats, false);
                        eb.AddField("PVP Achievements", info.Achievements, false);
                        break;
                }
                eb.WithFooter($"{prefix}armory help | http://github.com/imerzan/ArmoryBot"); // Display help information in footer
                eb.WithThumbnailUrl(info.AvatarUrl); // Set Character Avatar as Thumbnail Picture
                eb.WithUrl(info.CharInfo.ArmoryUrl); // Set Armory URL (Clickable on title)
                await this.Context.Message.Channel.SendMessageAsync("", false, eb.Build()); // Send message to requestor with Armory Info (embed)
            }
            catch (Exception ex)
            {
                Program.Log($"{this.Context.Message}: {ex} **Sending generic error notification to {this.Context.Message.Author}**");
                try { await this.Context.Message.Channel.SendMessageAsync($"**ERROR** looking up `{this.Context.Message}`\nSee `{prefix}armory help`"); } catch { } // Generic error notification to user
            }
        }
        private async Task CMD_Token(char prefix)
        {
            try
            {
                Program.Log($"Token Command requested by {this.Context.Message.Author}");
                WoWToken token = await Program.blizzardAPI.WoWTokenLookup();
                var eb = new EmbedBuilder(); // Build embedded discord msg
                eb.WithTitle("WoW Token");
                eb.AddField("Quote", $"• Price: {token.Price}\n• Last Updated: {token.Last_Updated}", false);
                eb.WithFooter($"{prefix}armory help | http://github.com/imerzan/ArmoryBot"); // Display help information in footer
                eb.WithThumbnailUrl(token.TokenAvatarUrl);
                await this.Context.Message.Channel.SendMessageAsync("", false, eb.Build()); // Send embed message to requestor
            }
            catch (Exception ex)
            {
                Program.Log($"{this.Context.Message}: {ex} **Sending generic error notification to {this.Context.Message.Author}**");
                try { await this.Context.Message.Channel.SendMessageAsync($"**ERROR** looking up WoW Token Data.\nSee `{prefix}armory help`"); } catch { } // Generic error notification to user
            }
        }
        private async Task CMD_Help(char prefix) // Display usage help to requestor
        {
            try
            {
                Program.Log($"Help Command requested by {this.Context.Message.Author}");
                var eb = new EmbedBuilder(); // Build embedded discord msg
                eb.WithTitle("ArmoryBot");
                eb.WithDescription($"Armory Lookup: `{prefix}armory CharacterName-Realm pve/pvp`\nWoW Token Lookup: `{prefix}armory token`\n**NOTE:** Spaces in realm name should have a dash ' - '\n\nLearn more about ArmoryBot at: http://github.com/imerzan/ArmoryBot/");
                eb.WithUrl("http://github.com/imerzan/ArmoryBot");
                await this.Context.User.GetOrCreateDMChannelAsync(); // Send user a DM Response
                await this.Context.User.SendMessageAsync(this.Context.User.Mention, false, eb.Build());
            }
            catch { return; }
        }
    }
}
