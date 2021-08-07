namespace ControlComponents.Core
{
    public enum ExecutionState
    {
        ABORTED, ABORTING, CLEARING,
        STOPPING, STOPPED, RESETTING,
        IDLE, STARTING,
        HOLDING, HELD, UNHOLDING,
        SUSPENDING, SUSPENDED, UNSUSPENDING,
        EXECUTE, COMPLETING, COMPLETED
    };
}