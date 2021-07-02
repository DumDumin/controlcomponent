using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public class ControlComponent : IControlComponent
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Execution execution;
        private Occupation occupation;
        private IOperationMode operationMode;
        private IDictionary<string, IOperationMode> operationModes;
        // TODO change string keys Enums
        protected IDictionary<string, IOrderOutput> orderOutputs;

        public event ExecutionStateEventHandler ExecutionStateChanged;

        public string OpModeName => operationMode != null ? operationMode.OpModeName : "NONE";
        public ICollection<string> OpModes => operationModes.Keys;
        public ICollection<string> Roles => orderOutputs.Keys;
        public string ComponentName { get; }
        Task runningOpMode;

        public ExecutionState EXST => execution.EXST;

        public ControlComponent(string name) 
            : this(name, new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>())
        {
        }

        public ControlComponent(string name, ICollection<IOperationMode> opModes, ICollection<IOrderOutput> orderOutputs, ICollection<string> neededRoles)
        {
            ComponentName = name;
            execution = new Execution(ComponentName);
            execution.ExecutionStateChanged += HandleExecutionChanged;
            occupation = new Occupation();

            var missingRoles = neededRoles.Except(orderOutputs.Select(o => o.Role));
            if(missingRoles.Any())
            {
                throw new ArgumentException($"Missing roles {string.Join(" ", missingRoles)} for {name}");
            }

            operationModes = opModes.ToDictionary(o => o.OpModeName);
            this.orderOutputs = orderOutputs.ToDictionary(o => o.Role);
        }

        public void AddOrderOutput(IOrderOutput newOrderOutput)
        {
            orderOutputs.Add(newOrderOutput.Role, newOrderOutput);
        }

        public void AddOperationMode(IOperationMode newOpMode)
        {
            operationModes.Add(newOpMode.OpModeName, newOpMode);
        }

        ~ControlComponent()
        {
            execution.ExecutionStateChanged -= HandleExecutionChanged;
        }

        private void HandleExecutionChanged(object sender, ExecutionStateEventArgs e) => ExecutionStateChanged?.Invoke(this, e);

        public string OCCUPIER => occupation.OCCUPIER;
        public void Occupy(string sender) => occupation.Occupy(sender);
        public void Prio(string sender) => occupation.Prio(sender);
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

        // TODO add Async to the name of this method and add a new method, which does not return a task (same for deselect)
        public async Task SelectOperationMode(string operationMode)
        {
            // TODO it is also possible to Deselect here and then Select new opmode?
            if(this.operationMode != null)
            {
                throw new InvalidOperationException("There is already an operation mode selected");
            }
            else
            {
                if (EXST == ExecutionState.STOPPED)
                {
                    this.operationMode = operationModes[operationMode];
                    runningOpMode = this.operationMode.Select(this.execution, orderOutputs);
                    await runningOpMode;
                }
                else
                {
                    throw new InvalidOperationException($"Operation mode can not be selected in {EXST} state.");
                }
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
                    logger.Debug($"{ComponentName} deselects {this.OpModeName}");
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
