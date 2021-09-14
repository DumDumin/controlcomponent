
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    internal class StateWaiter
    {
        CancellationTokenSource idle = new CancellationTokenSource();
        CancellationTokenSource execute = new CancellationTokenSource();
        CancellationTokenSource aborted = new CancellationTokenSource();
        CancellationTokenSource stopped = new CancellationTokenSource();
        CancellationTokenSource completed = new CancellationTokenSource();
        CancellationTokenSource suspend = new CancellationTokenSource();
        CancellationTokenSource held = new CancellationTokenSource();

        public ExecutionStateEventHandler EventHandler { get; }

        public StateWaiter()
        {
            EventHandler = (object sender, ExecutionStateEventArgs e) =>
            {
                switch (e.ExecutionState)
                {
                    case ExecutionState.IDLE:
                        idle.Cancel(); break;
                    case ExecutionState.EXECUTE:
                        execute.Cancel(); break;
                    case ExecutionState.ABORTED:
                        aborted.Cancel(); break;
                    case ExecutionState.STOPPED:
                        stopped.Cancel(); break;
                    case ExecutionState.COMPLETED:
                        completed.Cancel(); break;
                    case ExecutionState.SUSPENDED:
                        suspend.Cancel(); break;
                    case ExecutionState.HELD:
                        held.Cancel(); break;
                    default:
                        break;
                }
            };
        }

        private void HandleTaskResult(Task task)
        {
            if (!task.IsCanceled)
            {
                throw new Exception("Timeout");
            }
        }

        public async Task Idle(int delay = 1000)
        {
            await Task.Delay(delay, idle.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Execute(int delay = 1000)
        {
            await Task.Delay(delay, execute.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Completed(int delay = 1000)
        {
            await Task.Delay(delay, completed.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Stopped(int delay = 1000)
        {
            await Task.Delay(delay, stopped.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Aborted(int delay = 1000)
        {
            await Task.Delay(delay, aborted.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Suspend(int delay = 1000)
        {
            await Task.Delay(delay, suspend.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Held(int delay = 1000)
        {
            await Task.Delay(delay, held.Token).ContinueWith(HandleTaskResult);
        }
    }
}