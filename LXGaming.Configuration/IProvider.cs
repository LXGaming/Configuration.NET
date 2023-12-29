namespace LXGaming.Configuration;

public interface IProvider : IDisposable {

    object? Value { get; }

    Task LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(CancellationToken cancellationToken = default);
}