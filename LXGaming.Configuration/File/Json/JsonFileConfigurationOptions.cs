using System.Text.Json;

namespace LXGaming.Configuration.File.Json;

public class JsonFileConfigurationOptions : FileConfigurationOptions {

    public JsonSerializerOptions? Options { get; set; }
}