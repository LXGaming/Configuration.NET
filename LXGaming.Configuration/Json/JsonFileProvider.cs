using System.Text.Json;

namespace LXGaming.Configuration.Json;

public class JsonFileProvider<T>(
    string path,
    JsonSerializerOptions? options = null) : FileProvider<T>(path) {

    protected override async Task DeserializeAsync(CancellationToken cancellationToken) {
        await using var stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var value = await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken).ConfigureAwait(false);
        if (value == null) {
            throw new JsonException($"Failed to deserialize {nameof(T)}");
        }

        Value = value;
    }

    protected override Task SerializeAsync(CancellationToken cancellationToken) {
        var value = Value;
        if (value == null) {
            throw new InvalidOperationException("Value is unavailable");
        }

        using var stream = File.Open(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        return JsonSerializer.SerializeAsync<T>(stream, value, options, cancellationToken);
    }
}