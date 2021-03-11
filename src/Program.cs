using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArmoryBot
{
    class Program
    {
        private static ArmoryBot bot_Main;
        private static HttpClientHandler httpClientHandler;
        internal static HttpClient httpClient;
        static async Task Main() // ** Program Entry Point **
        {
            if (!File.Exists(Globals.DiscordConfigPath)) // check for Discord Config
            {
                File.WriteAllText(Globals.DiscordConfigPath, "{\n\"token\": \"\",\n\"cmdprefix\": \"!\"\n}");
                Console.WriteLine(Globals.DiscordConfigPath + " does not exist. Creating blank config file.");
                return;
            }
            if (!File.Exists(Globals.BlizzardConfigPath)) // check for Blizzard Config
            {
                File.WriteAllText(Globals.BlizzardConfigPath, "{\n\"client_id\": \"\",\n\"client_secret\": \"\",\n\"locale\": \"en_US\"\n}");
                Console.WriteLine(Globals.BlizzardConfigPath + " does not exist. Creating blank config file.");
                return;
            }
            Log("Starting up...");
            httpClientHandler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }; // Perform automatic Decompression
            httpClient = new HttpClient(httpClientHandler); // Create static instance of HttpClient for entire program
            bot_Main = new ArmoryBot(); // Create new instance of ArmoryBot, will simultaneously connect to the Blizzard API and request a token.
            await bot_Main.Startup(); // Startup Discord Bot
            await Task.Delay(-1); // Prevent exit
        }
        internal static void Log(string entry)
        {
            try { Console.WriteLine(DateTime.Now.ToString() + ": " + entry); } catch { }
        }
    }
}