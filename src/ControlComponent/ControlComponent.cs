using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ControlComponent
{
    public class ControlComponent
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Execution execution;
        private Occupation occupation;
        private IOperationMode operationMode;
        private IDictionary<string, IOperationMode> operationModes;
        // TODO chnage string keys Enums
        private IDictionary<string, OrderOutput> orderOutputs;

        public event ExecutionStateEventHandler ExecutionStateChanged;

        public string OpModeName => operationMode != null ? operationMode.OpModeName : "NONE";
        public ICollection<string> OpModes => operationModes.Keys;
        public ICollection<string> Roles => orderOutputs.Keys;
        public string ComponentName { get; }
        Task runningOpMode;

        public ExecutionState EXST => execution.EXST;

        public ControlComponent(string name, ICollection<IOperationMode> opModes, ICollection<OrderOutput> orderOutputs, ICollection<string> neededRoles)
        {
            var missingRoles = neededRoles.Except(orderOutputs.Select(o => o.Role));
            if(missingRoles.Any())
            {
                throw new ArgumentException($"Missing roles {string.Join(" ", missingRoles)} for {name}");
            }

            ComponentName = name;
            operationModes = opModes.ToDictionary(o => o.OpModeName);
            this.orderOutputs = orderOutputs.ToDictionary(o => o.Role);
            execution = new Execution(ComponentName);
            occupation = new Occupation();

            execution.ExecutionStateChanged += (object sender, ExecutionStateEventArgs e) => ExecutionStateChanged?.Invoke(sender, e);
        }

        public string OCCUPIER => occupation.OCCUPIER;
        public void Occupy(string sender) => occupation.Occupy(sender);
        public void Free(string sender) => occupation.Free(sender);
        public bool IsOccupied() => occupation.IsOccupied();
        public bool IsFree() => occupation.IsFree();

        private void ChangeStateOccupied(ExecutionState newState, string sender)
        {
            if (IsFree())
            {
                Occupy(sender);
                execution.SetState(newState);
            }
            else if (OCCUPIER == sender)
            {
                execution.SetState(newState);
            }
            else
            {
                throw new InvalidOperationException($"{sender} cannot change to {newState}, while {OCCUPIER} occupies cc.");
            }
        }

        private void ChangeState(ExecutionState newState, string sender)
        {
            if (operationMode != null)
            {
                ChangeStateOccupied(newState, sender);
            }
            else
            {
                throw new InvalidOperationException($"{ComponentName} cannot change to {newState}, if no operation mode is selected");
            }
        }

        public void Reset(string sender)
        {
            ChangeState(ExecutionState.RESETTING, sender);
        }

        public void Start(string sender)
        {
            ChangeState(ExecutionState.STARTING, sender);
        }

        public void Suspend(string sender)
        {
            ChangeState(ExecutionState.SUSPENDING, sender);
        }

        public void Unsuspend(string sender)
        {
            ChangeState(ExecutionState.UNSUSPENDING, sender);
        }
        public void Stop(string sender)
        {
            ChangeState(ExecutionState.STOPPING, sender);
        }
        public void Hold(string sender)
        {
            ChangeState(ExecutionState.HOLDING, sender);
        }
        public void Unhold(string sender)
        {
            ChangeState(ExecutionState.UNHOLDING, sender);
        }
        public void Abort(string sender)
        {
            ChangeState(ExecutionState.ABORTING, sender);
        }
        public void Clear(string sender)
        {
            ChangeState(ExecutionState.CLEARING, sender);
        }

        public async Task SelectOperationMode(string operationMode)
        {
            try
            { 
                if (EXST == ExecutionState.STOPPED)
                {
                    this.operationMode = operationModes[operationMode];
                    runningOpMode = this.operationMode.Select(this.execution, orderOutputs);
                    await runningOpMode;
                }
            }
            catch (System.Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        public async Task DeselectOperationMode()
        {
            if(operationMode == null)
            {
                throw new InvalidOperationException("No operation mode selected");
            }
            else
            {
                if (EXST == ExecutionState.STOPPED)
                {
                    this.operationMode.Deselect();
                    await runningOpMode;
                    this.operationMode = null;
                }
                else
                {
                    throw new InvalidOperationException($"Operation mode can not be deselected in {EXST} state.");
                }
            }
        }

    }
}
