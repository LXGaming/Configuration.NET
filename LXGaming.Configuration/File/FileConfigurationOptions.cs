namespace LXGaming.Configuration.File;

public class FileConfigurationOptions {

    /// <summary>
    /// Gets or sets a value indicating whether file operations should be performed atomically.
    /// </summary>
    public bool IsAtomic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the file is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }
}