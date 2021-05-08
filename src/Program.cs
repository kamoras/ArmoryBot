using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArmoryBot
{
    public class Program
    {
        public static void Main(string[] args) /* Program Entry Point */
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseSystemd()
            .ConfigureServices((hostContext, services) =>
            {
                ArmoryBotConfig config = hostContext.Configuration.GetSection("ArmoryBotConfig").Get<ArmoryBotConfig>(); // Read Config from asppsettings.json
                services.AddSingleton(config);
                services.AddHostedService<Worker>();
            });
    }
}
