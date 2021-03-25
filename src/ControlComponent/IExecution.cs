using System;

namespace ControlComponent
{
    public interface IExecution
    {
        ExecutionState EXST { get; }

        event EventHandler ExecutionStateChanged;

        void SetState(ExecutionState newState);
    }
}