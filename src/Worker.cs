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
            async void CheckForInternet()
            {
                _logger.LogWarning("Waiting for network connection...");
                using (var ping = new Ping())
                {
                    string[] urls = new string[] { $"{_config.Region}.battle.net", "discord.com", "google.com" }; // Check several urls for reliability
                    while (true)
                    {
                        foreach (string url in urls)
                        {
                            try
                            {
                                var reply = ping.Send(url);
                                if (reply.Status is IPStatus.Success) return;
                                await Task.Delay(250, stoppingToken);
                            }
                            catch (TaskCanceledException) { throw; } // Cancellation was requested
                            catch { }
                        }
                    }
                }
            }
            _logger.LogInformation("Connected!");
            _logger.LogInformation("Starting up ArmoryBot...");
            _ArmoryBot = new ArmoryBot(_logger, _config); // Initializes ArmoryBot
            await _ArmoryBot.StartupAsync(); // Startup Discord Bot (async)
            await Task.Delay(-1, stoppingToken); // Prevents program from terminating early
        }
    }
}
