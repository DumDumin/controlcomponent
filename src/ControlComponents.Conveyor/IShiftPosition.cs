
using System;
using System.Collections.Generic;

namespace ControlComponents.Conveyor
{
    public interface IShiftPosition
    {
        int Position { get; }

        // 0 if not moving or not between positions
        // otherwise 1 or -1 to determine between which positions we currently are
        int Direction { get; }
        IList<float> Positions { get; }
        event EventHandler SlowZoneReached;
        event EventHandler PositionChanged;
    }
}