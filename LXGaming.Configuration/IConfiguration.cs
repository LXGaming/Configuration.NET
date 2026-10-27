namespace LXGaming.Configuration;

/// <summary>
/// Represents a configuration.
/// </summary>
public interface IConfiguration : IDisposable {

    /// <summary>
    /// Gets a value indicating whether the configuration is read-only.
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Gets the value of the configuration.
    /// </summary>
    object? Value { get; }

    /// <summary>
    /// Asynchronously loads the configuration.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous load operation.
    /// </returns>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> has been cancelled.</exception>
    Task LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously saves the configuration.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// </returns>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <exception cref="InvalidOperationException">The configuration is read-only.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> has been cancelled.</exception>
    Task SaveAsync(CancellationToken cancellationToken = default);
}