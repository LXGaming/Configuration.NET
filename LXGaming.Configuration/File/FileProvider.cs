namespace LXGaming.Configuration.File;

public abstract class FileProvider<T> : IProvider<T> {

    public string DirectoryPath { get; }

    public string FilePath { get; }

    public T? Value { get; protected set; }

    private readonly SemaphoreSlim _semaphore;
    private bool _disposed;

    protected FileProvider(string path) {
        var fullPath = Path.GetFullPath(path);
        var fileName = Path.GetFileName(fullPath);
        if (string.IsNullOrEmpty(fileName)) {
            throw new ArgumentException("Invalid file name");
        }

        var directoryName = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directoryName)) {
            throw new ArgumentException("Invalid directory name");
        }

        DirectoryPath = directoryName;
        FilePath = fullPath;

        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!System.IO.File.Exists(FilePath)) {
            Value ??= Activator.CreateInstance<T>();
            await SaveAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try {
            await DeserializeAsync(cancellationToken).ConfigureAwait(false);
        } finally {
            _semaphore.Release();
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!Directory.Exists(DirectoryPath)) {
            Directory.CreateDirectory(DirectoryPath);
        }

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try {
            await SerializeAsync(cancellationToken).ConfigureAwait(false);
        } finally {
            _semaphore.Release();
        }
    }

    protected abstract Task DeserializeAsync(CancellationToken cancellationToken);

    protected abstract Task SerializeAsync(CancellationToken cancellationToken);

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        if (disposing) {
            _semaphore.Dispose();
        }

        _disposed = true;
    }
}