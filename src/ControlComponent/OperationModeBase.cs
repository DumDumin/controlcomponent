using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ControlComponent
{
    public abstract class OperationModeBase : IOperationMode
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        Dictionary<ExecutionState, Func<CancellationToken, Task>> stateActions;
        public string OpModeName { get; }
        public string WORKST { get; protected set; }
        protected IExecution execution;

        CancellationTokenSource executionTokenSource;
        CancellationTokenSource mainTokenSource;

        protected IDictionary<string, OrderOutput> outputs;
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

        private Dictionary<string, Task> runningOutputs = new Dictionary<string, Task>();

        protected void SelectRole(string role, string operationMode)
        {
            runningOutputs.Add(role, outputs[role].SelectOperationMode(operationMode));
        }

        protected abstract void Selected();

        public async Task Select(IExecution execution, IDictionary<string, OrderOutput> orderOutputs)
        {
            try
            { 
                mainTokenSource = new CancellationTokenSource();
                this.execution = execution;
                this.outputs = orderOutputs;

                foreach (var item in neededRoles)
                {
                    if(!orderOutputs.Keys.Contains(item))
                    {
                        throw new Exception($"Role {item} is not available in known outputs = {string.Join(" ", orderOutputs.Keys)}");
                    }
                }

                Selected();

                execution.ExecutionStateChanged += OnExecutionStateChanged;

                while(!mainTokenSource.IsCancellationRequested)
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
                foreach (var output in runningOutputs)
                {
                    await outputs[output.Key].DeselectOperationMode();
                }
            }
        }

        private async Task WaitForSelectedOutputsToDo(Action<OrderOutput> action, IEnumerable<ExecutionState> states, IEnumerable<OrderOutput> selectedOutputs)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task waitForStateChange = Task.Delay(100000, tokenSource.Token).ContinueWith(task => {});

            // TODO do not only wait for actions, but check if outputs are already in the desired state
            ExecutionStateEventHandler subscriber = (object sender, ExecutionStateEventArgs e) => {
                logger.Debug($"Received {e.ExecutionState} while waiting for {states}");
                if(states.Contains(e.ExecutionState))
                {
                    if(selectedOutputs.All(o => states.Contains(o.EXST)))
                    {
                        tokenSource.Cancel();
                    }
                }
            };

            logger.Debug("Subscribe");
            foreach (var output in selectedOutputs)
            {
                // TODO might need the occupier of the cc of this opmode
                output.ExecutionStateChanged += subscriber;
                action(output);
            }

            if(selectedOutputs.All(o => states.Contains(o.EXST)))
            {
                tokenSource.Cancel();
            }

            await waitForStateChange;
            
            foreach (var output in selectedOutputs)
            {
                output.ExecutionStateChanged -= subscriber;
            }
        }

        private async Task WaitForOutputsToDo(Action<OrderOutput> action, IEnumerable<ExecutionState> states)
        {
            var selectedOutputs = outputs.Values.Where(o => o.OpModeName != "NONE");
            if(selectedOutputs.Count() > 0)
            {
                await WaitForSelectedOutputsToDo(action, states, selectedOutputs);
            }

            execution.SetState(states.First());
            await Task.CompletedTask;
        }

        protected virtual async Task Resetting(CancellationToken token)
        {
            await WaitForOutputsToDo((OrderOutput output) => output.Reset(this.OpModeName) , new Collection<ExecutionState>(){ExecutionState.IDLE});
        }

        protected virtual async Task Starting(CancellationToken token)
        {
            await WaitForOutputsToDo((OrderOutput output) => output.Start(this.OpModeName) , new Collection<ExecutionState>(){ExecutionState.EXECUTE});
        }

        protected virtual async Task Stopping(CancellationToken token)
        {
            await WaitForOutputsToDo((OrderOutput output) => output.Stop(this.OpModeName) , new Collection<ExecutionState>(){ExecutionState.STOPPED});
            // if (output.Value.Cc != null && output.Value.Cc.IsOccupied(control.ComponentName))
            //     output.Value.Cc.Stop(control.ComponentName);
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

        protected virtual async Task Aborting(CancellationToken token)
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
        protected virtual async Task Holding(CancellationToken token)
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
        protected virtual async Task Unholding(CancellationToken token)
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

        // Unhold has to set EXST to EXECUTE after completion.
        protected virtual async Task Unsuspending(CancellationToken token)
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
            await WaitForOutputsToDo((OrderOutput output) => {}, new Collection<ExecutionState>(){ExecutionState.COMPLETING, ExecutionState.COMPLETED});
        }

        protected virtual async Task Completing(CancellationToken token)
        {
            await WaitForOutputsToDo((OrderOutput output) => {} , new Collection<ExecutionState>(){ExecutionState.COMPLETED});
        }

        protected virtual async Task Held(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});
        }
        protected virtual async Task Idle(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});;
        }
        protected virtual async Task Completed(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});
        }
        protected virtual async Task Aborted(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});
        }
        protected virtual async Task Stopped(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});
        }
        protected virtual async Task Suspended(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => {});
        }
    }
}