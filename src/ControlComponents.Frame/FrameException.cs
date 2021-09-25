using System;

namespace ControlComponents.Core
{
    public class FrameException : Exception
    {
        public FrameException(string ComponentName) : base($"{ComponentName} is a FrameControlComponent. Access output interface by calling reflection methods.")
        {
        }
    }
}