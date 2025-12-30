using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LXGaming.Configuration.Hosting;

internal sealed class ConfigurationService : IHostedService {

    public static readonly ServiceDescriptor ServiceDescriptor =
        ServiceDescriptor.Singleton<IHostedService, ConfigurationService>();

    public ConfigurationService(IServiceProvider serviceProvider) {
        // Resolve all services to ensure they are disposed with the IServiceProvider.
        _ = serviceProvider.GetServices<IConfiguration>();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}