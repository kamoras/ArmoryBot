using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;


namespace ArmoryBot
{
    public class ArmoryBot
    {
        private readonly DiscordConfig Config;
        private BlizzardAPI BlizzAPI;
        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Services;
        public ArmoryBot()
        {
            this.BlizzAPI = new BlizzardAPI(); // Initializes Blizzard API
            this.Config = JsonSerializer.Deserialize<DiscordConfig>(File.ReadAllText(Globals.DiscordConfigPath)); // Load Config
        }
        public async Task StartupAsync() // Discord bot startup
        {
            this.Client = new DiscordSocketClient();
            this.Commands = new CommandService();
            this.Services = new ServiceCollection().AddSingleton(this.Client).AddSingleton(this.Commands).BuildServiceProvider();
            this.Client.Log += this.Discord_Log; // Set logging method
            this.Client.MessageReceived += this.Discord_HandleCommandAsync; // Set msg received method
            await this.Commands.AddModulesAsync(Assembly.GetEntryAssembly(), this.Services);
            await this.Client.LoginAsync(TokenType.Bot, this.Config.token);
            await this.Client.StartAsync();
            await this.Client.SetGameAsync($"{this.Config.cmdprefix}armory help", null, ActivityType.Listening); // Set Discord Status
        }
#pragma warning disable 4014
#pragma warning disable 1998
        private async Task Discord_HandleCommandAsync(SocketMessage msgParam) // Triggered when a message is received in a channel the bot has visible
        {
            if (msgParam is null) return; // Null check
            if (msgParam is not SocketUserMessage msg) return; // Ignore System/Other messages
            if (msg.Source != MessageSource.User) return; // Only process user messages
            int argPos = 0;
            if (!msg.HasCharPrefix(this.Config.cmdprefix, ref argPos)) return; // Check for cmd prefix
            Task.Run(async delegate // Long running task
            {
                await this.Commands.ExecuteAsync(new ArmoryCommandContext(this.Client, msg, ref this.BlizzAPI, this.Config.cmdprefix), argPos, this.Services);
            });
        }
#pragma warning restore 4014
#pragma warning restore 1998

        private async Task Discord_Log(LogMessage msg) // Discord Logging Method
        {
            await Program.Log(msg.ToString());
        }
    }

    [Group("armory")] // Armory Base Command
    public class ArmoryModule : ModuleBase<ArmoryCommandContext>
    {
        [Command]
        public async Task HandleCMD(params string[] args)
        {
            for (int i = 0; i < args.Length; i++) { args[i] = args[i].Trim().ToLower(); } // Trim,lowercase cmd string
            if (args.Length < 1) { await this.SendErrorResponse("No command arguments provided!"); return; }
            if (args[0] == "help")
            {
                await this.CMD_Help();
                return;
            }
            else if (args[0] == "token")
            {
                await this.CMD_Token();
                return;
            }
            else if (args[0] == "pve" | args[0] == "pvp")
            {
                if (args.Length < 2) { await this.SendErrorResponse("Must provide CharacterName-Realm!"); return; }
                if (args[0] == "pve")
                {
                    await this.CMD_Armory(args[1], LookupType.PVE);
                    return;
                }
                else if (args[0] == "pvp")
                {
                    await this.CMD_Armory(args[1], LookupType.PVP);
                    return;
                }
            }
            else await this.SendErrorResponse("Invalid command entry!");
        }

        private async Task CMD_Armory(string identity, LookupType type) // Main Armory Lookup: [0] = user-realm , [1] = pve/pvp
        {
            try
            {
                await Program.Log($"Armory Command requested by {this.Context.Message.Author}");
                string[] character = identity.Split(new[] { '-' }, 2); // Split CharacterName-Realm. Example: splits Frostchiji-Wyrmrest-Accord into [0]Frostchiji [1]Wyrmrest-Accord (keeps second dash).
                ArmoryData info = await this.Context.API.ArmoryLookup(character[0], character[1], type); // Main Blizzard API Lookup, [0]name [1]realm [1]LookupType.PVE/PVP
                var eb = new EmbedBuilder(); // Build embedded discord msg
                eb.WithTitle(info.CharInfo.Name);
                eb.WithDescription($"{info.CharInfo.ItemLevel} | {info.CharInfo.Renown}");
                switch (type)
                {
                    case LookupType.PVE:
                        if (info.RaidInfo.Raids.Count == 0) eb.AddField("Raids", "None", true); // None placeholder if no raids logged
                        else foreach (RaidItem raid in info.RaidInfo.Raids) // Add a field for each raid
                            {
                                eb.AddField(raid.Name, raid.ToString(), true); // inline, up to 3 columns per row
                            }
                        eb.AddField("M+ Best Runs", info.MythicPlus, false);
                        eb.AddField("PVE Achievements", info.Achievements, false);
                        break;
                    case LookupType.PVP:
                        eb.AddField("Rated PVP", info.PVPRating, false);
                        eb.AddField("PVP Stats", info.PVPStats, false);
                        eb.AddField("PVP Achievements", info.Achievements, false);
                        break;
                }
                eb.WithFooter($"{this.Context.Prefix}armory help | https://github.com/imerzan/ArmoryBot"); // Display help information in footer
                eb.WithThumbnailUrl(info.AvatarUrl); // Set Character Avatar as Thumbnail Picture
                eb.WithUrl(info.CharInfo.ArmoryUrl); // Set Armory URL (Clickable on title)
                await this.Context.Message.Channel.SendMessageAsync("", false, eb.Build()); // Send message to requestor with Armory Info (embed)
            }
            catch (Exception ex)
            {
                await this.SendErrorResponse(ex.ToString());
            }
        }
        private async Task CMD_Token()
        {
            try
            {
                await Program.Log($"Token Command requested by {this.Context.Message.Author}");
                WoWToken token = await this.Context.API.WoWTokenLookup();
                var eb = new EmbedBuilder(); // Build embedded discord msg
                eb.WithTitle("WoW Token");
                eb.AddField("Quote", $"• Price: {token.Price}\n• Last Updated: {token.Last_Updated}", false);
                eb.WithFooter($"{this.Context.Prefix}armory help | https://github.com/imerzan/ArmoryBot"); // Display help information in footer
                eb.WithThumbnailUrl(Globals.WowTokenIconUrl);
                await this.Context.Message.Channel.SendMessageAsync("", false, eb.Build()); // Send embed message to requestor
            }
            catch (Exception ex)
            {
                await this.SendErrorResponse(ex.ToString());
            }
        }
        private async Task CMD_Help() // Display usage help to requestor
        {
            try
            {
                await Program.Log($"Help Command requested by {this.Context.Message.Author}");
                var eb = new EmbedBuilder(); // Build embedded discord msg
                eb.WithTitle("ArmoryBot Help");
                eb.WithDescription($"• Armory Lookup: `{this.Context.Prefix}armory pve/pvp CharacterName-Realm`\n• WoW Token Lookup: `{this.Context.Prefix}armory token`\n\n**NOTE:** Spaces in realm name should have a dash ' - '");
                eb.WithUrl("https://github.com/imerzan/ArmoryBot");
                eb.WithFooter($"Info/Feedback: https://github.com/imerzan/ArmoryBot");
                eb.WithThumbnailUrl(Globals.TomeIconUrl);
                await this.Context.Message.Channel.SendMessageAsync("", false, eb.Build());
            }
            catch { }
        }

        private async Task SendErrorResponse(string error) // Logs error,exception & sends a generic Error Response Msg to the end-user
        {
            try
            {
                await Program.Log($"{this.Context.Message}: {error}\n**Sending generic error notification to {this.Context.Message.Author}**");
                var eb = new EmbedBuilder();
                eb.WithDescription($"**ERROR** executing command `{this.Context.Message}`\nPlease double check your spelling and try again.");
                eb.WithFooter($"{this.Context.Prefix}armory help | https://github.com/imerzan/ArmoryBot"); // Display help information in footer
                eb.WithThumbnailUrl(Globals.QuestionMarkIconUrl);
                await this.Context.Message.Channel.SendMessageAsync("", false, eb.Build());
            }
            catch { }
        }
    }
    public class ArmoryCommandContext : ICommandContext // Custom Command Context to pass BlizzardAPI reference, using DI
    {
        public readonly BlizzardAPI API;
        public readonly char Prefix;
        public ArmoryCommandContext(DiscordSocketClient _client, SocketUserMessage _msg, ref BlizzardAPI _api, char _prefix)
        {
            this.Client = _client;
            this.Guild = (_msg.Channel as IGuildChannel)?.Guild;
            this.Channel = _msg.Channel;
            this.User = _msg.Author;
            this.Message = _msg;
            this.API = _api;
            this.Prefix = _prefix;
        }
        public IDiscordClient Client { get; }
        public IUserMessage Message { get; }
        public IUser User { get; }
        public IGuild Guild { get; }
        public IMessageChannel Channel { get; }
    }
}
