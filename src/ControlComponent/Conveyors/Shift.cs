using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponent
{
    internal class Shift : OperationModeBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public IMotor motor { get; }

        private int direction;
        private IShiftPosition shiftPosition;

        public Shift(string name, IMotor motor, IShiftPosition shiftPosition, int direction) : base(name)
        {
            this.motor = motor;
            this.direction = direction;
            this.shiftPosition = shiftPosition;
        }

        protected override void Selected()
        {
        }

        private async Task<bool> WaitForNewPosition(int newPosition, CancellationToken token)
        {
            // The use of linkedTokenSource allows a second cancellation condition to leave this method
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationTokenSource linkedTokens = CancellationTokenSource.CreateLinkedTokenSource(source.Token, token);
            EventHandler ClearToken = (object sender, EventArgs e) =>
            {
                logger.Debug($"Position changed to {shiftPosition.Position}");
                linkedTokens.Cancel();
            };

            shiftPosition.PositionChanged += ClearToken;
            logger.Debug($"Wait for new Position {newPosition}, now = {shiftPosition.Position}");
            // TODO calculate timeout by speed and distance
            await Task.Delay(17500, linkedTokens.Token).ContinueWith(task => { });
            shiftPosition.PositionChanged -= ClearToken;

            return newPosition == shiftPosition.Position;
        }

        private async Task<bool> Move(CancellationToken token)
        {
            motor.Direction = direction;
            motor.Speed = 1;
            bool targetReached = await WaitForNewPosition(shiftPosition.Position + direction, token);
            // Target reached
            motor.Speed = 0;
            return targetReached;
        }


        protected override async Task Execute(CancellationToken token)
        {

            bool moveFailed = false;
            if (shiftPosition.Position <= 0)
            {
                // we are not allowed to use a direction of -1
                if (direction == 1)
                {
                    moveFailed = !await Move(token);
                }
                else
                {
                    logger.Warn($"Direction {direction} not allowed in this position");
                }
            }
            else if (shiftPosition.Position >= shiftPosition.Positions.Count - 1)
            {
                if (direction == -1)
                {
                    moveFailed = !await Move(token);
                }
                else
                {
                    logger.Warn($"Direction {direction} not allowed in this position");
                }
            }
            else
            {
                // everything between is allowed to use both directions
                moveFailed = !await Move(token);
            }

            if (!token.IsCancellationRequested)
            {
                if (moveFailed)
                {
                    logger.Error("No position was not reached in time");
                    execution.SetState(ExecutionState.ABORTING);
                    await Task.CompletedTask;
                }
                else
                {
                    // move on as expected only, if task wasnt interrupted
                    await base.Execute(token);
                }
            }
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
    }
}