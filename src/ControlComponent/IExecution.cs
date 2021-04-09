using System;

namespace ControlComponent
{
    public interface IExecutionState
    {
        ExecutionState EXST { get; }
    }

    public interface IExecution : IExecutionState
    {
        string ComponentName { get; }
        event ExecutionStateEventHandler ExecutionStateChanged;
        void SetState(ExecutionState newState);
    }
}