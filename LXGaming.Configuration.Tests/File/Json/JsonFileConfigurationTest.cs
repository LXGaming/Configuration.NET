using LXGaming.Configuration.File.Json;
using LXGaming.Configuration.Tests.Models;
using NUnit.Framework;
using static System.IO.File;

namespace LXGaming.Configuration.Tests.File.Json;

public class JsonFileConfigurationTest {

    private const string Path = "config.json";
    private string _data;

    [SetUp]
    public async Task SetUpAsync() {
        // Generate data
        _data = CreateData();
        // Write configuration contents
        await WriteAllTextAsync(Path, CreateContents(_data));
    }

    [OneTimeTearDown]
    public Task OneTimeTearDownAsync() {
        Delete(Path);
        return Task.CompletedTask;
    }

    [Test]
    public async Task TestLoad() {
        // Load configuration
        using var configuration = await JsonFileConfiguration<Config>.LoadAsync();
        // Test generated data
        Assert.That(configuration.Value?.Data, Is.EqualTo(_data));
    }

    [Test]
    public async Task TestSave() {
        // Load configuration
        using var configuration = await JsonFileConfiguration<Config>.LoadAsync();
        // Test generated data
        Assert.That(configuration.Value?.Data, Is.EqualTo(_data));

        // Regenerate data
        configuration.Value.Data = CreateData();
        // Save configuration
        await configuration.SaveAsync();

        // Read configuration contents
        var contents = await ReadAllTextAsync(Path);
        // Test configuration contents
        Assert.That(contents, Is.EqualTo(CreateContents(configuration.Value.Data)));
    }

    [Test]
    public void TestCancelledLoad() {
        // Attempt to load configuration
        using var configuration = new JsonFileConfiguration<Config>(new JsonFileConfigurationOptions());
        var cancellationToken = new CancellationToken(true);
        Assert.ThrowsAsync<TaskCanceledException>(async () => await configuration.LoadAsync(cancellationToken));
        Assert.That(configuration.Value, Is.Null);
    }

    [Test]
    public async Task TestCancelledSave() {
        // Load configuration
        using var configuration = await JsonFileConfiguration<Config>.LoadAsync();
        // Test generated data
        Assert.That(configuration.Value?.Data, Is.EqualTo(_data));

        // Regenerate data
        configuration.Value.Data = CreateData();
        // Attempt to save configuration
        var cancellationToken = new CancellationToken(true);
        Assert.ThrowsAsync<TaskCanceledException>(async () => await configuration.SaveAsync(cancellationToken));

        // Read configuration contents
        var contents = await ReadAllTextAsync(Path, CancellationToken.None);
        // Test configuration contents
        Assert.That(contents, Is.EqualTo(CreateContents(_data)));
    }

    private static string CreateContents(string data) {
        return $"{{\"data\":\"{data}\"}}";
    }

    private static string CreateData() {
        return Guid.NewGuid().ToString();
    }
}