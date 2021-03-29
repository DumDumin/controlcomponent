using System;

namespace ControlComponent
{
    public interface IExecutionState
    {
        ExecutionState EXST { get; }
    }

    public interface IExecution : IExecutionState
    {

        event EventHandler ExecutionStateChanged;

        void SetState(ExecutionState newState);
    }
}