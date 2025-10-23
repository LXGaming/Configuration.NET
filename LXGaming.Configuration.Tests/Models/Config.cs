using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace LXGaming.Configuration.Tests.Models;

public class Config {

    [JsonPropertyName("data")]
    [YamlMember(Alias = "data")]
    public string Data { get; set; } = "";
}