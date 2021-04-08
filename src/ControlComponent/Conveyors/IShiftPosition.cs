
using System;
using System.Collections.Generic;

namespace ControlComponent
{
    public interface IShiftPosition
    {
        int Position { get; }
        IList<float> Positions { get; }
        event EventHandler PositionChanged;
    }
}