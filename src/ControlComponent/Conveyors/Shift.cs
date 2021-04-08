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

        private async Task WaitForNewPosition(int newPosition, CancellationToken token)
        {
            // The use of linkedTokenSource allows a second cancellation condition to leave this method
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationTokenSource linkedTokens = CancellationTokenSource.CreateLinkedTokenSource(source.Token, token);
            EventHandler ClearToken = (object sender, EventArgs e) =>
            {
                logger.Debug("Position changed");
                linkedTokens.Cancel();
            };

            shiftPosition.PositionChanged += ClearToken;
            logger.Debug($"Wait for new Position {newPosition}");
            // TODO calculate timeout by speed and distance
            await Task.Delay(100, linkedTokens.Token).ContinueWith(task => { });
            shiftPosition.PositionChanged -= ClearToken;

            if(newPosition != shiftPosition.Position)
            {
                throw new Exception();
            }
        }

        private async Task Move(CancellationToken token)
        {
            motor.Direction = direction;
            motor.Speed = 1;
            await WaitForNewPosition(shiftPosition.Position + direction, token);
            // Target reached
            motor.Speed = 0;
        }

        protected override async Task Execute(CancellationToken token)
        {
            try
            {  
                if(shiftPosition.Position <= 0)
                {
                    // we are not allowed to use a direction of -1
                    if(direction == 1)
                    {
                        await Move(token);
                    }
                }
                else if(shiftPosition.Position >= shiftPosition.Positions.Count - 1)
                {
                    if(direction == -1)
                    {
                        await Move(token);
                    }
                }
                else {
                    // everything between is allowed to use both directions
                    await Move(token);
                }

                if(!token.IsCancellationRequested)
                {
                    // move on as expected only, if task wasnt interrupted
                    await base.Execute(token);
                }
            }
            catch (System.Exception)
            {
                execution.SetState(ExecutionState.ABORTING);
                await Task.CompletedTask;
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