namespace LXGaming.Configuration;

public interface IProvider<out T> : IDisposable {

    public T? Value { get; }

    Task LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(CancellationToken cancellationToken = default);
}