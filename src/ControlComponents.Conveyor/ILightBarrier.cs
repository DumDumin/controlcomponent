using System;

namespace ControlComponents.Conveyor
{
    public interface ILightBarrier
    {
        string Id { get; }
        event EventHandler Hit;
        bool Occupied { get; }
    }
}