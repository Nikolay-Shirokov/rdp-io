namespace RdpIo.Core.StateManagement;

/// <summary>
/// State transition validation rules
/// </summary>
public static class StateTransitionRules
{
    /// <summary>
    /// Transition matrix
    /// </summary>
    private static readonly Dictionary<ApplicationState, HashSet<ApplicationState>> TransitionMatrix = new()
    {
        [ApplicationState.Idle] = new HashSet<ApplicationState>
        {
            ApplicationState.ValidatingClipboard,
            ApplicationState.Settings,
            ApplicationState.SelectingRegion
        },

        [ApplicationState.ValidatingClipboard] = new HashSet<ApplicationState>
        {
            ApplicationState.Countdown,
            ApplicationState.Idle
        },

        [ApplicationState.Countdown] = new HashSet<ApplicationState>
        {
            ApplicationState.Transmitting,
            ApplicationState.Idle
        },

        [ApplicationState.Transmitting] = new HashSet<ApplicationState>
        {
            ApplicationState.Paused,
            ApplicationState.Completed,
            ApplicationState.Failed,
            ApplicationState.Cancelled
        },

        [ApplicationState.Paused] = new HashSet<ApplicationState>
        {
            ApplicationState.Transmitting,
            ApplicationState.Cancelled
        },

        [ApplicationState.Completed] = new HashSet<ApplicationState>
        {
            ApplicationState.Idle
        },

        [ApplicationState.Failed] = new HashSet<ApplicationState>
        {
            ApplicationState.Idle
        },

        [ApplicationState.Cancelled] = new HashSet<ApplicationState>
        {
            ApplicationState.Idle
        },

        [ApplicationState.Settings] = new HashSet<ApplicationState>
        {
            ApplicationState.Idle
        },

        // ===== OCR States =====

        [ApplicationState.SelectingRegion] = new HashSet<ApplicationState>
        {
            ApplicationState.CapturingScreen,
            ApplicationState.Idle
        },

        [ApplicationState.CapturingScreen] = new HashSet<ApplicationState>
        {
            ApplicationState.ProcessingOcr,
            ApplicationState.Failed,
            ApplicationState.Idle
        },

        [ApplicationState.ProcessingOcr] = new HashSet<ApplicationState>
        {
            ApplicationState.ShowingOcrResult,
            ApplicationState.Failed,
            ApplicationState.Idle
        },

        [ApplicationState.ShowingOcrResult] = new HashSet<ApplicationState>
        {
            ApplicationState.Countdown,
            ApplicationState.Idle
        }
    };

    /// <summary>
    /// Validates state transition
    /// </summary>
    public static bool IsValid(ApplicationState from, ApplicationState to)
    {
        if (from == to)
            return false;

        if (!TransitionMatrix.TryGetValue(from, out var allowedStates))
            return false;

        return allowedStates.Contains(to);
    }

    /// <summary>
    /// Gets list of allowed transitions
    /// </summary>
    public static IEnumerable<ApplicationState> GetAllowedTransitions(ApplicationState from)
    {
        if (TransitionMatrix.TryGetValue(from, out var allowedStates))
        {
            return allowedStates;
        }

        return Enumerable.Empty<ApplicationState>();
    }

    /// <summary>
    /// Gets human-readable state description
    /// </summary>
    public static string GetStateDescription(ApplicationState state)
    {
        return state switch
        {
            ApplicationState.Idle => "Готов к работе",
            ApplicationState.ValidatingClipboard => "Проверка буфера обмена",
            ApplicationState.Countdown => "Обратный отсчет",
            ApplicationState.Transmitting => "Передача текста",
            ApplicationState.Paused => "Приостановлено",
            ApplicationState.Completed => "Завершено успешно",
            ApplicationState.Failed => "Завершено с ошибкой",
            ApplicationState.Cancelled => "Отменено",
            ApplicationState.Settings => "Настройки",
            ApplicationState.SelectingRegion => "Выбор области экрана",
            ApplicationState.CapturingScreen => "Захват экрана",
            ApplicationState.ProcessingOcr => "Распознавание текста",
            ApplicationState.ShowingOcrResult => "Результат OCR",
            _ => "Неизвестное состояние"
        };
    }
}

