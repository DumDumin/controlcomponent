using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ControlComponents.Core;

namespace ControlComponents.ML
{
    public abstract class MLUsingOperationMode : OperationModeWaitOutputs
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected IMLControlComponent _cc;

        private int _steps;
        protected Task _currentExecution = Task.CompletedTask;

        public float RTaskCompleted = 1f;
        public float RTaskFailed = -1f;

        public MLUsingOperationMode(string name, IMLControlComponent cc) : base(name)
        {
            this._cc = cc;
        }

        protected abstract Task AfterSuccessfulExecution();
        protected abstract Task AfterUnsuccessfulExecution(Exception e);
        protected abstract bool TargetReached();
        // public abstract void WhenTargetReached();

        protected abstract Task EndEpisode(float reward);


        protected override async Task Starting(CancellationToken token)
        {
            // Reset internal state
            _currentExecution = Task.CompletedTask;
            _steps = 0;

            await base.Starting(token);
        }


        protected override async Task Execute(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_currentExecution.IsCompleted)
                {
                    // await completed task to catch exceptions
                    try
                    {
                        await _currentExecution;
                        await AfterSuccessfulExecution();
                    }
                    catch (Exception e)
                    {
                        await AfterUnsuccessfulExecution(e);
                    }
                    if (TargetReached())
                    {
                        if (!token.IsCancellationRequested)
                        {
                            base.WORKST = "TargetReached";
                            base.execution.SetState(ExecutionState.COMPLETING);
                            await Task.CompletedTask;
                        }
                    }
                    else if (_steps++ > 100)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            base.WORKST = "TargetFailed, too many steps";
                            base.execution.SetState(ExecutionState.ABORTING);
                            await Task.CompletedTask;
                        }
                    }
                    else if (base.outputs.Values.Any(o => o.IsSet && o.EXST == ExecutionState.ABORTED))
                    {
                        base.WORKST = "Output ABORTED";
                        base.execution.SetState(ExecutionState.ABORTING);
                        await Task.CompletedTask;
                    }
                    else
                    {
                        if (!token.IsCancellationRequested)
                        {
                            base.execution.SetState(ExecutionState.SUSPENDING);
                            await Task.CompletedTask;
                        }
                    }
                    await base.Execute(token);
                }
                else
                {
                    // TODO action timeout from base method is ignored this way
                    await Task.Delay(25);
                }
            }
        }

        protected override async Task Completing(CancellationToken token)
        {
            await EndEpisode(RTaskCompleted);
            await base.Completing(token);
        }

        protected override async Task Stopping(CancellationToken token)
        {
            await EndEpisode(RTaskFailed);
            await base.Stopping(token);
        }

        protected override async Task Aborting(CancellationToken token)
        {
            await EndEpisode(RTaskFailed);
            await base.Aborting(token);
        }
    }
}