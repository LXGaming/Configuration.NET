using LXGaming.Configuration.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LXGaming.Configuration.Hosting;

public static class Extensions {

    public static IHostBuilder UseConfiguration(this IHostBuilder builder, IConfiguration configuration) {
        return builder.ConfigureServices((_, services) => {
            services.AddConfiguration(configuration);
        });
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration) {
        if (!services.Contains(ConfigurationService.ServiceDescriptor)) {
            services.Add(ConfigurationService.ServiceDescriptor);
        }

        foreach (var type in configuration.GetType().GetInterfaces()) {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IConfiguration<>)) {
                // The implementation factory forces the IServiceProvider to resolve the service.
                services.AddSingleton(type, _ => configuration);
            }

            if (type == typeof(IConfiguration)) {
                // The implementation factory forces the IServiceProvider to resolve the service.
                services.AddSingleton(type, _ => configuration);
            }
        }

        return services;
    }
}