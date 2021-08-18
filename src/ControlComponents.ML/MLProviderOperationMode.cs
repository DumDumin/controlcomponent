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

        protected abstract Task<float[][]> Decide();
        protected abstract Task EndEpisode();

        public MLProviderOperationMode(IMLControlComponent cc) : base("Inference")
        {
            this.cc = cc;
        }

        // TODO where to load the model? 
        // protected override async Task Starting(CancellationToken token)
        // {
        //     cc.MLSC = ExecutionState.EXECUTE;
        //     await base.Starting(token);
        // }

        protected override async Task Execute(CancellationToken token)
        {
            cc.MLDECIDE = await Decide();
            await base.Execute(token);
        }

        protected override async Task Stopping(CancellationToken token)
        {
            await EndEpisode();
            await base.Stopping(token);
        }
    }
}
