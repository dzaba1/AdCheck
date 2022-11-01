using Dzaba.AdCheck.ActiveDirectory;
using Dzaba.AdCheck.DataAccess;
using Dzaba.AdCheck.DataAccess.SqlServer;
using Dzaba.AdCheck.Diff;
using Dzaba.AdCheck.Polling;
using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Dzaba.AdCheck.Cmd
{
    internal static class Bootstrapper
    {
        public static ServiceProvider BuildContainer()
        {
            var services = new ServiceCollection();
            services.RegisterAd();
            services.RegisterAdPolling();
            services.RegisterAdDiff();
            services.RegisterAdDatabase(c => c.GetRequiredService<IConfiguration>().GetConnectionString("Default"));
            services.RegisterAdSqlServer();

            services.AddTransient<IConfiguration>(c => GetConfig());

            SetupLogging(services);

            return services.BuildServiceProvider();
        }

        private static void SetupLogging(IServiceCollection services)
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DzabaAdCheckCmd.log");

            var logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .MinimumLevel.Debug()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: Constants.SerilogFormat)
                .WriteTo.File(logPath, rollOnFileSizeLimit: true, fileSizeLimitBytes: 15728640,
                    outputTemplate: Constants.SerilogFormat)
                .CreateLogger();

            services.AddLogging(l => l.AddSerilog(logger, true));
        }

        private static IConfiguration GetConfig()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            return new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build();
        }
    }
}
