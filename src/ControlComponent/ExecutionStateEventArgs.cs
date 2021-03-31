using System;

namespace ControlComponent
{
    public delegate void ExecutionStateEventHandler(object? sender, ExecutionStateEventArgs e);

    public class ExecutionStateEventArgs : EventArgs
    {
        public ExecutionState ExecutionState { get; }

        public ExecutionStateEventArgs(ExecutionState executionState)
        {
            ExecutionState = executionState;
        }
    }
}