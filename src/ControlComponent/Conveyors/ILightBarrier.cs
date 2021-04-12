using System;

namespace ControlComponent
{
    public interface ILightBarrier
    {
        event EventHandler Hit;
        bool Occupied { get; }
    }
}