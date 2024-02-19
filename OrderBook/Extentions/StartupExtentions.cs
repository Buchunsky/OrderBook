using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using OrderBook.Services;
using OrderBook.Services.Interfaces;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace OrderBook.Extentions;

public static class StartupExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddScoped<IBitstampService, BitstampService>()
            .AddScoped<IWebSocketService, WebSocketService>();

    public static IServiceCollection ConfigureLogger(this IServiceCollection services)
    {
        LogManager.Setup().LoadConfiguration(builder =>
        {
            builder.ForLogger().FilterMinLevel(NLog.LogLevel.Info).WriteToConsole();
        });

        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddNLog();
        });
        return services;
    }
}