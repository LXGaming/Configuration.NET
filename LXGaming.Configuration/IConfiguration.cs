namespace LXGaming.Configuration;

public interface IConfiguration : IDisposable {

    Task LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(CancellationToken cancellationToken = default);

    void Register<T>(string key, IProvider<T> provider) where T : class;

    void Unregister(string key);

    IProvider<T> GetRequiredProvider<T>(string key) where T : class;

    IProvider<T>? GetProvider<T>(string key) where T : class;

    IProvider<T> GetRequiredProvider<T>() where T : class;

    IProvider<T>? GetProvider<T>() where T : class;

    IEnumerable<IProvider<T>> GetProviders<T>() where T : class;
}