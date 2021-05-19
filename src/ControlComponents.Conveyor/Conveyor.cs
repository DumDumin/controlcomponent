using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ControlComponents;
using ControlComponents.Core;

namespace ControlComponents.Conveyor
{
    public class Conveyor : ControlComponent
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public IMotor Motor { get; }
        public ILightBarrier LeftStop { get; }
        public ILightBarrier LeftSlow { get; }
        public ILightBarrier RightSlow { get; }
        public ILightBarrier RightStop { get; }

        public Conveyor(string name, ICollection<IOperationMode> additionalOpModes, IMotor motor, ILightBarrier leftStop, ILightBarrier leftSlow, ILightBarrier rightSlow, ILightBarrier rightStop)
            : base(
                name, 
                additionalOpModes.Concat(createOperationModes(motor, leftStop, leftSlow, rightSlow, rightStop)).ToList(),
                new Collection<OrderOutput>(),
                new Collection<string>()
            )
        {
            Motor = motor;
            LeftStop = leftStop;
            LeftSlow = leftSlow;
            RightSlow = rightSlow;
            RightStop = rightStop;
        }

        public Conveyor(string name, IMotor motor, ILightBarrier leftStop, ILightBarrier leftSlow, ILightBarrier rightSlow, ILightBarrier rightStop)
            : this(name, new Collection<IOperationMode>(), motor, leftStop, leftSlow, rightSlow, rightStop)
        {}

        private static ICollection<IOperationMode> createOperationModes(IMotor motor, ILightBarrier leftStop, ILightBarrier leftSlow, ILightBarrier rightSlow, ILightBarrier rightStop)
        {
            return new Collection<IOperationMode>()
                {
                    new Transport("FPASS", motor, leftStop, leftSlow, rightSlow, rightStop, 1, false),
                    new Transport("FTAKE", motor, leftStop, leftSlow, rightSlow, rightStop, 1, true),
                    new Transport("BPASS", motor, leftStop, leftSlow, rightSlow, rightStop, -1, false),
                    new Transport("BTAKE", motor, leftStop, leftSlow, rightSlow, rightStop, -1, true),
                };
        }
    }
}