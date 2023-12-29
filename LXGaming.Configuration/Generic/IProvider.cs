namespace LXGaming.Configuration.Generic;

public interface IProvider<out T> : IProvider {

    new T? Value { get; }

    object? IProvider.Value => Value;
}