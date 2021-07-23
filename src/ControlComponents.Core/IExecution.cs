using System;

namespace ControlComponents.Core
{
    public interface IExecutionState
    {
        string ComponentName { get; }
        ExecutionState EXST { get; }
        ExecutionMode EXMODE { get; }
        event ExecutionStateEventHandler ExecutionStateChanged;
        event ExecutionModeEventHandler ExecutionModeChanged;
    }

    public interface IExecution : IExecutionState
    {
        void SetState(ExecutionState newState);
        void SetMode(ExecutionMode mode);
    }
}