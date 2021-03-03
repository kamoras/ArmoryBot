using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ArmoryBot
{
    public class ArmoryBot
    {
        private DiscordConfig discordConfig;
        private BlizzardAPI BlizzAPI;
        private DiscordSocketClient Discord;
        public ArmoryBot()
        {
            this.BlizzAPI = new BlizzardAPI(); // Initializes BlizzardAPI Class, will load config and obtain a Blizz API Token
            using (StreamReader json = File.OpenText(Globals.DiscordConfigPath)) // Load Discord Config from json Config file
            {
                var serializer = new JsonSerializer();
                this.discordConfig = new DiscordConfig();
                this.discordConfig = (DiscordConfig)serializer.Deserialize(json, typeof(DiscordConfig));
            }
        }
        public async Task Startup() // Discord bot startup
        {
            this.Discord = new DiscordSocketClient();
            this.Discord.Log += this.Log; // Set logging method
            this.Discord.MessageReceived += this.MsgReceived; // Set msg received method
            await this.Discord.LoginAsync(TokenType.Bot, this.discordConfig.token);
            await this.Discord.StartAsync();
        }
        private async Task MsgReceived(SocketMessage msgParam) // Triggered when a message is received in a channel the bot has visible
        {
            if (msgParam is null) return; // Null check
            if (!(msgParam is SocketUserMessage msg)) return; // Ignore System/Other Bot Messages
            if (msg.Source != MessageSource.User) return; // Only process user messages
            int argPos = 0;
            bool HasPrefix = msg.HasCharPrefix(this.discordConfig.cmdprefix, ref argPos); // Check for cmd prefix
            bool IsDM = msg.Channel.GetType() == typeof(SocketDMChannel); // Check if DM Channel
            switch (HasPrefix)
            {
                case false: // No prefix used
                    if (IsDM) this.CMD_Help(msg); // Respond to DM
                    return;
                case true: // Prefix used
                    this.Handle_CMD(msg); // Handle CMD
                    return;
            }
        }
        private async Task Handle_CMD(SocketUserMessage msg) // Handle prefixed message, determine which command to run
        {
            try
            {
                string[] cmd = msg.ToString().Split(' '); // Split msg string by spaces
                for (int i = 0; i < cmd.Length; i++) { cmd[i] = cmd[i].Trim().ToLower(); } // Trim cmd string
                if (cmd[0] == this.discordConfig.Prefix + "armory") // base armory command
                {
                    if (cmd.Length < 2) return; // Arg Length Check
                    if (cmd[1] == "help") // Help command
                    {
                        this.CMD_Help(msg); // Do not await
                        return;
                    }
                    // Armory Lookup Command
                    if (cmd.Length < 3) return; // Arg Length Check
                    this.CMD_Armory(msg, cmd); // Do not await
                    return;
                }
            }
            catch (Exception ex)
            {
                Program.Log($"{msg}: {ex}");
            }
        }
        private async Task CMD_Help(SocketUserMessage msg) // Display usage help to requestor
        {
            try
            {
                msg.Channel.SendMessageAsync($"ArmoryBot Usage: `{this.discordConfig.Prefix}armory CharacterName-Realm pve/pvp`\n**NOTE:** Spaces in realm name should have a dash ' - '\n\nLearn more about ArmoryBot at: http://github.com/imerzan/ArmoryBot/");
            }
            catch (Exception ex)
            {
                Program.Log($"{msg}: {ex}");
            }
        }
        private async Task CMD_Armory(SocketUserMessage msg, string[] cmd) // Main Armory Lookup: [0] = command, [1] = user-realm , [2] = pve/pvp
        {
            try
            {
                Program.Log($"Armory Lookup requested by {msg.Author}");
                string[] character = cmd[1].Split(new[] { '-' }, 2); // Split CharacterName-Realm. Example: splits Frostchiji-Wyrmrest-Accord into [0]Frostchiji [1]Wyrmrest-Accord (keeps second dash).
                ArmoryData info = await BlizzAPI.ArmoryLookup(character[0], character[1], cmd[2]); // Main Blizzard API Lookup
                var eb = new EmbedBuilder(); // Build embedded discord msg
                eb.WithTitle(info.CharacterInfo);
                switch (cmd[2])
                {
                    case "pve":
                        if (info.RaidInfo.Raids.Count == 0) eb.AddField("Raids", "None", true); // None placeholder if no raids logged
                        else foreach (RaidItem raid in info.RaidInfo.Raids) // Add a field for each raid
                        {
                            eb.AddField(raid.Name, raid.ToString(), true);
                        }
                        eb.AddField("Mythic+", info.MythicPlus, true);
                        eb.AddField("PVE Achievements", info.Achievements, true);
                        break;
                    case "pvp":
                        eb.AddField("Rated PVP", info.PVPRating, true);
                        eb.AddField("PVP Stats", info.PVPStats, true);
                        eb.AddField("PVP Achievements", info.Achievements, true);
                        break;
                    default:
                        throw new Exception("Invalid type specified.");
                }
                eb.WithFooter($"{this.discordConfig.Prefix}armory help | http://github.com/imerzan/ArmoryBot"); // Display help information in footer
                eb.ThumbnailUrl = info.AvatarUrl; // Set Character Avatar as Thumbnail Picture
                msg.Channel.SendMessageAsync("", false, eb.Build()); // Send message to requestor with Armory Info (embed)
            }
            catch (Exception ex)
            {
                Program.Log($"{msg}: {ex} **Sending error notification to requestor**");
                try { msg.Channel.SendMessageAsync($"**ERROR** looking up: {msg}\nSee `{this.discordConfig.Prefix}armory help`"); } catch { } // Generic error notification to user
            }
        }
        private async Task Log(LogMessage msg)
        {
            Program.Log(msg.ToString());
        }
    }

}
