namespace LXGaming.Configuration;

public interface IConfiguration : IDisposable {

    Task LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(CancellationToken cancellationToken = default);

    void Register(string key, IProvider provider);

    void Unregister(string key);

    IProvider? GetProvider(string key);

    IProvider? GetProvider(Type providerType);

    IEnumerable<IProvider> GetProviders(Type providerType);
}