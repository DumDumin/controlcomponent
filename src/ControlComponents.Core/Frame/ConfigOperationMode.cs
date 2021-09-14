using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public class ConfigOperationMode : OperationModeBase
    {
        // This component includes the external deployed operation mode
        private readonly IControlComponent _externalCC;
        Task externalOperationMode = Task.CompletedTask;

        public ConfigOperationMode(string name, IControlComponent externalCC) : base(name)
        {
            _externalCC = externalCC;
        }

        protected override void Selected()
        {
            if (AllNeededOutputsArePresent())
            {
                // By selecting the ConfigOperationMode, simply the same OperationMode is selected on the external cc
                externalOperationMode = _externalCC.SelectOperationMode(OpModeName);
                _externalCC.ExecutionStateChanged += ExecutionStateChanged;
            }
            else
            {
                base.execution.SetState(ExecutionState.ABORTING);
            }
        }
        
        protected override async Task Deselected()
        {
            _externalCC.ExecutionStateChanged -= ExecutionStateChanged;
            // No need to call DeselectOperationMode, because all outputs are deselected if this opmode is deselected
            await externalOperationMode;
        }

        private bool AllNeededOutputsArePresent()
        {
            return _externalCC.Roles.All(role => this.outputs.Keys.Contains(role));
        }

        private void ExecutionStateChanged(object sender, ExecutionStateEventArgs e)
        {
            if(e.ExecutionState != base.execution.EXST)
                base.execution.SetState(e.ExecutionState);
        }

        protected override async Task Resetting(CancellationToken token)
        {
            if(_externalCC.OpModeName != "NONE" && _externalCC.EXST != ExecutionState.RESETTING)
            {
                _externalCC.Reset(base.execution.ComponentName);
            }
            await MirrorState(token);
            await base.Resetting(token);
        }

        protected override async Task Starting(CancellationToken token)
        {
            if(_externalCC.OpModeName != "NONE" && _externalCC.EXST != ExecutionState.STARTING)
            {
                _externalCC.Start(base.execution.ComponentName);
            }
            await MirrorState(token);
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

        protected override async Task Suspending(CancellationToken token)
        {
            if(_externalCC.OpModeName != "NONE" && _externalCC.EXST != ExecutionState.SUSPENDING)
            {
                _externalCC.Suspend(base.execution.ComponentName);
            }
            await MirrorState(token);
            await base.Suspending(token);
        }

        protected override async Task Unsuspending(CancellationToken token)
        {
            if(_externalCC.OpModeName != "NONE" && _externalCC.EXST != ExecutionState.UNSUSPENDING)
            {
                _externalCC.Unsuspend(base.execution.ComponentName);
            }
            await MirrorState(token);
            await base.Unsuspending(token);
        }

        protected override async Task Stopping(CancellationToken token)
        {
            if(_externalCC.OpModeName != "NONE" && _externalCC.EXST != ExecutionState.STOPPING)
            {
                _externalCC.Stop(base.execution.ComponentName);
            }
            await MirrorState(token);
            await base.Stopping(token);
        }

        protected override async Task Clearing(CancellationToken token)
        {
            if(_externalCC.OpModeName != "NONE" && _externalCC.EXST != ExecutionState.CLEARING)
            {
                _externalCC.Clear(base.execution.ComponentName);
            }
            await MirrorState(token);
            await base.Clearing(token);
        }

        protected override async Task Aborting(CancellationToken token)
        {
            if(_externalCC.OpModeName != "NONE")
            {
                if(!token.IsCancellationRequested && _externalCC.EXST != ExecutionState.ABORTING)
                {
                    _externalCC.Abort(base.execution.ComponentName);
                }
                await MirrorState(token);
            }
            await base.Aborting(token);
        }

        private async Task MirrorState(CancellationToken token)
        {
            await Task.Delay(Timeout.Infinite, token).ContinueWith(task => { });
        }
    }
}