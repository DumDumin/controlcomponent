using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ControlComponents.Core
{
    public class OperationModeWaitOutputs : OperationModeBase
    {
        public OperationModeWaitOutputs(string name) : base(name)
        {

        }
        public OperationModeWaitOutputs(string name, Collection<string> neededRoles) : base(name, neededRoles)
        {

        }

        protected override async Task Resetting(CancellationToken token)
        {
            await WaitForOutputs((IOrderOutput output) => output.ResetAndWaitForIdle(this.execution.ComponentName));
        }

        protected override async Task Stopping(CancellationToken token)
        {
            await WaitForOutputs((IOrderOutput output) => output.StopAndWaitForStopped(this.execution.ComponentName));
            foreach (var output in outputs.Values.Where(o => o.IsSet && o.IsUsableBy(execution.ComponentName)))
            {
                output.Free(this.execution.ComponentName);
            }
        }

        protected override async Task Aborting(CancellationToken token)
        {
            await WaitForOutputs((IOrderOutput output) => output.AbortAndWaitForAborted(this.execution.ComponentName));
        }

        protected override async Task Starting(CancellationToken token)
        {
            await WaitForOutputs((IOrderOutput output) => output.StartAndWaitForExecute(this.execution.ComponentName));
        }
        
        protected override async Task Clearing(CancellationToken token)
        {
            await WaitForOutputs((IOrderOutput output) => output.StopAndWaitForStopped(this.execution.ComponentName));
            foreach (var output in outputs.Values.Where(o => o.IsSet && o.IsUsableBy(execution.ComponentName)))
            {
                output.Free(this.execution.ComponentName);
            }
        }
    }
}