using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ArmoryBot
{
    public class ArmoryBot
    {
        private DiscordConfig Config;
        private readonly BlizzardAPI BlizzAPI = new BlizzardAPI(); // Initializes Blizzard API
        private DiscordSocketClient Discord;
        public ArmoryBot()
        {
            using (StreamReader json = File.OpenText(Globals.DiscordConfigPath)) // Load Config
            {
                this.Config = (DiscordConfig)Program.jsonSerializer.Deserialize(json, typeof(DiscordConfig));
            }
        }
        public async Task Discord_Startup() // Discord bot startup
        {
            this.Discord = new DiscordSocketClient();
            this.Discord.Log += this.Discord_Log; // Set logging method
            this.Discord.MessageReceived += this.Discord_MsgReceived; // Set msg received method
            await this.Discord.LoginAsync(TokenType.Bot, this.Config.token);
            await this.Discord.StartAsync();
        }
        private async Task Discord_MsgReceived(SocketMessage msgParam) // Triggered when a message is received in a channel the bot has visible
        {
            if (msgParam is null) return; // Null check
            if (!(msgParam is SocketUserMessage msg)) return; // Ignore System/Other Bot Messages
            if (msg.Source != MessageSource.User) return; // Only process user messages
            int argPos = 0;
            bool HasPrefix = msg.HasCharPrefix(this.Config.cmdprefix, ref argPos); // Check for cmd prefix
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
                if (cmd[0] == this.Config.Prefix + "armory") // Base Prefix/Command
                {
                    if (cmd.Length < 2) return; // Arg Length Check
                    if (cmd[1] == "help")
                    {
                        this.CMD_Help(msg); // Help command
                        return;
                    }
                    if (cmd.Length < 3) return; // Arg Length Check
                    this.CMD_Armory(msg, cmd); // Armory Command
                }
            }
            catch { return; }
        }
        private async Task CMD_Help(SocketUserMessage msg) // Display usage help to requestor
        {
            try
            {
                Program.Log($"Help Command requested by {msg.Author}");
                msg.Channel.SendMessageAsync($"ArmoryBot Usage: `{this.Config.Prefix}armory CharacterName-Realm pve/pvp`\n**NOTE:** Spaces in realm name should have a dash ' - '\n\nLearn more about ArmoryBot at: http://github.com/imerzan/ArmoryBot/");
            }
            catch { return; }
        }
        private async Task CMD_Armory(SocketUserMessage msg, string[] cmd) // Main Armory Lookup: [0] = command, [1] = user-realm , [2] = pve/pvp
        {
            try
            {
                Program.Log($"Armory Command requested by {msg.Author}");
                if (cmd[2] != "pve" & cmd[2] != "pvp") throw new Exception($"Type must be either pve or pvp. '{cmd[2]}' is invalid."); // Make sure lookup type is valid
                string[] character = cmd[1].Split(new[] { '-' }, 2); // Split CharacterName-Realm. Example: splits Frostchiji-Wyrmrest-Accord into [0]Frostchiji [1]Wyrmrest-Accord (keeps second dash).
                ArmoryData info = await this.BlizzAPI.ArmoryLookup(character[0], character[1], cmd[2]); // Main Blizzard API Lookup
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
                }
                eb.WithFooter($"{this.Config.Prefix}armory help | http://github.com/imerzan/ArmoryBot"); // Display help information in footer
                eb.ThumbnailUrl = info.AvatarUrl; // Set Character Avatar as Thumbnail Picture
                msg.Channel.SendMessageAsync("", false, eb.Build()); // Send message to requestor with Armory Info (embed)
            }
            catch (Exception ex)
            {
                Program.Log($"{msg}: {ex} **Sending generic error notification to {msg.Author}**");
                try { msg.Channel.SendMessageAsync($"**ERROR** looking up `{msg}`\nSee `{this.Config.Prefix}armory help`"); } catch { } // Generic error notification to user
            }
        }
        private async Task Discord_Log(LogMessage msg) // Discord Logging Method
        {
            Program.Log(msg.ToString());
        }
    }

}
