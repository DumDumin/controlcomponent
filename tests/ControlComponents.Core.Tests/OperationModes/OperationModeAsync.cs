using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponents.Core.Tests
{
    public class OperationModeAsync : OperationModeRaw
    {
        public OperationModeAsync(string name) : base(name) { }

        private async Task WaitToFinish(CancellationToken token)
        {
            await Task.Delay(10);
            // int counter = 0;
            // while (!token.IsCancellationRequested && counter < 10)
            // {
            //     counter++;
            // }
        }

        private async Task DoTask(Func<CancellationToken, Task> action, CancellationToken token)
        {
            await WaitToFinish(token);
            await action(token);
        }

        protected override Task Resetting(CancellationToken token)
        {
            return DoTask(base.Resetting, token);
        }

        protected override Task Starting(CancellationToken token)
        {
            return DoTask(base.Starting, token);
        }

        protected override Task Stopping(CancellationToken token)
        {
            return DoTask(base.Stopping, token);
        }

        protected override Task Execute(CancellationToken token)
        {
            return DoTask(base.Execute, token);
        }

        protected override Task Completing(CancellationToken token)
        {
            return DoTask(base.Completing, token);
        }

        protected override Task Suspending(CancellationToken token)
        {
            return DoTask(base.Suspending, token);
        }

        protected override Task Unsuspending(CancellationToken token)
        {
            return DoTask(base.Unsuspending, token);
        }

        protected override Task Holding(CancellationToken token)
        {
            return DoTask(base.Holding, token);
        }

        protected override Task Unholding(CancellationToken token)
        {
            return DoTask(base.Unholding, token);
        }

        protected override Task Aborting(CancellationToken token)
        {
            return DoTask(base.Aborting, token);
        }
        protected override Task Clearing(CancellationToken token)
        {
            return DoTask(base.Clearing, token);
        }
    }
}