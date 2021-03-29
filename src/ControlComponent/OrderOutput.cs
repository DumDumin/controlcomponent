using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlComponent
{
    public class OrderOutput : IExecutionState
    {
        //TODO throw errors instead
        // public enum OrderOutputError { OK, Completed, Stopped, NotExisting, NullRequested, NotExecuting, NotAccepted, Occupied };

        // public ControlComponentUnity Cc;

        // public OrderOutputError Error;
        // // Used to manage coroutines in ExecuteOpMode macro
        // public IEnumerator Coroutine;

        // TOBI create Reset method to auto assign cc

        private ControlComponent controlComponent;

        public string Role { get; }
        public ExecutionState EXST => controlComponent.EXST;

        public OrderOutput(string role, ControlComponent cc)
        {
            Role = role;
            controlComponent = cc;
        }

        public async Task SelectOperationMode(OperationMode operationMode, IEnumerable<OrderOutput> outputs)
        {
            await controlComponent.SelectOperationMode(operationMode, outputs);
        }

        public void Reset(string sender)
        {
            controlComponent.Reset(sender);
        }

        // public void Start(string sender)
        // {
        //     ChangeState(ExecutionState.STARTING, sender);
        // }

        // public void Suspend(string sender)
        // {
        //     ChangeState(ExecutionState.SUSPENDING, sender);
        // }

        // public void Unsuspend(string sender)
        // {
        //     ChangeState(ExecutionState.UNSUSPENDING, sender);
        // }
        public void Stop(string sender)
        {
            controlComponent.Stop(sender);
        }

        // public void Hold(string sender)
        // {
        //     ChangeState(ExecutionState.HOLDING, sender);
        // }
        // public void Unhold(string sender)
        // {
        //     ChangeState(ExecutionState.UNHOLDING, sender);
        // }
        // public void Abort(string sender)
        // {
        //     ChangeState(ExecutionState.ABORTING, sender);
        // }
        // public void Clear(string sender)
        // {
        //     ChangeState(ExecutionState.CLEARING, sender);
        // }


    }
}
