namespace RdpIo.Core.StateManagement;

/// <summary>
/// Event args for state changed event
/// </summary>
public class StateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Previous state
    /// </summary>
    public ApplicationState PreviousState { get; set; }

    /// <summary>
    /// Current state
    /// </summary>
    public ApplicationState CurrentState { get; set; }

    /// <summary>
    /// Transition time
    /// </summary>
    public DateTime TransitionTime { get; set; }

    /// <summary>
    /// Transition context (optional)
    /// </summary>
    public object? Context { get; set; }
}

/// <summary>
/// Event args for state transition requested
/// </summary>
public class StateTransitionRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Requested state
    /// </summary>
    public ApplicationState RequestedState { get; set; }

    /// <summary>
    /// Reason for request
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Can this transition be cancelled
    /// </summary>
    public bool CanCancel { get; set; }

    /// <summary>
    /// Cancellation flag (set by subscribers)
    /// </summary>
    public bool Cancel { get; set; }
}

