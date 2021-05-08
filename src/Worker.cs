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
            while (!NetworkInterface.GetIsNetworkAvailable()) // Wait for network/internet access
            {
                await Task.Delay(100);
            }
            _logger.LogInformation("Starting up ArmoryBot...");
            _ArmoryBot = new ArmoryBot(_logger, _config); // Initializes ArmoryBot
            await _ArmoryBot.StartupAsync(); // Startup Discord Bot (async)
            await Task.Delay(-1, stoppingToken); // Prevents program from terminating early
        }
    }
}
