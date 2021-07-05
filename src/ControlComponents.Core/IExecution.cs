using System;

namespace ControlComponents.Core
{
    public interface IExecutionState
    {
        string ComponentName { get; }
        ExecutionState EXST { get; }
        event ExecutionStateEventHandler ExecutionStateChanged;
    }

    public interface IExecution : IExecutionState
    {
        void SetState(ExecutionState newState);
    }
}