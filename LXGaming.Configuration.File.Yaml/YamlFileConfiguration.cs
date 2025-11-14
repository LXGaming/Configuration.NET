using YamlDotNet.Core;
using YamlDotNet.Serialization;
using static System.IO.File;

namespace LXGaming.Configuration.File.Yaml;

public class YamlFileConfiguration<T>(string path, YamlFileConfigurationOptions options)
    : FileConfiguration<T>(path, options) where T : new() {

    private readonly IDeserializer _deserializer = options.Deserializer ?? new DeserializerBuilder().Build();
    private readonly ISerializer _serializer = options.Serializer ?? new SerializerBuilder().Build();

    public YamlFileConfiguration(YamlFileConfigurationOptions options) : this(GetPath(), options) {
        // no-op
    }

    public static Task<YamlFileConfiguration<T>> LoadAsync(YamlFileConfigurationOptions? options = null,
        CancellationToken cancellationToken = default) {
        return LoadAsync(GetPath(), options, cancellationToken);
    }

    public static async Task<YamlFileConfiguration<T>> LoadAsync(string path,
        YamlFileConfigurationOptions? options = null, CancellationToken cancellationToken = default) {
        var configuration = new YamlFileConfiguration<T>(path, options ?? new YamlFileConfigurationOptions());
        await configuration.LoadAsync(cancellationToken).ConfigureAwait(false);
        return configuration;
    }

    protected override async Task DeserializeAsync(CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        await using var stream = Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
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

        if (Atomic) {
            var tempFilePath = GetTempFilePath();
            await using (var stream = Open(tempFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) {
                await using var streamWriter = new StreamWriter(stream);
                _serializer.Serialize(streamWriter, value, typeof(T));
            }

            MoveOrReplace(tempFilePath);
        } else {
            await using var stream = Open(FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await using var streamWriter = new StreamWriter(stream);
            _serializer.Serialize(streamWriter, value, typeof(T));
        }
    }

    private static string GetPath() {
        return $"{typeof(T).Name.ToLower()}.yaml";
    }
}