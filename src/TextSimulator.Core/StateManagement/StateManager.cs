using TextSimulator.Infrastructure.Logging;

namespace TextSimulator.Core.StateManagement;

/// <summary>
/// Manages application state and transitions
/// </summary>
public class StateManager
{
    private ApplicationState _currentState;
    private readonly ILogger _logger;
    private readonly object _stateLock = new object();

    // Events for notifying subscribers
    public event EventHandler<StateChangedEventArgs>? StateChanged;
    public event EventHandler<StateTransitionRequestedEventArgs>? StateTransitionRequested;

    public ApplicationState CurrentState
    {
        get
        {
            lock (_stateLock)
            {
                return _currentState;
            }
        }
        private set
        {
            lock (_stateLock)
            {
                _currentState = value;
            }
        }
    }

    public StateManager(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentState = ApplicationState.Idle;

        _logger.LogInfo("StateManager initialized in Idle state");
    }

    /// <summary>
    /// Transition to new state with validation
    /// </summary>
    public bool TransitionTo(ApplicationState newState, object? context = null)
    {
        ApplicationState previousState = CurrentState;

        // Validate transition
        if (!IsTransitionValid(previousState, newState))
        {
            _logger.LogWarning($"Invalid state transition: {previousState} -> {newState}");
            return false;
        }

        // Execute transition
        CurrentState = newState;

        _logger.LogInfo($"State transition: {previousState} -> {newState}");

        // Notify subscribers
        OnStateChanged(new StateChangedEventArgs
        {
            PreviousState = previousState,
            CurrentState = newState,
            TransitionTime = DateTime.UtcNow,
            Context = context
        });

        return true;
    }

    /// <summary>
    /// Checks if transition is possible
    /// </summary>
    public bool CanTransitionTo(ApplicationState targetState)
    {
        return IsTransitionValid(CurrentState, targetState);
    }

    /// <summary>
    /// Reset to Idle state
    /// </summary>
    public void ResetToIdle()
    {
        _logger.LogInfo($"Resetting state from {CurrentState} to Idle");
        TransitionTo(ApplicationState.Idle);
    }

    /// <summary>
    /// Gets valid transitions from current state
    /// </summary>
    public IEnumerable<ApplicationState> GetValidTransitions()
    {
        return GetValidTransitionsFrom(CurrentState);
    }

    // ===== Private Methods =====

    private bool IsTransitionValid(ApplicationState from, ApplicationState to)
    {
        // Cannot transition to same state
        if (from == to)
        {
            return false;
        }

        var validTransitions = GetValidTransitionsFrom(from);
        return validTransitions.Contains(to);
    }

    private IEnumerable<ApplicationState> GetValidTransitionsFrom(ApplicationState state)
    {
        return StateTransitionRules.GetAllowedTransitions(state);
    }

    private void OnStateChanged(StateChangedEventArgs e)
    {
        StateChanged?.Invoke(this, e);
    }
}
