using System;

namespace ControlComponents.Core
{
    public delegate void ExecutionModeEventHandler(object sender, ExecutionModeEventArgs e);

    public class ExecutionModeEventArgs : EventArgs
    {
        public ExecutionMode ExecutionMode { get; }

        public ExecutionModeEventArgs(ExecutionMode executionMode)
        {
            ExecutionMode = executionMode;
        }
    }
}