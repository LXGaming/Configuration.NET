using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace LXGaming.Configuration;

public class Configuration : IDisposable {

    protected ConcurrentDictionary<string, IProvider<object>> Providers { get; } = new();

    private bool _disposed;

    public async Task LoadAsync() {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var task = Task.WhenAll(Providers.Select(pair => pair.Value.LoadAsync()));
        try {
            await task.ConfigureAwait(false);
        } catch {
            if (task.Exception == null) {
                throw;
            }

            ExceptionDispatchInfo.Throw(task.Exception);
        }
    }

    public async Task SaveAsync() {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var task = Task.WhenAll(Providers.Select(pair => pair.Value.SaveAsync()));
        try {
            await task.ConfigureAwait(false);
        } catch {
            if (task.Exception == null) {
                throw;
            }

            ExceptionDispatchInfo.Throw(task.Exception);
        }
    }

    public void Register<T>(string key, IProvider<T> provider) where T : class {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (Providers.TryAdd(key, provider)) {
            return;
        }

        throw new InvalidOperationException($"{key} is already registered");
    }

    public void Unregister(string key) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (Providers.Remove(key, out var provider)) {
            provider.Dispose();
        }
    }

    public IProvider<T> GetRequiredProvider<T>(string key) where T : class {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var provider = GetProvider<T>(key);
        if (provider == null) {
            throw new InvalidOperationException($"No service for '{key}' has been registered");
        }

        return provider;
    }

    public IProvider<T>? GetProvider<T>(string key) where T : class {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (Providers.TryGetValue(key, out var value)) {
            return (IProvider<T>) value;
        }

        return null;
    }

    public IProvider<T> GetRequiredProvider<T>() where T : class {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var provider = GetProvider<T>();
        if (provider == null) {
            throw new InvalidOperationException($"No service for '{nameof(T)}' has been registered");
        }

        return provider;
    }

    public IProvider<T>? GetProvider<T>() where T : class {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return GetProviders<T>().FirstOrDefault();
    }

    public IEnumerable<IProvider<T>> GetProviders<T>() where T : class {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var pair in Providers) {
            if (pair.Value is IProvider<T> provider) {
                yield return provider;
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