using System;

namespace ControlComponent
{
    public interface ILightBarrier
    {
        string Id { get; }
        event EventHandler Hit;
        bool Occupied { get; }
    }
}