using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace ArmoryBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ArmoryBotConfig _config;
        private ArmoryBot _ArmoryBot;

        public Worker(ILogger<Worker> logger, ArmoryBotConfig config)
        {
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CheckForInternet(); // Make sure there is an internet connection before starting up
            void CheckForInternet()
            {
                _logger.LogWarning("Waiting for network connection...");
                using (var ping = new Ping())
                {
                    string[] urls = new string[] { $"{_config.Region}.battle.net", "discord.com", "google.com" }; // Check several hosts for reliability
                    while (true)
                    {
                        foreach (string url in urls)
                        {
                            try
                            {
                                stoppingToken.ThrowIfCancellationRequested(); // Check if cancellation is requested
                                var reply = ping.Send(url); // Ping host
                                if (reply.Status is IPStatus.Success) return; // Success, continue startup
                            }
                            catch (OperationCanceledException) { throw; } // Cancellation was requested
                            catch { } // Ignore other exceptions (dns, no response, etc.)
                            finally { Thread.Sleep(250); } // Rate-limit pings
                        }
                    }
                }
            }
            _logger.LogInformation("Starting up ArmoryBot...");
            _ArmoryBot = new ArmoryBot(_logger, _config); // Initializes ArmoryBot & Blizzard API
            await _ArmoryBot.Discord_StartupAsync(); // Startup Discord Bot
            await Task.Delay(-1, stoppingToken); // Prevents program from terminating early (unless cancellation is requested)
        }
    }
}
