using NUnit.Framework;

namespace LXGaming.Configuration.Tests.File;
using static System.IO.File;

public abstract class FileConfigurationTest {

    protected abstract string Path { get; }

    protected string Data { get; private set; }

    [SetUp]
    public async Task SetUpAsync() {
        // Generate data
        Data = CreateData();
        // Write configuration contents
        await WriteAllTextAsync(Path, CreateContents(Data));
    }

    [OneTimeTearDown]
    public Task OneTimeTearDownAsync() {
        Delete(Path);
        return Task.CompletedTask;
    }

    protected abstract string CreateContents(string data);

    protected virtual string CreateData() {
        return Guid.NewGuid().ToString();
    }
}