using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace LXGaming.Configuration.File.Yaml;

public class YamlFileConfiguration<T>(string path, IDeserializer? deserializer = null, ISerializer? serializer = null)
    : FileConfiguration<T>(path) where T : new() {

    private readonly IDeserializer _deserializer = deserializer ?? new DeserializerBuilder().Build();
    private readonly ISerializer _serializer = serializer ?? new SerializerBuilder().Build();

    public YamlFileConfiguration(IDeserializer? deserializer = null, ISerializer? serializer = null)
        : this(GetPath(), deserializer, serializer) {
        // no-op
    }

    public static Task<YamlFileConfiguration<T>> LoadAsync(IDeserializer? deserializer = null,
        ISerializer? serializer = null, CancellationToken cancellationToken = default) {
        return LoadAsync(GetPath(), deserializer, serializer, cancellationToken);
    }

    public static async Task<YamlFileConfiguration<T>> LoadAsync(string path, IDeserializer? deserializer = null,
        ISerializer? serializer = null, CancellationToken cancellationToken = default) {
        var configuration = new YamlFileConfiguration<T>(path, deserializer, serializer);
        await configuration.LoadAsync(cancellationToken).ConfigureAwait(false);
        return configuration;
    }

    protected override async Task DeserializeAsync(CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        await using var stream = System.IO.File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var streamReader = new StreamReader(stream);
        var value = _deserializer.Deserialize<T>(streamReader);
        if (value == null) {
            throw new YamlException($"Failed to deserialize {typeof(T).FullName}.");
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
        await using var streamWriter = new StreamWriter(stream);
        _serializer.Serialize(streamWriter, value, typeof(T));
    }

    private static string GetPath() {
        return $"{typeof(T).Name.ToLower()}.yaml";
    }
}