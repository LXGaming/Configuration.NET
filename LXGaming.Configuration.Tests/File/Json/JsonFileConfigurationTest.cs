using LXGaming.Configuration.File.Json;
using LXGaming.Configuration.Tests.Models;
using NUnit.Framework;
using static System.IO.File;

namespace LXGaming.Configuration.Tests.File.Json;

public class JsonFileConfigurationTest : FileConfigurationTest {

    protected override string Path => "config.json";

    private static readonly JsonFileConfigurationOptions[] Options = [
        new() {
            Atomic = false
        },
        new() {
            Atomic = true
        }
    ];

    [TestCaseSource(nameof(Options))]
    public async Task TestLoad(JsonFileConfigurationOptions options) {
        // Load configuration
        using var configuration = await JsonFileConfiguration<Config>.LoadAsync(options);
        // Test generated data
        Assert.That(configuration.Value?.Data, Is.EqualTo(Data));
    }

    [TestCaseSource(nameof(Options))]
    public async Task TestSave(JsonFileConfigurationOptions options) {
        // Load configuration
        using var configuration = await JsonFileConfiguration<Config>.LoadAsync(options);
        // Test generated data
        Assert.That(configuration.Value?.Data, Is.EqualTo(Data));

        // Regenerate data
        configuration.Value.Data = CreateData();
        // Save configuration
        await configuration.SaveAsync();

        // Read configuration contents
        var contents = await ReadAllTextAsync(Path);
        // Test configuration contents
        Assert.That(contents, Is.EqualTo(CreateContents(configuration.Value.Data)));
    }

    [TestCaseSource(nameof(Options))]
    public void TestCancelledLoad(JsonFileConfigurationOptions options) {
        // Attempt to load configuration
        using var configuration = new JsonFileConfiguration<Config>(options);
        var cancellationToken = new CancellationToken(true);
        Assert.ThrowsAsync<TaskCanceledException>(async () => await configuration.LoadAsync(cancellationToken));
        Assert.That(configuration.Value, Is.Null);
    }

    [TestCaseSource(nameof(Options))]
    public async Task TestCancelledSave(JsonFileConfigurationOptions options) {
        // Load configuration
        using var configuration = await JsonFileConfiguration<Config>.LoadAsync(options);
        // Test generated data
        Assert.That(configuration.Value?.Data, Is.EqualTo(Data));

        // Regenerate data
        configuration.Value.Data = CreateData();
        // Attempt to save configuration
        var cancellationToken = new CancellationToken(true);
        Assert.ThrowsAsync<TaskCanceledException>(async () => await configuration.SaveAsync(cancellationToken));

        // Read configuration contents
        var contents = await ReadAllTextAsync(Path, CancellationToken.None);
        // Test configuration contents
        Assert.That(contents, Is.EqualTo(CreateContents(Data)));
    }

    protected override string CreateContents(string data) {
        return $"{{\"data\":\"{data}\"}}";
    }
}