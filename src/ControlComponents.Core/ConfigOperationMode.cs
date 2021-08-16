using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public class ConfigOperationMode : OperationModeBase
    {
        // This component includes the external deployed operation mode
        private readonly IControlComponent _externalCC;
        Task externalOperationMode;

        public ConfigOperationMode(string name, IControlComponent externalCC) : base(name)
        {
            _externalCC = externalCC;
        }

        protected override void Selected()
        {
            // Check if all necessary outputs are available
            if(!_externalCC.Roles.All(role => this.outputs.Keys.Contains(role) && this.outputs[role].IsSet))
            {
                base.execution.SetState(ExecutionState.ABORTING);
            }
            
            // configure all outputs to be outputs at external opmode as well
            foreach (var role in _externalCC.Roles)
            {
                _externalCC.ChangeOutput(role, this.outputs[role].ComponentName);
            }

            // By selecting the ConfigOperationMode, simply the same OperationMode is selected on the external cc
            externalOperationMode = _externalCC.SelectOperationMode(OpModeName);
        }

        protected override async Task Deselected()
        {
            await _externalCC.DeselectOperationMode();
            await externalOperationMode;
        }

        protected override async Task Resetting(CancellationToken token)
        {
            await _externalCC.ResetAndWaitForIdle(base.execution.ComponentName);
            await base.Resetting(token);
        }

        protected override async Task Starting(CancellationToken token)
        {
            await _externalCC.StartAndWaitForExecute(base.execution.ComponentName);
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
            await _externalCC.StopAndWaitForStopped(base.execution.ComponentName);
            await base.Stopping(token);
        }

        private async Task MirrorState(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                // TODO might be too slow if network is involved => subscription
                if(_externalCC.EXST != base.execution.EXST)
                {
                    base.execution.SetState(_externalCC.EXST);
                }
                else
                {
                    await Task.Delay(1);
                }
            }
        }
    }
}