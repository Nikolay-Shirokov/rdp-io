namespace RdpIo.Core.ClipboardManagement;

/// <summary>
/// Cache for clipboard content with TTL
/// </summary>
public class ClipboardCache
{
    private ClipboardContent? _cachedContent;
    private DateTime _cacheTime;

    // Cache lifetime
    private readonly TimeSpan _cacheLifetime;

    public ClipboardCache(TimeSpan? cacheLifetime = null)
    {
        // Default cache lifetime: 5 seconds
        _cacheLifetime = cacheLifetime ?? TimeSpan.FromSeconds(5);
    }

    /// <summary>
    /// Stores content in cache
    /// </summary>
    public void Set(ClipboardContent content)
    {
        _cachedContent = content;
        _cacheTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets cached content
    /// </summary>
    public ClipboardContent? GetCached()
    {
        return _cachedContent;
    }

    /// <summary>
    /// Checks if cache is valid
    /// </summary>
    public bool IsValid()
    {
        if (_cachedContent == null)
        {
            return false;
        }

        TimeSpan age = DateTime.UtcNow - _cacheTime;
        return age < _cacheLifetime;
    }

    /// <summary>
    /// Clears cache
    /// </summary>
    public void Clear()
    {
        _cachedContent = null;
        _cacheTime = DateTime.MinValue;
    }
}

