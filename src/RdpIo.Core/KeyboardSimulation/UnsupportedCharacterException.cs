namespace RdpIo.Core.KeyboardSimulation;

/// <summary>
/// Исключение, возникающее при попытке обработать неподдерживаемый символ
/// </summary>
public class UnsupportedCharacterException : Exception
{
    /// <summary>
    /// Неподдерживаемый символ
    /// </summary>
    public char Character { get; }

    public UnsupportedCharacterException(char character)
        : base($"Символ '{character}' (U+{(int)character:X4}) не поддерживается")
    {
        Character = character;
    }

    public UnsupportedCharacterException(string message)
        : base(message)
    {
    }

    public UnsupportedCharacterException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

