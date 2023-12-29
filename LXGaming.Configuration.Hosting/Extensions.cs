using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LXGaming.Configuration.Hosting;

public static class Extensions {

    public static IHostBuilder UseConfiguration(this IHostBuilder builder, IConfiguration configuration) {
        return builder.ConfigureServices((_, collection) => {
            collection.AddConfiguration(configuration);
        });
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration) {
        return services
            .AddSingleton<IConfiguration>(_ => configuration)
            .AddHostedService<ConfigurationService>();
    }
}