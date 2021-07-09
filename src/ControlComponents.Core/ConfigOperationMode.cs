using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public class ConfigOperationMode : OperationModeBase
    {
        // This component includes the external deployed operation mode
        private readonly IControlComponent cc;
        Task externalOperationMode;

        public ConfigOperationMode(string name, IControlComponent cc) : base(name)
        {
            this.cc = cc;
        }

        protected override void Selected()
        {
            // Check if all necessary outputs are available
            if(!cc.Roles.All(role => this.outputs.Keys.Contains(role) && this.outputs[role].IsSet))
            {
                base.execution.SetState(ExecutionState.ABORTING);
            }
            
            // configure all outputs to be outputs at external opmode as well
            foreach (var role in cc.Roles)
            {
                cc.ChangeOutput(role, this.outputs[role].ComponentName);
            }

            // By selecting the ConfigOperationMode, simply the same OperationMode is selected on the external cc
            externalOperationMode = cc.SelectOperationMode(OpModeName);
        }

        protected override async Task Deselected()
        {
            await cc.DeselectOperationMode();
            await externalOperationMode;
        }

        protected override async Task Resetting(CancellationToken token)
        {
            await cc.ResetAndWaitForIdle(base.execution.ComponentName);
            await base.Resetting(token);
        }

        protected override async Task Starting(CancellationToken token)
        {
            await cc.StartAndWaitForExecute(base.execution.ComponentName);
            await base.Starting(token);
        }

        protected override async Task Execute(CancellationToken token)
        {
            await MirrorState(token);
            await base.Execute(token);
        }

        protected override async Task Completing(CancellationToken token)
        {
            await MirrorState(token);
            await base.Completing(token);
        }

        protected override async Task Stopping(CancellationToken token)
        {
            await cc.StopAndWaitForStopped(base.execution.ComponentName, false);
            await base.Stopping(token);
        }

        private async Task MirrorState(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                // TODO might be too slow if network is involved => subscription
                if(cc.EXST != base.execution.EXST)
                {
                    base.execution.SetState(cc.EXST);
                }
                else
                {
                    await Task.Delay(1);
                }
            }
        }
    }
}