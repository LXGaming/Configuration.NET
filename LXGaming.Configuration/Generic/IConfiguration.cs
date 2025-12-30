namespace LXGaming.Configuration.Generic;

/// <summary>
/// Represents a generic configuration.
/// </summary>
/// <typeparam name="T">The type of the configuration value.</typeparam>
public interface IConfiguration<out T> : IConfiguration {

    /// <inheritdoc cref="IConfiguration.Value" />
    new T? Value { get; }

    /// <inheritdoc />
    object? IConfiguration.Value => Value;
}