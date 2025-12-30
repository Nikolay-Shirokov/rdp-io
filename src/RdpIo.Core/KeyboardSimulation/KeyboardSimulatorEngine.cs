using RdpIo.Infrastructure.Logging;
using RdpIo.Infrastructure.Win32;

namespace RdpIo.Core.KeyboardSimulation;

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

        // Нормализация концов строк: заменяем CRLF на LF, чтобы избежать двойных переносов.
        // Это гарантирует, что \r и \n не будут обрабатываться как два отдельных переноса.
        string normalizedText = text.Replace("\r\n", "\n");

        _logger.LogInfo($"Начало передачи: {normalizedText.Length} символов, стратегия: {_strategy.Name}");

        var result = new TransmissionResult
        {
            TotalCharacters = normalizedText.Length,
            StartTime = DateTime.UtcNow
        };

        try
        {
            for (int i = 0; i < normalizedText.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                char currentChar = normalizedText[i];

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
                    TotalCharacters = normalizedText.Length,
                    CurrentCharacter = currentChar,
                    EstimatedTimeRemaining = CalculateEstimatedTime(i + 1, normalizedText.Length)
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
    /// Переключает раскладку клавиатуры через эмуляцию Win+Space
    /// </summary>
    private async Task SwitchLayoutAsync(KeyboardLayout targetLayout, CancellationToken cancellationToken)
    {
        _logger.LogInfo($"Переключение раскладки: {_layoutManager.CurrentLayout} -> {targetLayout}");

        // Эмулируем нажатие Right Win + Space для переключения раскладки
        // Правая Win-клавиша иногда снижает риск открытия "Пуск" в RDP
        var inputs = new INPUT[]
        {
            // Нажатие Right Win
            CreateKeyInput(VirtualKeyCode.RWIN, isKeyUp: false),

            // Нажатие Space
            CreateKeyInput(VirtualKeyCode.SPACE, isKeyUp: false),

            // Отпускание Space
            CreateKeyInput(VirtualKeyCode.SPACE, isKeyUp: true),

            // Отпускание Right Win
            CreateKeyInput(VirtualKeyCode.RWIN, isKeyUp: true)
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
            // 1. Нажатие модификаторов
            var modifiersDown = new List<INPUT>();
            if (mapping.RequiresShift) modifiersDown.Add(CreateKeyInput(VirtualKeyCode.LSHIFT, isKeyUp: false));
            if (mapping.RequiresCtrl) modifiersDown.Add(CreateKeyInput(VirtualKeyCode.LCONTROL, isKeyUp: false));
            if (mapping.RequiresAlt) modifiersDown.Add(CreateKeyInput(VirtualKeyCode.LMENU, isKeyUp: false));

            if (modifiersDown.Count > 0)
            {
                uint result = _win32Api.SendInput((uint)modifiersDown.Count, modifiersDown.ToArray());
                // Небольшая задержка для обработки состояния модификаторов в RDP
                await Task.Delay(20, cancellationToken);
            }

            // 2. Нажатие и отпускание основной клавиши
            var keyInputs = new List<INPUT>
            {
                CreateKeyInput(mapping.VirtualKeyCode, isKeyUp: false),
                CreateKeyInput(mapping.VirtualKeyCode, isKeyUp: true)
            };
            
            uint keyResult = _win32Api.SendInput((uint)keyInputs.Count, keyInputs.ToArray());
            bool success = keyResult == keyInputs.Count;

            // 3. Отпускание модификаторов (в обратном порядке)
            var modifiersUp = new List<INPUT>();
            if (mapping.RequiresAlt) modifiersUp.Add(CreateKeyInput(VirtualKeyCode.LMENU, isKeyUp: true));
            if (mapping.RequiresCtrl) modifiersUp.Add(CreateKeyInput(VirtualKeyCode.LCONTROL, isKeyUp: true));
            if (mapping.RequiresShift) modifiersUp.Add(CreateKeyInput(VirtualKeyCode.LSHIFT, isKeyUp: true));

            if (modifiersUp.Count > 0)
            {
                // Задержка перед отпусканием
                await Task.Delay(20, cancellationToken);
                _win32Api.SendInput((uint)modifiersUp.Count, modifiersUp.ToArray());
            }

            if (!success)
            {
                _logger.LogError($"SendInput для клавиши {mapping.VirtualKeyCode} завершился с ошибкой");
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


    /// <summary>
    /// Создает INPUT структуру для клавиатурного события
    /// </summary>
    private INPUT CreateKeyInput(VirtualKeyCode keyCode, bool isKeyUp)
    {
        // 0 - MAPVK_VK_TO_VSC (Virtual Key to Scan Code)
        uint scanCode = _win32Api.MapVirtualKey((uint)keyCode, 0);

        var flags = isKeyUp ? KeyEventFlags.KEYUP : 0;
        // Добавляем флаг SCANCODE для корректной работы в RDP (особенно для Shift)
        flags |= KeyEventFlags.SCANCODE;
        
        // Для расширенных клавиш (стрелки, Insert, Delete, Home, End, PageUp, PageDown и т.д.)
        if ((ExtendedKeys.Contains(keyCode)))
        {
            flags |= KeyEventFlags.EXTENDEDKEY;
        }

        return new INPUT
        {
            Type = InputType.KEYBOARD,
            Data = new InputUnion
            {
                Keyboard = new KEYBDINPUT
                {
                    Vk = (ushort)keyCode,
                    Scan = (ushort)scanCode,
                    Flags = flags,
                    Time = 0,
                    ExtraInfo = IntPtr.Zero
                }
            }
        };
    }
    
    // Список расширенных клавиш, требующих флага EXTENDEDKEY
    private static readonly HashSet<VirtualKeyCode> ExtendedKeys = new HashSet<VirtualKeyCode>
    {
        VirtualKeyCode.PRIOR, VirtualKeyCode.NEXT, VirtualKeyCode.END, VirtualKeyCode.HOME,
        VirtualKeyCode.LEFT, VirtualKeyCode.UP, VirtualKeyCode.RIGHT, VirtualKeyCode.DOWN,
        VirtualKeyCode.INSERT, VirtualKeyCode.DELETE, VirtualKeyCode.LWIN, VirtualKeyCode.RWIN,
        VirtualKeyCode.APPS, VirtualKeyCode.DIVIDE, VirtualKeyCode.NUMLOCK, VirtualKeyCode.RCONTROL,
        VirtualKeyCode.RMENU
    };

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

