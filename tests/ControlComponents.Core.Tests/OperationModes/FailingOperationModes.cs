using System.Threading;
using System.Threading.Tasks;

namespace ControlComponents.Core.Tests
{
    internal class FailingOperationModeReset : OperationModeBase
    {
        public FailingOperationModeReset(string name) : base(name)
        {
        }

        protected override Task Resetting(CancellationToken token)
        {
            base.execution.SetState(ExecutionState.ABORTING);
            return base.Resetting(token);
        }
    }

    internal class FailingOperationModeStart : OperationModeBase
    {
        public FailingOperationModeStart(string name) : base(name)
        {
        }

        protected override Task Starting(CancellationToken token)
        {
            base.execution.SetState(ExecutionState.ABORTING);
            return base.Starting(token);
        }
    }

    internal class FailingOperationModeExecute : OperationModeBase
    {
        public FailingOperationModeExecute(string name) : base(name)
        {
        }

        protected override Task Execute(CancellationToken token)
        {
            base.execution.SetState(ExecutionState.ABORTING);
            return base.Execute(token);
        }
    }
}