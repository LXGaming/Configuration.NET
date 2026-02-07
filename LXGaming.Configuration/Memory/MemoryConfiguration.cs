using LXGaming.Configuration.Generic;

namespace LXGaming.Configuration.Memory;

public sealed class MemoryConfiguration<T> : IConfiguration<T> where T : new() {

    /// <inheritdoc />
    public T? Value { get; } = new();

    /// <inheritdoc />
    public Task LoadAsync(CancellationToken cancellationToken = default) {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SaveAsync(CancellationToken cancellationToken = default) {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose() {
        // no-op
    }
}