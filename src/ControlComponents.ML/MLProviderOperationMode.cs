using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ControlComponents.Core;

namespace ControlComponents.ML
{
    public abstract class MLProviderOperationMode : OperationModeWaitOutputs
    {
        private readonly IMLControlComponent cc;

        public MLProviderOperationMode(string name, IMLControlComponent cc) : base(name)
        {
            this.cc = cc;
        }

        protected override async Task Starting(CancellationToken token)
        {
            cc.MLSC = ExecutionState.EXECUTE;
            await base.Starting(token);
        }

        protected override async Task Execute(CancellationToken token)
        {
            // go to unsuspending if execute is done
            base.execution.SetState(ExecutionState.SUSPENDING);
            await base.Execute(token);
        }

        protected override async Task Suspending(CancellationToken token)
        {
            cc.MLSC = ExecutionState.SUSPENDED;
            await base.Suspending(token);
        }

        protected override async Task Suspended(CancellationToken token)
        {
            while(cc.MLSC == ExecutionState.SUSPENDED)
            {
                // wait until MLSC is set from outside to signal new input
                await Task.Delay(5);
            }
            await base.Suspended(token);
        }

        protected override async Task Unsuspending(CancellationToken token)
        {
            cc.MLSC = ExecutionState.EXECUTE;
            await base.Unsuspending(token);
        }
    }
}
