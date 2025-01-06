using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace LXGaming.Configuration;

public class DefaultConfiguration : IConfiguration {

    protected ConcurrentDictionary<string, IProvider> Providers { get; } = new();

    private bool _disposed;

    public async Task LoadAsync(CancellationToken cancellationToken = default) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var task = Task.WhenAll(Providers.Select(pair => pair.Value.LoadAsync(cancellationToken)));
        try {
            await task.ConfigureAwait(false);
        } catch {
            if (task.Exception == null) {
                throw;
            }

            ExceptionDispatchInfo.Throw(task.Exception);
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var task = Task.WhenAll(Providers.Select(pair => pair.Value.SaveAsync(cancellationToken)));
        try {
            await task.ConfigureAwait(false);
        } catch {
            if (task.Exception == null) {
                throw;
            }

            ExceptionDispatchInfo.Throw(task.Exception);
        }
    }

    public void Register(string key, IProvider provider) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (Providers.TryAdd(key, provider)) {
            return;
        }

        throw new InvalidOperationException($"Key '{key}' is already registered.");
    }

    public void Unregister(string key) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (Providers.Remove(key, out var provider)) {
            provider.Dispose();
        }
    }

    public IProvider? GetProvider(string key) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return Providers.GetValueOrDefault(key);
    }

    public IProvider? GetProvider(Type providerType) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return GetProviders(providerType).FirstOrDefault();
    }

    public IEnumerable<IProvider> GetProviders(Type providerType) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var pair in Providers) {
            if (providerType.IsInstanceOfType(pair.Value)) {
                yield return pair.Value;
            }
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        if (disposing) {
            foreach (var pair in Providers) {
                pair.Value.Dispose();
            }
        }

        _disposed = true;
    }
}