using System.Text.Json;
using static System.IO.File;

namespace LXGaming.Configuration.File.Json;

public class JsonFileConfiguration<T>(string path, JsonFileConfigurationOptions options)
    : FileConfiguration<T>(path) where T : new() {

    private readonly JsonSerializerOptions? _options = options.Options;

    public JsonFileConfiguration(JsonFileConfigurationOptions options) : this(GetPath(), options) {
        // no-op
    }

    public static Task<JsonFileConfiguration<T>> LoadAsync(JsonFileConfigurationOptions? options = null,
        CancellationToken cancellationToken = default) {
        return LoadAsync(GetPath(), options, cancellationToken);
    }

    public static async Task<JsonFileConfiguration<T>> LoadAsync(string path,
        JsonFileConfigurationOptions? options = null, CancellationToken cancellationToken = default) {
        var configuration = new JsonFileConfiguration<T>(path, options ?? new JsonFileConfigurationOptions());
        await configuration.LoadAsync(cancellationToken).ConfigureAwait(false);
        return configuration;
    }

    protected override async Task DeserializeAsync(CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        await using var stream = Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var value = await JsonSerializer.DeserializeAsync<T>(stream, _options, CancellationToken.None)
            .ConfigureAwait(false);
        if (value == null) {
            throw new JsonException($"Failed to deserialize {typeof(T).FullName}.");
        }

        Value = value;
    }

    protected override async Task SerializeAsync(CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        var value = Value;
        if (value == null) {
            throw new InvalidOperationException("Value is unavailable.");
        }

        await using var stream = Open(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        // Don't pass the cancellation token as we've just truncated the file.
        await JsonSerializer.SerializeAsync<T>(stream, value, _options, CancellationToken.None);
    }

    private static string GetPath() {
        return $"{typeof(T).Name.ToLower()}.json";
    }
}