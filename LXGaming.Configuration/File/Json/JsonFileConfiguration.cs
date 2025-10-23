using System.Text.Json;

namespace LXGaming.Configuration.File.Json;

public class JsonFileConfiguration<T>(string path, JsonSerializerOptions? options = null)
    : FileConfiguration<T>(path) where T : new() {

    public JsonFileConfiguration(JsonSerializerOptions? options = null) : this(GetPath(), options) {
        // no-op
    }

    public static Task<JsonFileConfiguration<T>> LoadAsync(JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default) {
        return LoadAsync(GetPath(), options, cancellationToken);
    }

    public static async Task<JsonFileConfiguration<T>> LoadAsync(string path, JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default) {
        var configuration = new JsonFileConfiguration<T>(path, options);
        await configuration.LoadAsync(cancellationToken).ConfigureAwait(false);
        return configuration;
    }

    protected override async Task DeserializeAsync(CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        await using var stream = System.IO.File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var value = await JsonSerializer.DeserializeAsync<T>(stream, options, CancellationToken.None)
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

        await using var stream = System.IO.File.Open(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        // Don't pass the cancellation token as we've just truncated the file.
        await JsonSerializer.SerializeAsync<T>(stream, value, options, CancellationToken.None);
    }

    private static string GetPath() {
        return $"{typeof(T).Name.ToLower()}.json";
    }
}