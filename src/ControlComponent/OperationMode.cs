using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponent
{
    public class OperationMode
    {
        Dictionary<ExecutionState, Func<CancellationToken, Task>> stateActions;
        public string OpModeName { get; }
        IExecution execution;

        CancellationTokenSource executionTokenSource;
        CancellationTokenSource mainTokenSource;

        public OperationMode(string name)
        {
            OpModeName = name;

            stateActions = new Dictionary<ExecutionState, Func<CancellationToken, Task>>()
            {
                { ExecutionState.RESETTING  , Resetting },
                { ExecutionState.IDLE       , Idle },
                { ExecutionState.STARTING   , Starting },
                { ExecutionState.EXECUTE    , Execute },
                { ExecutionState.COMPLETING , Completing },
                { ExecutionState.COMPLETED  , Completed },

                { ExecutionState.HOLDING    , Holding },
                { ExecutionState.HELD       , Held },
                { ExecutionState.UNHOLDING  , Unholding },

                { ExecutionState.SUSPENDING    , Suspending },
                { ExecutionState.SUSPENDED     , Suspended },
                { ExecutionState.UNSUSPENDING  , Unsuspending },

                { ExecutionState.STOPPING       , Stopping},
                { ExecutionState.STOPPED       , Stopped},

                { ExecutionState.ABORTED        , Aborted },
                { ExecutionState.ABORTING       , Aborting },

                { ExecutionState.CLEARING        , Clearing },
            };
        }

        private void OnExecutionStateChanged(object sender, EventArgs e)
        {
            executionTokenSource.Cancel();
        }

        public void Deselect()
        {
            // The order is important to avoid a new execution task to begin before the main loop is canceled
            mainTokenSource.Cancel();
            executionTokenSource.Cancel();
        }

        public async Task Select(IExecution execution)
        {
            mainTokenSource = new CancellationTokenSource();
            this.execution = execution;
            execution.ExecutionStateChanged += OnExecutionStateChanged;

            while(!mainTokenSource.IsCancellationRequested)
            {
                executionTokenSource = new CancellationTokenSource();
                await stateActions[execution.EXST].Invoke(executionTokenSource.Token);
            }
            execution.ExecutionStateChanged -= OnExecutionStateChanged;
        }

        protected virtual async Task Resetting(CancellationToken token)
        {
            execution.SetState(ExecutionState.IDLE);
            await Task.CompletedTask;
        }

        protected virtual async Task Starting(CancellationToken token)
        {
            execution.SetState(ExecutionState.EXECUTE);
            await Task.CompletedTask;
        }

        // Stop has to set EXST to STOPPED after completion.
        public virtual async Task Stopping(CancellationToken token)
        {
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Stop(control.ComponentName);
            // }
            execution.SetState(ExecutionState.STOPPED);
            await Task.CompletedTask;
        }

        // Reset has to set EXST to IDLE after completion.
        // public virtual Task Reset()
        // {
        //     foreach (var output in control.OrderOutputs)
        //     {
        //         if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
        //         // TOBI TODO how to wait for sub control components to reach idle ?? -> Update loop in MonoBehaviour
        //             output.Value.Cc.Reset(control.ComponentName);
        //     }
        //     _WORKST = "BSTATE";
        //     return Task.FromResult(ExecutionState.IDLE);
        // }
        // Abort has to set EXST to ABORTED after completion.

        public virtual async Task Aborting(CancellationToken token)
        {
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Abort(control.ComponentName);
            // }
            // await Task.Run( async () => {
            //     control.OrderOutputs.Values.All((OrderOutput o) => o.Cc.EXST == ExecutionState.ABORTED);
            //     await Task.Delay(25);
            // });

            execution.SetState(ExecutionState.ABORTED);
            await Task.CompletedTask;
        }

        // Clear has to set EXST to STOPPED after completion.
        protected virtual async Task Clearing(CancellationToken token)
        {
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Clear(control.ComponentName);
            // }

            // Task clearing = Clearing(token);

            // // Wait for OrderOutputs to be stopped
            // await Task.Run( async () => {
            //     control.OrderOutputs.Values.All((OrderOutput o) => o.Cc.EXST == ExecutionState.STOPPED);
            //     await Task.Delay(25);
            // });

            // await clearing;

            execution.SetState(ExecutionState.STOPPED);
            await Task.CompletedTask;
        }
        // Hold has to set EXST to HELD after completion.
        public virtual async Task Holding(CancellationToken token)
        {
            // TOBI TODO stay in this state till all outputs are done with hold
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Hold(control.ComponentName);
            // }
            execution.SetState(ExecutionState.HELD);
            await Task.CompletedTask;
        }
        // Unhold has to set EXST to EXECUTE after completion.
        public virtual async Task Unholding(CancellationToken token)
        {
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Unhold(control.ComponentName);
            // }
            execution.SetState(ExecutionState.EXECUTE);
            await Task.CompletedTask;
        }

        protected virtual async Task Suspending(CancellationToken token)
        {
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Suspend(control.ComponentName);
            // }

            // Task suspending = Suspending(token);

            // await Task.Run( async () => {
            //     control.OrderOutputs.Values.All((OrderOutput o) => o.Cc.EXST == ExecutionState.STOPPED);
            //     await Task.Delay(25);
            // });

            // await suspending;

            execution.SetState(ExecutionState.SUSPENDED);
            await Task.CompletedTask;
        }

        public virtual async Task Suspended(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token);
        }
        // Unhold has to set EXST to EXECUTE after completion.
        public virtual async Task Unsuspending(CancellationToken token)
        {
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Unsuspend(control.ComponentName);
            // }
            execution.SetState(ExecutionState.EXECUTE);
            await Task.CompletedTask;
        }

        /*
        * OnExecute is called cyclic, if OnSelected isn't overriden.
        * If execution is finished set EXST to COMPLETING an call OnCompleting():
        * <code>
        * control.EXST = ExecutionState.COMPLETING;
        * OnCompleting();
        * </code>
        * //TODO maybe its better to use this as a coroutine ?
        */
        protected virtual async Task Execute(CancellationToken token)
        {
            execution.SetState(ExecutionState.COMPLETING);
            await Task.CompletedTask;
        }

        public virtual async Task Completing(CancellationToken token)
        {
            execution.SetState(ExecutionState.COMPLETED);
            await Task.CompletedTask;
        }

        public virtual async Task Held(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});
        }
        public virtual async Task Idle(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});;
        }
        public virtual async Task Completed(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});
        }
        public virtual async Task Aborted(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});
        }
        public virtual async Task Stopped(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});
        }
    }
}