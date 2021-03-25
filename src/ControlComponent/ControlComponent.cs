using System;

namespace ControlComponent
{
    public class ControlComponent
    {
        private Execution execution;
        private Occupation occupation;
        private OperationMode operationMode;

        public ExecutionState EXST => execution.EXST;

        public ControlComponent()
        {
            operationMode = new OperationMode();
            execution = new Execution(operationMode);
            occupation = new Occupation();
        }

        public string OCCUPIER => occupation.OCCUPIER;
        public void Occupy(string sender) => occupation.Occupy(sender);
        public void Free(string sender) => occupation.Free(sender);
        public bool IsOccupied() => occupation.IsOccupied();
        public bool IsFree() => occupation.IsFree();

        public async Task Reset()
        {
            execution.SetState(ExecutionState.IDLE);
        }

        public void Start()
        {

            execution.SetState(ExecutionState.EXECUTE);
        }

        public void Suspend()
        {
            execution.SetState(ExecutionState.SUSPENDED);
        }
    }
}
