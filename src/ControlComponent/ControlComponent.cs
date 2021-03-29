﻿using System;
using System.Threading.Tasks;

namespace ControlComponent
{
    public class ControlComponent
    {
        private Execution execution;
        private Occupation occupation;
        private OperationMode operationMode;
        public string OpModeName => operationMode != null ? operationMode.OpModeName : "NONE";
        Task runningOpMode;

        public ExecutionState EXST => execution.EXST;

        public ControlComponent()
        {
            execution = new Execution();
            occupation = new Occupation();
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
                throw new InvalidOperationException($"Cannot change to {newState}, if no operation mode is selected");
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

        public async Task SelectOperationMode(OperationMode operationMode)
        {
            if (EXST == ExecutionState.STOPPED)
            {
                this.operationMode = operationMode;
                runningOpMode = this.operationMode.Select(this.execution);
                await runningOpMode;
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
