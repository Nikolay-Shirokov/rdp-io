using System.Runtime.InteropServices;
using TextSimulator.Infrastructure.Logging;
using TextSimulator.Infrastructure.Win32;

namespace TextSimulator.Core.ClipboardManagement;

/// <summary>
/// Windows implementation of clipboard manager
/// </summary>
public class WindowsClipboardManager : IClipboardManager
{
    private readonly IWin32ApiWrapper _win32Api;
    private readonly ILogger _logger;
    private readonly ClipboardValidator _validator;
    private readonly ClipboardCache _cache;

    // Constants for retry logic
    private const int MaxRetryAttempts = 3;
    private const int RetryDelayMs = 50;

    public WindowsClipboardManager(
        IWin32ApiWrapper win32Api,
        ILogger logger,
        ClipboardValidator validator,
        ClipboardCache cache)
    {
        _win32Api = win32Api ?? throw new ArgumentNullException(nameof(win32Api));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<ClipboardContent?> GetTextAsync()
    {
        _logger.LogInfo("Reading clipboard content");

        // Check cache first
        if (_cache.IsValid())
        {
            _logger.LogInfo("Returning cached clipboard content");
            return _cache.GetCached();
        }

        // Read from clipboard with retry logic
        for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            try
            {
                var content = await ReadClipboardWithRetryAsync(attempt);

                if (content != null)
                {
                    // Validation
                    var validationResult = _validator.Validate(content);

                    if (!validationResult.IsValid)
                    {
                        _logger.LogWarning($"Clipboard validation failed: {validationResult.ErrorMessage}");
                        content.ValidationWarnings = validationResult.Warnings;
                    }

                    // Cache it
                    _cache.Set(content);

                    _logger.LogInfo($"Clipboard read successfully: {content.Text.Length} characters");
                    return content;
                }
            }
            catch (ClipboardAccessException ex)
            {
                _logger.LogWarning($"Clipboard access attempt {attempt} failed: {ex.Message}");

                if (attempt < MaxRetryAttempts)
                {
                    await Task.Delay(RetryDelayMs * attempt);
                }
                else
                {
                    _logger.LogError("Failed to access clipboard after all retry attempts");
                    throw;
                }
            }
        }

        return null;
    }

    public bool HasText()
    {
        try
        {
            return _win32Api.IsClipboardFormatAvailable(ClipboardFormat.CF_UNICODETEXT) ||
                   _win32Api.IsClipboardFormatAvailable(ClipboardFormat.CF_TEXT);
        }
        catch (Exception ex)
        {
            _logger.LogError($"HasText check failed: {ex.Message}");
            return false;
        }
    }

    public string GetClipboardInfo()
    {
        if (!HasText())
        {
            return "Clipboard empty";
        }

        try
        {
            // Try to get from cache for speed
            if (_cache.IsValid())
            {
                var cached = _cache.GetCached();
                return cached != null ? $"Ready: {cached.Text.Length} chars" : "Clipboard has text";
            }

            // If cache unavailable, return general info
            return "Clipboard has text";
        }
        catch (Exception ex)
        {
            _logger.LogError($"GetClipboardInfo failed: {ex.Message}");
            return "Clipboard status unknown";
        }
    }

    public void ClearCache()
    {
        _cache.Clear();
        _logger.LogInfo("Clipboard cache cleared");
    }

    // ===== Private Methods =====

    private async Task<ClipboardContent?> ReadClipboardWithRetryAsync(int attemptNumber)
    {
        IntPtr hWnd = IntPtr.Zero; // NULL for global clipboard access

        try
        {
            // Open clipboard
            if (!_win32Api.OpenClipboard(hWnd))
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new ClipboardAccessException($"Failed to open clipboard. Error code: {errorCode}");
            }

            try
            {
                // Try to get Unicode text (priority)
                IntPtr hData = _win32Api.GetClipboardData(ClipboardFormat.CF_UNICODETEXT);

                if (hData == IntPtr.Zero)
                {
                    // Fallback to ANSI text
                    hData = _win32Api.GetClipboardData(ClipboardFormat.CF_TEXT);

                    if (hData == IntPtr.Zero)
                    {
                        _logger.LogInfo("Clipboard is empty or does not contain text");
                        return null;
                    }
                }

                // Lock memory for reading
                IntPtr pText = _win32Api.GlobalLock(hData);

                if (pText == IntPtr.Zero)
                {
                    throw new ClipboardAccessException("Failed to lock global memory");
                }

                try
                {
                    // Read text
                    string? text = Marshal.PtrToStringUni(pText);

                    if (string.IsNullOrEmpty(text))
                    {
                        return null;
                    }

                    return new ClipboardContent
                    {
                        Text = text,
                        Length = text.Length,
                        ReadTime = DateTime.UtcNow,
                        Format = ClipboardFormat.CF_UNICODETEXT
                    };
                }
                finally
                {
                    _win32Api.GlobalUnlock(hData);
                }
            }
            finally
            {
                _win32Api.CloseClipboard();
            }
        }
        catch (ClipboardAccessException)
        {
            throw; // Re-throw for retry logic
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error reading clipboard: {ex.Message}");
            throw new ClipboardAccessException("Unexpected error reading clipboard", ex);
        }
    }
}
