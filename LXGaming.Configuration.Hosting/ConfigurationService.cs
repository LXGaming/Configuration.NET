using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LXGaming.Configuration.Hosting;

internal sealed class ConfigurationService : IHostedService {

    public ConfigurationService(IServiceProvider serviceProvider) {
        // Ensure that the IConfiguration is disposed with the IServiceProvider.
        _ = serviceProvider.GetRequiredService<IConfiguration>();
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}