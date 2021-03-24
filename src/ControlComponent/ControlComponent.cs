using System;

namespace ControlComponent
{
    public class ControlComponent
    {
        private ExecutionState exst = ExecutionState.STOPPED;
        public ExecutionState EXST => exst;

        public void Reset()
        {
            exst = ExecutionState.IDLE;
        }

        public void Start()
        {
            if(exst == ExecutionState.IDLE)
            {
                exst = ExecutionState.EXECUTE;
            }
        }
    }
}
