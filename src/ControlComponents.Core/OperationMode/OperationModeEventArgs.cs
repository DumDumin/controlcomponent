using System;

namespace ControlComponents.Core
{
    public delegate void OperationModeEventHandler(object sender, OperationModeEventArgs e);

    public class OperationModeEventArgs : EventArgs
    {
        public string OperationModeName { get; }

        public OperationModeEventArgs(string opmode)
        {
            OperationModeName = opmode;
        }
    }
}