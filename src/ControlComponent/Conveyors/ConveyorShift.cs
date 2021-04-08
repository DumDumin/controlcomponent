using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponent
{
    public class ConveyorShift : Conveyor
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public int Position => shiftPosition.Position;

        private IShiftPosition shiftPosition;

        public ConveyorShift(string name, IMotor motorShift, IShiftPosition shiftPosition, IMotor motorConveyor, ILightBarrier leftStop, ILightBarrier leftSlow, ILightBarrier rightSlow, ILightBarrier rightStop)
            : base(name, createOperationModes(motorShift, shiftPosition), motorConveyor, leftStop, leftSlow, rightSlow, rightStop)
        {
            this.shiftPosition = shiftPosition;
        }

        private static ICollection<IOperationMode> createOperationModes(IMotor motor, IShiftPosition shiftPosition)
        {
            return new Collection<IOperationMode>()
            {
                new Shift("DSHIFT", motor, shiftPosition, direction:  1),
                new Shift("USHIFT", motor, shiftPosition, direction: -1),
            };
        }
    }
}