using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
            this._services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();
            this._client.Log += this.Discord_Log; // Set logging method
            this._client.MessageReceived += this.Discord_HandleCommandAsync; // Set msg received method
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await this._client.LoginAsync(TokenType.Bot, this.Config.token);
            await this._client.StartAsync();
        }
        private async Task Discord_HandleCommandAsync(SocketMessage msgParam) // Triggered when a message is received in a channel the bot has visible
        {
            if (msgParam is null) return; // Null check
            if (!(msgParam is SocketUserMessage msg)) return; // Ignore System/Other messages
            if (msg.Source != MessageSource.User) return; // Only process user messages
            int argPos = 0;
            if (!msg.HasCharPrefix(this.Config.cmdprefix, ref argPos)) return; // Check for cmd prefix
            this._commands.ExecuteAsync(new SocketCommandContext(_client, msg), argPos, _services); // Do not await
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
        public async Task HandleCMD(params string[] cmd)
        {
            string prefix = Context.Message.ToString()[0].ToString(); // Get prefix char (ex: !)
            for (int i = 0; i < cmd.Length; i++) { cmd[i] = cmd[i].Trim().ToLower(); } // Trim cmd string
            if (cmd.Length < 1) return; // Args Length Check
            if (cmd[0] == "help") await this.CMD_Help(prefix);
            else if (cmd[0] == "token") await this.CMD_Token(prefix);
            if (cmd.Length < 2) return;
            await this.CMD_Armory(cmd, prefix);
        }
        private async Task CMD_Armory(string[] cmd, string prefix) // Main Armory Lookup: [0] = user-realm , [1] = pve/pvp
        {
            try
            {
                Program.Log($"Armory Command requested by {Context.Message.Author}");
                if (cmd[1] != "pve" & cmd[1] != "pvp") throw new Exception($"Type must be either pve or pvp. '{cmd[2]}' is invalid."); // Make sure lookup type is valid
                string[] character = cmd[0].Split(new[] { '-' }, 2); // Split CharacterName-Realm. Example: splits Frostchiji-Wyrmrest-Accord into [0]Frostchiji [1]Wyrmrest-Accord (keeps second dash).
                ArmoryData info = await Program.blizzardAPI.ArmoryLookup(character[0], character[1], cmd[1]); // Main Blizzard API Lookup
                var eb = new EmbedBuilder(); // Build embedded discord msg
                eb.WithTitle(info.CharInfo.Name);
                eb.WithDescription($"{info.CharInfo.ItemLevel} | {info.CharInfo.Renown}");
                switch (cmd[1])
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
                await Context.Message.Channel.SendMessageAsync("", false, eb.Build()); // Send message to requestor with Armory Info (embed)
            }
            catch (Exception ex)
            {
                Program.Log($"{Context.Message}: {ex} **Sending generic error notification to {Context.Message.Author}**");
                try { await Context.Message.Channel.SendMessageAsync($"**ERROR** looking up `{Context.Message}`\nSee `{prefix}armory help`"); } catch { } // Generic error notification to user
            }
        }
        private async Task CMD_Token(string prefix)
        {
            try
            {
                Program.Log($"Token Command requested by {Context.Message.Author}");
                WoWToken token = await Program.blizzardAPI.WoWTokenLookup();
                var eb = new EmbedBuilder(); // Build embedded discord msg
                eb.WithTitle("WoW Token");
                eb.AddField("Quote", $"Price: {token.Price}\nLast Updated: {token.Last_Updated}", false);
                eb.WithThumbnailUrl(token.TokenAvatarUrl);
                await Context .Message.Channel.SendMessageAsync("", false, eb.Build()); // Send embed message to requestor
            }
            catch (Exception ex)
            {
                Program.Log($"{Context.Message}: {ex} **Sending generic error notification to {Context.Message.Author}**");
                try { await Context .Message.Channel.SendMessageAsync($"**ERROR** looking up WoW Token Data.\nSee `{prefix}armory help`"); } catch { } // Generic error notification to user
            }
        }
        private async Task CMD_Help(string prefix) // Display usage help to requestor
        {
            try
            {
                Program.Log($"Help Command requested by {Context.Message.Author}");
                await Context .Message.Channel.SendMessageAsync($"ArmoryBot Usage: `{prefix}armory CharacterName-Realm pve/pvp`\n**NOTE:** Spaces in realm name should have a dash ' - '\n\nLearn more about ArmoryBot at: http://github.com/imerzan/ArmoryBot/");
            }
            catch { return; }
        }
    }
}
