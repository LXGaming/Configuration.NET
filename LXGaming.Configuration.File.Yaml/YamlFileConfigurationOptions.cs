using YamlDotNet.Serialization;

namespace LXGaming.Configuration.File.Yaml;

public class YamlFileConfigurationOptions {

    public IDeserializer? Deserializer { get; set; }

    public ISerializer? Serializer { get; set; }
}