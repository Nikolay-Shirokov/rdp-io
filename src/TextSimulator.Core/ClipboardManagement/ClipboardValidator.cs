using TextSimulator.Infrastructure.Logging;

namespace TextSimulator.Core.ClipboardManagement;

/// <summary>
/// Validator for clipboard content
/// </summary>
public class ClipboardValidator
{
    private readonly ILogger _logger;

    // Limits
    private const int MaxReasonableLength = 100_000; // 100k characters
    private const int WarnLongTextThreshold = 5_000; // Warn for long texts

    public ClipboardValidator(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates clipboard content
    /// </summary>
    public ValidationResult Validate(ClipboardContent content)
    {
        var result = new ValidationResult { IsValid = true };

        if (content == null || string.IsNullOrEmpty(content.Text))
        {
            result.IsValid = false;
            result.ErrorMessage = "Clipboard content is empty";
            return result;
        }

        // Check for text that is too long (possible error)
        if (content.Length > MaxReasonableLength)
        {
            result.IsValid = false;
            result.ErrorMessage = $"Text is too long ({content.Length} characters). Maximum: {MaxReasonableLength}";
            _logger.LogWarning(result.ErrorMessage);
            return result;
        }

        // Warning for long texts
        if (content.Length > WarnLongTextThreshold)
        {
            string warning = $"Text is quite long ({content.Length} characters). Transmission may take a while.";
            result.Warnings.Add(warning);
            _logger.LogInfo(warning);
        }

        // Check for null characters
        if (content.Text.Contains('\0'))
        {
            string warning = "Text contains null characters (\\0) which may cause issues";
            result.Warnings.Add(warning);
            _logger.LogWarning(warning);
        }

        // Count control characters for information
        int controlChars = content.Text.Count(char.IsControl);
        if (controlChars > content.Length * 0.1) // More than 10% control characters
        {
            string warning = $"Text contains {controlChars} control characters which may not transmit correctly";
            result.Warnings.Add(warning);
            _logger.LogWarning(warning);
        }

        return result;
    }
}
