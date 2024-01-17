using System.Text.Json;

namespace LXGaming.Configuration.File.Json;

public class JsonFileProvider<T>(
    string path,
    JsonSerializerOptions? options = null) : FileProvider<T>(path) {

    protected override async Task DeserializeAsync(CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        await using var stream = System.IO.File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var value = await JsonSerializer.DeserializeAsync<T>(stream, options, CancellationToken.None).ConfigureAwait(false);
        if (value == null) {
            throw new JsonException($"Failed to deserialize {nameof(T)}");
        }

        Value = value;
    }

    protected override Task SerializeAsync(CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        var value = Value;
        if (value == null) {
            throw new InvalidOperationException("Value is unavailable");
        }

        using var stream = System.IO.File.Open(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        // Don't pass the cancellation token as we've just truncated the file.
        return JsonSerializer.SerializeAsync<T>(stream, value, options, CancellationToken.None);
    }
}