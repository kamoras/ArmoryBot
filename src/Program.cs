﻿using System;
using System.IO;
using System.Threading.Tasks;
#pragma warning disable 4014

namespace ArmoryBot
{
    class Program
    {
        private static ArmoryBot armoryBot;
        static async Task Main() // ** Program Entry Point **
        {
            if (!File.Exists(Globals.DiscordConfigPath) | !File.Exists(Globals.BlizzardConfigPath)) // Verify Config Files Exist
            {
                if (!File.Exists(Globals.DiscordConfigPath)) // check for Discord Config
                {
                    File.WriteAllText(Globals.DiscordConfigPath, "{\n\"token\": \"\",\n\"cmdprefix\": \"!\"\n}");
                    Console.WriteLine($"{Globals.DiscordConfigPath} does not exist. Creating blank config file.");
                }
                if (!File.Exists(Globals.BlizzardConfigPath)) // check for Blizzard Config
                {
                    File.WriteAllText(Globals.BlizzardConfigPath, "{\n\"client_id\": \"\",\n\"client_secret\": \"\",\n\"locale\": \"en_US\"\n}");
                    Console.WriteLine($"{Globals.BlizzardConfigPath} does not exist. Creating blank config file.");
                }
                return;
            }
            Log("Starting up ArmoryBot...");
            armoryBot = new ArmoryBot(); // Initializes ArmoryBot
            await armoryBot.StartupAsync(); // Startup Discord Bot (async)
            await Task.Delay(-1); // Prevents program from terminating early
        }
        public async static Task Log(string entry)
        {
            try { await Task.Run(() => Console.WriteLine($"{DateTime.Now}: {entry}")); } catch { return; }
        }
    }
}