using System.Text.Json;

namespace LXGaming.Configuration.File.Json;

public static class JsonFileExtensions {

    public static JsonFileProvider<T> CreateJsonFile<T>(
        this IConfiguration configuration,
        JsonSerializerOptions? options = null) where T : class {
        var name = GetJsonFileName<T>();
        return configuration.CreateJsonFile<T>(name, options);
    }

    public static JsonFileProvider<T> CreateJsonFile<T>(
        this IConfiguration configuration,
        string path,
        JsonSerializerOptions? options = null) where T : class {
        var provider = new JsonFileProvider<T>(path, options);
        configuration.Register(provider.FilePath, provider);
        return provider;
    }

    public static Task<JsonFileProvider<T>> LoadJsonFileAsync<T>(
        this IConfiguration configuration,
        JsonSerializerOptions? options = null) where T : class {
        var name = GetJsonFileName<T>();
        return configuration.LoadJsonFileAsync<T>(name, options);
    }

    public static async Task<JsonFileProvider<T>> LoadJsonFileAsync<T>(
        this IConfiguration configuration,
        string path,
        JsonSerializerOptions? options = null) where T : class {
        var provider = configuration.CreateJsonFile<T>(path, options);
        await provider.LoadAsync().ConfigureAwait(false);
        return provider;
    }

    private static string GetJsonFileName<T>() {
        return $"{typeof(T).Name.ToLower()}.json";
    }
}