using System;
using System.Threading;
using System.Threading.Tasks;
using ControlComponents.Core;

namespace ControlComponents.Conveyor
{
    internal class Transport : OperationModeBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public IMotor motor { get; }
        public ILightBarrier leftStop { get; }
        public ILightBarrier leftSlow { get; }
        public ILightBarrier rightSlow { get; }
        public ILightBarrier rightStop { get; }

        private int direction;
        private bool take;

        public Transport(string name, IMotor motor, ILightBarrier leftStop, ILightBarrier leftSlow, ILightBarrier rightSlow, ILightBarrier rightStop, int direction, bool take) : base(name)
        {
            this.motor = motor;
            this.leftStop = leftStop;
            this.leftSlow = leftSlow;
            this.rightSlow = rightSlow;
            this.rightStop = rightStop;
            this.direction = direction;
            this.take = take;
        }

        protected override void Selected()
        {
        }


        private ILightBarrier GetLightBarrierStop()
        {
            if (direction == 1)
            {
                return rightStop;
            }
            else if(direction == -1)
            {
                return leftStop;
            }
            else
            {
                throw new Exception($"Direction {direction} is not supported for motor");
            }
        }

        private ILightBarrier GetLightBarrierSlow()
        {
            if (direction == 1)
            {
                return rightSlow;
            }
            else if(direction == -1)
            {
                return leftSlow;
            }
            else
            {
                throw new Exception($"Direction {direction} is not supported for motor");
            }
        }

        protected override async Task Execute(CancellationToken token)
        {
            // TODO to support complete hold / unhold functionality we must implement a little state machine here 
            // OR
            // detect state of process at entering this method

            // TODO use WORKST from opmode in cc
            // control.WORKST = ControlComponent.WORKST_READY;

            // Set direction and speed
            motor.Direction = direction;
            motor.Speed = 1;

            // await motor.Start();
            // yield return new WaitUntil(() => motor.Speed == Speed);
            ILightBarrier stop = GetLightBarrierStop();
            ILightBarrier slow = GetLightBarrierSlow();

            // Check light barriers and control motorspeed
            // control.WORKST = "TakeIn";
            await slow.WaitForLightBarrier(token, take);

            if(!token.IsCancellationRequested)
            {
                // 3rd light barrier reached --> Start positioning
                // control.WORKST = "Positioning";
                motor.Speed = 0.5f;
                await stop.WaitForLightBarrier(token, take);

                // control.WORKST = "Stopping";
                motor.Speed = 0;
                // TODO read measured speed value instead
                //yield return new WaitUntil(() => motor.Speed == 0);
                motor.Direction = 0;
                // control.WORKST = "DONE";
                if(!token.IsCancellationRequested)
                {
                    // move on as expected only, if task wasnt interrupted
                    await base.Execute(token);
                }
            }
        }

        protected override async Task Holding(CancellationToken token)
        {
            motor.Speed = 0;
            await base.Holding(token);
        }

         protected override async Task Stopping(CancellationToken token)
        {
            motor.Speed = 0;
            await base.Stopping(token);
        }

        protected override async Task Aborting(CancellationToken token)
        {
            motor.Speed = 0;
            await base.Aborting(token);
        }

        // protected override async Task Completing(CancellationToken token)
        // {
        //     motor.Direction = 0;
        //     motor.Speed = 0;
        //     await base.Completing(token);
        // }
    }
}