using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ControlComponents.Core;

namespace ControlComponents.ML
{
    // TODO check of OperationMode as super class could be sufficient
    public abstract class MLProviderOperationMode : OperationModeWaitOutputs
    {
        private readonly IMLControlComponent cc;

        protected abstract Task<float[][]> Decide();
        protected abstract Task EndEpisode();

        public MLProviderOperationMode(IMLControlComponent cc) : base("Inference")
        {
            this.cc = cc;
        }

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
