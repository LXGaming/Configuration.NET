namespace LXGaming.Configuration;

public static class Extensions {

    public static T? GetProvider<T>(this IConfiguration configuration, string key) where T : IProvider {
        return (T?) configuration.GetProvider(key);
    }

    public static T GetRequiredProvider<T>(this IConfiguration configuration, string key) where T : IProvider {
        var provider = configuration.GetProvider<T>(key);
        if (provider == null) {
            throw new InvalidOperationException($"No provider for '{key}' has been registered.");
        }

        return provider;
    }

    public static T? GetProvider<T>(this IConfiguration configuration) where T : IProvider {
        return (T?) configuration.GetProvider(typeof(T));
    }

    public static T GetRequiredProvider<T>(this IConfiguration configuration) where T : IProvider {
        var provider = configuration.GetProvider<T>();
        if (provider == null) {
            throw new InvalidOperationException($"No provider for '{typeof(T).FullName}' has been registered.");
        }

        return provider;
    }

    public static IEnumerable<T> GetProviders<T>(this IConfiguration configuration) where T : IProvider {
        return configuration.GetProviders(typeof(T)).Cast<T>();
    }
}