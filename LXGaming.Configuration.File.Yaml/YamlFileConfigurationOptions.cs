using YamlDotNet.Serialization;

namespace LXGaming.Configuration.File.Yaml;

public class YamlFileConfigurationOptions : FileConfigurationOptions {

    public IDeserializer? Deserializer { get; set; }

    public ISerializer? Serializer { get; set; }
}