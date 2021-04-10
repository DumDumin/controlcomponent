
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly:InternalsVisibleTo("ControlComponent.Tests")]
namespace ControlComponent
{
    internal class StateWaiter
    {
        CancellationTokenSource idle = new CancellationTokenSource();
        CancellationTokenSource execute = new CancellationTokenSource();
        CancellationTokenSource aborted = new CancellationTokenSource();
        CancellationTokenSource stopped = new CancellationTokenSource();
        CancellationTokenSource completed = new CancellationTokenSource();

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

        public async Task Idle()
        {
            await Task.Delay(1000, idle.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Execute()
        {
            await Task.Delay(1000, execute.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Completed()
        {
            await Task.Delay(1000, completed.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Stopped()
        {
            await Task.Delay(1000, stopped.Token).ContinueWith(HandleTaskResult);
        }
        public async Task Aborted()
        {
            await Task.Delay(1000, aborted.Token).ContinueWith(HandleTaskResult);
        }
    }
}