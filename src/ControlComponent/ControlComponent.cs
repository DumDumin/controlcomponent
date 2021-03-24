using System;

namespace ControlComponent
{
    public class ControlComponent
    {
        private Execution execution;
        private OperationMode operationMode;

        public ExecutionState EXST => execution.EXST;

        public ControlComponent()
        {
            execution = new Execution();
        }

        public void Reset()
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
