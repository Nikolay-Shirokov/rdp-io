using TextSimulator.Infrastructure.Logging;
using TextSimulator.Infrastructure.Win32;

namespace TextSimulator.Core.KeyboardSimulation;

/// <summary>
/// Основной движок симуляции клавиатурного ввода
/// Интегрирует все компоненты для эмуляции набора текста посимвольно
/// </summary>
public class KeyboardSimulatorEngine : IKeyboardSimulator
{
    private readonly LayoutManager _layoutManager;
    private readonly CharacterMapper _characterMapper;
    private readonly IWin32ApiWrapper _win32Api;
    private readonly ILogger _logger;
    private TransmissionStrategy _strategy;

    /// <summary>
    /// Создает новый экземпляр движка симуляции
    /// </summary>
    public KeyboardSimulatorEngine(
        LayoutManager layoutManager,
        CharacterMapper characterMapper,
        IWin32ApiWrapper win32Api,
        ILogger logger,
        TransmissionStrategy defaultStrategy)
    {
        _layoutManager = layoutManager ?? throw new ArgumentNullException(nameof(layoutManager));
        _characterMapper = characterMapper ?? throw new ArgumentNullException(nameof(characterMapper));
        _win32Api = win32Api ?? throw new ArgumentNullException(nameof(win32Api));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _strategy = defaultStrategy ?? throw new ArgumentNullException(nameof(defaultStrategy));
    }

    /// <summary>
    /// Передает текст посимвольно через эмуляцию клавиатурного ввода
    /// </summary>
    public async Task<TransmissionResult> TransmitTextAsync(
        string text,
        IProgress<TransmissionProgress>? progress,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Текст не может быть пустым", nameof(text));
        }

        _logger.LogInfo($"Начало передачи: {text.Length} символов, стратегия: {_strategy.Name}");

        var result = new TransmissionResult
        {
            TotalCharacters = text.Length,
            StartTime = DateTime.UtcNow
        };

        try
        {
            for (int i = 0; i < text.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                char currentChar = text[i];

                // Проверка поддержки символа
                if (!IsCharacterSupported(currentChar))
                {
                    _logger.LogWarning($"Неподдерживаемый символ на позиции {i}: '{currentChar}' (U+{(int)currentChar:X4})");
                    result.UnsupportedCharacters.Add(currentChar);
                    continue;
                }

                // Определение раскладки для символа
                KeyboardLayout requiredLayout = _layoutManager.GetLayoutForCharacter(currentChar);

                // Переключение раскладки если нужно
                if (requiredLayout != KeyboardLayout.Neutral &&
                    _layoutManager.CurrentLayout != requiredLayout)
                {
                    await SwitchLayoutAsync(requiredLayout, cancellationToken);
                }

                // Получение маппинга символа
                KeyMapping mapping = _characterMapper.GetKeyMapping(currentChar, requiredLayout);

                // Эмуляция нажатия клавиши
                bool success = await SendKeyAsync(mapping, cancellationToken);

                if (success)
                {
                    result.TransmittedCharacters++;
                }
                else
                {
                    result.FailedCharacters++;
                    _logger.LogError($"Ошибка передачи символа на позиции {i}: '{currentChar}'");
                }

                // Отчет о прогрессе
                progress?.Report(new TransmissionProgress
                {
                    CurrentPosition = i + 1,
                    TotalCharacters = text.Length,
                    CurrentCharacter = currentChar,
                    EstimatedTimeRemaining = CalculateEstimatedTime(i + 1, text.Length)
                });

                // Задержка между символами согласно стратегии
                await Task.Delay(_strategy.DelayBetweenCharacters, cancellationToken);
            }

            result.EndTime = DateTime.UtcNow;
            result.IsSuccess = result.FailedCharacters == 0 && result.UnsupportedCharacters.Count == 0;

            _logger.LogInfo($"Передача завершена: {result.TransmittedCharacters}/{result.TotalCharacters} символов, " +
                          $"пропущено: {result.UnsupportedCharacters.Count}, ошибок: {result.FailedCharacters}");

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInfo("Передача отменена пользователем");
            result.EndTime = DateTime.UtcNow;
            result.IsCancelled = true;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Передача прервана исключением: {ex.Message}");
            result.EndTime = DateTime.UtcNow;
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            throw new KeyboardSimulationException("Ошибка при передаче текста", ex);
        }
    }

    /// <summary>
    /// Устанавливает стратегию скорости передачи
    /// </summary>
    public void SetTransmissionStrategy(TransmissionStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        _logger.LogInfo($"Стратегия передачи изменена на: {strategy.Name} ({strategy.DelayBetweenCharacters} мс)");
    }

    /// <summary>
    /// Проверяет, поддерживается ли символ
    /// </summary>
    public bool IsCharacterSupported(char character)
    {
        return _characterMapper.IsCharacterSupported(character);
    }

    /// <summary>
    /// Получает список неподдерживаемых символов в тексте
    /// </summary>
    public IEnumerable<char> GetUnsupportedCharacters(string text)
    {
        return text.Where(c => !IsCharacterSupported(c)).Distinct();
    }

    // ===== ПРИВАТНЫЕ МЕТОДЫ =====

    /// <summary>
    /// Переключает раскладку клавиатуры через эмуляцию Alt+Shift
    /// </summary>
    private async Task SwitchLayoutAsync(KeyboardLayout targetLayout, CancellationToken cancellationToken)
    {
        _logger.LogInfo($"Переключение раскладки: {_layoutManager.CurrentLayout} -> {targetLayout}");

        // Эмулируем нажатие Alt+Shift для переключения раскладки
        // Это стандартная комбинация для переключения раскладки в Windows
        var inputs = new INPUT[]
        {
            // Нажатие Alt
            CreateKeyInput(VirtualKeyCode.MENU, isKeyUp: false),

            // Нажатие Shift
            CreateKeyInput(VirtualKeyCode.SHIFT, isKeyUp: false),

            // Отпускание Shift
            CreateKeyInput(VirtualKeyCode.SHIFT, isKeyUp: true),

            // Отпускание Alt
            CreateKeyInput(VirtualKeyCode.MENU, isKeyUp: true)
        };

        // Отправка всех событий
        uint result = _win32Api.SendInput((uint)inputs.Length, inputs);

        if (result != inputs.Length)
        {
            _logger.LogWarning($"Не все события переключения раскладки отправлены: {result}/{inputs.Length}");
        }

        // Обновляем внутреннее состояние
        _layoutManager.CurrentLayout = targetLayout;

        // Задержка после переключения раскладки (даем системе время обработать)
        await Task.Delay(50, cancellationToken);
    }

    /// <summary>
    /// Отправляет нажатие клавиши через Win32 SendInput API
    /// </summary>
    private async Task<bool> SendKeyAsync(KeyMapping mapping, CancellationToken cancellationToken)
    {
        try
        {
            // Подготовка INPUT структуры с модификаторами
            var inputs = PrepareInputStructure(mapping);

            // Отправка через Win32 API
            uint result = _win32Api.SendInput((uint)inputs.Length, inputs);

            // Проверка успешности: SendInput возвращает количество отправленных событий
            bool success = result == inputs.Length;

            if (!success)
            {
                _logger.LogError($"SendInput завершился с ошибкой: ожидалось {inputs.Length}, отправлено {result}");
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Исключение в SendKeyAsync: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Подготавливает массив INPUT структур для отправки клавиши с модификаторами
    /// Порядок: нажатие модификаторов -> нажатие клавиши -> отпускание клавиши -> отпускание модификаторов
    /// </summary>
    private INPUT[] PrepareInputStructure(KeyMapping mapping)
    {
        var inputs = new List<INPUT>();

        // Нажатие модификаторов (Shift, Ctrl, Alt если нужно)
        if (mapping.RequiresShift)
        {
            inputs.Add(CreateKeyInput(VirtualKeyCode.SHIFT, isKeyUp: false));
        }

        if (mapping.RequiresCtrl)
        {
            inputs.Add(CreateKeyInput(VirtualKeyCode.CONTROL, isKeyUp: false));
        }

        if (mapping.RequiresAlt)
        {
            inputs.Add(CreateKeyInput(VirtualKeyCode.MENU, isKeyUp: false));
        }

        // Нажатие основной клавиши
        inputs.Add(CreateKeyInput(mapping.VirtualKeyCode, isKeyUp: false));

        // Отпускание основной клавиши
        inputs.Add(CreateKeyInput(mapping.VirtualKeyCode, isKeyUp: true));

        // Отпускание модификаторов (в обратном порядке)
        if (mapping.RequiresAlt)
        {
            inputs.Add(CreateKeyInput(VirtualKeyCode.MENU, isKeyUp: true));
        }

        if (mapping.RequiresCtrl)
        {
            inputs.Add(CreateKeyInput(VirtualKeyCode.CONTROL, isKeyUp: true));
        }

        if (mapping.RequiresShift)
        {
            inputs.Add(CreateKeyInput(VirtualKeyCode.SHIFT, isKeyUp: true));
        }

        return inputs.ToArray();
    }

    /// <summary>
    /// Создает INPUT структуру для клавиатурного события
    /// </summary>
    private INPUT CreateKeyInput(VirtualKeyCode keyCode, bool isKeyUp)
    {
        return new INPUT
        {
            Type = InputType.KEYBOARD,
            Data = new InputUnion
            {
                Keyboard = new KEYBDINPUT
                {
                    Vk = (ushort)keyCode,
                    Scan = 0,
                    Flags = isKeyUp ? KeyEventFlags.KEYUP : 0,
                    Time = 0,
                    ExtraInfo = IntPtr.Zero
                }
            }
        };
    }

    /// <summary>
    /// Вычисляет оценочное время до завершения передачи
    /// </summary>
    private TimeSpan CalculateEstimatedTime(int currentPosition, int totalCharacters)
    {
        int remaining = totalCharacters - currentPosition;
        int delayMs = _strategy.DelayBetweenCharacters;
        return TimeSpan.FromMilliseconds(remaining * delayMs);
    }
}
