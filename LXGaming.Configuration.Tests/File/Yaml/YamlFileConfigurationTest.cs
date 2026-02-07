using LXGaming.Configuration.File.Yaml;
using LXGaming.Configuration.Tests.Models;
using NUnit.Framework;
using YamlDotNet.Core;
using static System.IO.File;

namespace LXGaming.Configuration.Tests.File.Yaml;

public class YamlFileConfigurationTest : FileConfigurationTest {

    protected override string Path => "config.yaml";

    private static readonly YamlFileConfigurationOptions[] Options = [
        new() {
            Atomic = false
        },
        new() {
            Atomic = true
        }
    ];

    [TestCaseSource(nameof(Options))]
    public async Task TestLoadAsync(YamlFileConfigurationOptions options) {
        // Load configuration
        using var configuration = await YamlFileConfiguration<Config>.LoadAsync(options);
        // Test generated data
        Assert.That(configuration.Value?.Data, Is.EqualTo(Data));
    }

    [TestCaseSource(nameof(Options))]
    public async Task TestSaveAsync(YamlFileConfigurationOptions options) {
        // Load configuration
        using var configuration = await YamlFileConfiguration<Config>.LoadAsync(options);
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
    public void TestCancelledLoad(YamlFileConfigurationOptions options) {
        // Attempt to load configuration
        using var configuration = new YamlFileConfiguration<Config>(options);
        var cancellationToken = new CancellationToken(true);
        Assert.ThrowsAsync<TaskCanceledException>(async () => await configuration.LoadAsync(cancellationToken));
        Assert.That(configuration.Value, Is.Null);
    }

    [TestCaseSource(nameof(Options))]
    public async Task TestCancelledSaveAsync(YamlFileConfigurationOptions options) {
        // Load configuration
        using var configuration = await YamlFileConfiguration<Config>.LoadAsync(options);
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

    [TestCaseSource(nameof(Options))]
    public async Task TestNullLoadAsync(YamlFileConfigurationOptions options) {
        // Write empty configuration
        await Create(Path).DisposeAsync();

        // Attempt to load configuration
        Assert.ThrowsAsync<YamlException>(async () => {
            using var configuration = await YamlFileConfiguration<Config>.LoadAsync(options);
        });

        // Read configuration contents
        var contents = await ReadAllTextAsync(Path, CancellationToken.None);
        // Test configuration contents
        Assert.That(contents, Is.Empty);
    }

    [TestCaseSource(nameof(Options))]
    public async Task TestNullSaveAsync(YamlFileConfigurationOptions options) {
        // Attempt to save configuration
        using var configuration = new YamlFileConfiguration<Config>(options);
        Assert.ThrowsAsync<InvalidOperationException>(async () => await configuration.SaveAsync());

        // Read configuration contents
        var contents = await ReadAllTextAsync(Path, CancellationToken.None);
        // Test configuration contents
        Assert.That(contents, Is.EqualTo(CreateContents(Data)));
    }

    protected override string CreateContents(string data) {
        return $"data: {data}{Environment.NewLine}";
    }
}