using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ControlComponents.Core
{
    // TOBI TODO add abstract methods for all states and call those methods => page 89 Stable Abstractions (Dont override concrete functions) "Clean Architecture"
    public abstract class OperationModeBase : IOperationMode
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        Dictionary<ExecutionState, Func<CancellationToken, Task>> stateActions;
        public string OpModeName { get; }
        public string WORKST { get; protected set; }
        protected IExecution execution;

        CancellationTokenSource executionTokenSource;
        CancellationTokenSource mainTokenSource;

        protected IDictionary<string, IOrderOutput> outputs;
        // protected IReadOnlyDictionary<string, OrderOutput> Outputs => outputs;
        public ReadOnlyCollection<string> neededRoles;

        public OperationModeBase(string name) : this(name, new Collection<string>())
        {
        }

        public OperationModeBase(string name, Collection<string> neededRoles)
        {
            OpModeName = name;
            WORKST = "NONE";
            this.neededRoles = new ReadOnlyCollection<string>(neededRoles);

            stateActions = new Dictionary<ExecutionState, Func<CancellationToken, Task>>()
            {
                { ExecutionState.RESETTING  , Resetting_ },
                { ExecutionState.IDLE       , Idle_ },
                { ExecutionState.STARTING   , Starting_ },
                { ExecutionState.EXECUTE    , Execute_ },
                { ExecutionState.COMPLETING , Completing_ },
                { ExecutionState.COMPLETED  , Completed_ },

                { ExecutionState.HOLDING    , Holding_ },
                { ExecutionState.HELD       , Held_ },
                { ExecutionState.UNHOLDING  , Unholding_ },

                { ExecutionState.SUSPENDING    , Suspending_ },
                { ExecutionState.SUSPENDED     , Suspended_ },
                { ExecutionState.UNSUSPENDING  , Unsuspending_ },

                { ExecutionState.STOPPING       , Stopping_},
                { ExecutionState.STOPPED       , Stopped_},

                { ExecutionState.ABORTED        , Aborted_ },
                { ExecutionState.ABORTING       , Aborting_ },

                { ExecutionState.CLEARING        , Clearing_ },
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

        private Dictionary<string, Task> runningOutputs = new Dictionary<string, Task>();

        protected void SelectRole(string role, string operationMode)
        {
            runningOutputs.Add(role, outputs[role].SelectOperationMode(operationMode));
        }

        protected abstract void Selected();

        public async Task Select(IExecution execution, IDictionary<string, IOrderOutput> orderOutputs)
        {
            try
            {
                mainTokenSource = new CancellationTokenSource();
                this.execution = execution;
                this.outputs = orderOutputs;

                foreach (var item in neededRoles)
                {
                    if (!orderOutputs.Keys.Contains(item))
                    {
                        throw new Exception($"Role {item} is not available in known outputs = {string.Join(" ", orderOutputs.Keys)}");
                    }
                }

                Selected();

                execution.ExecutionStateChanged += OnExecutionStateChanged;

                while (!mainTokenSource.IsCancellationRequested)
                {
                    executionTokenSource = new CancellationTokenSource();
                    logger.Debug($"Invoke {execution.EXST} action");
                    await stateActions[execution.EXST].Invoke(executionTokenSource.Token);
                }
                execution.ExecutionStateChanged -= OnExecutionStateChanged;
            }
            catch (System.Exception e)
            {
                logger.Error(e);
                throw e;
            }
            finally
            {
                // TOBI TODO test that output opmodes are deselected
                foreach (var output in outputs.Values.Where(o => o != null && o.OpModeName != "NONE"))
                {
                    await output.DeselectOperationMode();
                }
            }
        }

        protected async Task WaitForOutputs(Func<IOrderOutput, Task> action)
        {
            try
            {
                // TODO The ML application requires to allow outputs to have no value - can it be done without null values??
                var selectedOutputs = outputs.Values.Where(o => o != null && o.OpModeName != "NONE");
                if (selectedOutputs.Count() > 0)
                {
                    await Task.WhenAll(selectedOutputs.Select(o => action(o)));
                }
                await Task.CompletedTask;
            }
            catch (System.Exception e)
            {
                logger.Error(e, $"Outputs = {string.Join(" ", this.outputs.Keys)}");
                throw e;
            }
        }

        protected virtual Task Resetting(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Starting(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Stopping(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Aborting(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Clearing(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Holding(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Unholding(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Suspending(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Unsuspending(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Execute(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Completing(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Held(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Idle(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Completed(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Aborted(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Stopped(CancellationToken token) { return Task.CompletedTask; }
        protected virtual Task Suspended(CancellationToken token) { return Task.CompletedTask; }


        private async Task Resetting_(CancellationToken token)
        {
            await Resetting(token);
            if (!token.IsCancellationRequested)
            {
                execution.SetState(ExecutionState.IDLE);
            }
        }

        // private async Task Resetting_(CancellationToken token)
        // {
        //     bool wait = await Resetting(token);
        //     if (!token.IsCancellationRequested)
        //     {
        //         if(wait)
        //         {
        //             await WaitForOutputs((OrderOutput output) => output.ResetAndWaitForIdle(this.execution.ComponentName));
        //         }
        //         execution.SetState(ExecutionState.IDLE);
        //     }
        // }

        private async Task Starting_(CancellationToken token)
        {
            await Starting(token);
            if (!token.IsCancellationRequested)
            {
                
                execution.SetState(ExecutionState.EXECUTE);
            }
        }

        private async Task Stopping_(CancellationToken token)
        {
            await Stopping(token);
            if (!token.IsCancellationRequested)
            {
                
                execution.SetState(ExecutionState.STOPPED);
            }
        }

        private async Task Aborting_(CancellationToken token)
        {
            await Aborting(token);
            if (!token.IsCancellationRequested)
            {
                
                execution.SetState(ExecutionState.ABORTED);
            }
        }

        // Clear has to set EXST to STOPPED after completion.
        private async Task Clearing_(CancellationToken token)
        {
            await Clearing(token);
            if (!token.IsCancellationRequested)
            {

                execution.SetState(ExecutionState.STOPPED);
            }
        }
        // Hold has to set EXST to HELD after completion.
        private async Task Holding_(CancellationToken token)
        {
            // TOBI TODO stay in this state till all outputs are done with hold
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Hold(control.ComponentName);
            // }
            await Holding(token);
            if (!token.IsCancellationRequested)
            {
                execution.SetState(ExecutionState.HELD);
                await Task.CompletedTask;
            }
        }
        // Unhold has to set EXST to EXECUTE after completion.
        private async Task Unholding_(CancellationToken token)
        {
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Unhold(control.ComponentName);
            // }
            await Unholding(token);
            if (!token.IsCancellationRequested)
            {
                execution.SetState(ExecutionState.EXECUTE);
                await Task.CompletedTask;
            }
        }

        private async Task Suspending_(CancellationToken token)
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
            await Suspending(token);
            if (!token.IsCancellationRequested)
            {
                execution.SetState(ExecutionState.SUSPENDED);
                await Task.CompletedTask;
            }
        }

        // Unhold has to set EXST to EXECUTE after completion.
        private async Task Unsuspending_(CancellationToken token)
        {
            // foreach (var output in control.OrderOutputs)
            // {
            //     if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //         output.Value.Cc.Unsuspend(control.ComponentName);
            // }
            await Unsuspending(token);
            if (!token.IsCancellationRequested)
            {
                execution.SetState(ExecutionState.EXECUTE);
                await Task.CompletedTask;
            }
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
        private async Task Execute_(CancellationToken token)
        {
            
            await Execute(token);
            if (!token.IsCancellationRequested)
            {
                execution.SetState(ExecutionState.COMPLETING);
                await Task.CompletedTask;
            }
        }

        private async Task Completing_(CancellationToken token)
        {
            await Completing(token);
            if (!token.IsCancellationRequested)
            {
                // await WaitForOutputsToDo((OrderOutput output) => {} , new Collection<ExecutionState>(){ExecutionState.COMPLETED});
                execution.SetState(ExecutionState.COMPLETED);
                await Task.CompletedTask;
            }
        }

        private async Task Held_(CancellationToken token)
        {
            await Held(token);
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => { });
        }
        private async Task Idle_(CancellationToken token)
        {
            await Idle(token);
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => { }); ;
        }
        private async Task Completed_(CancellationToken token)
        {
            await Completed(token);
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => { });
        }
        private async Task Aborted_(CancellationToken token)
        {
            await Aborted(token);
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => { });
        }
        private async Task Stopped_(CancellationToken token)
        {
            await Stopped(token);
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => { });
        }
        private async Task Suspended_(CancellationToken token)
        {
            await Suspended(token);
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => { });
        }
    }
}