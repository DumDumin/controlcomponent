using NLog;
using System.Collections.Generic;

namespace ControlComponent
{
    internal class Execution
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public ExecutionState EXST { get ; private set; } = ExecutionState.STOPPED;

        private readonly Dictionary<ExecutionState, List<ExecutionState>> allowedTransitions;

        public Execution()
        {
            allowedTransitions = new Dictionary<ExecutionState, List<ExecutionState>>()
            {
                { ExecutionState.STOPPED    , new List<ExecutionState>() {ExecutionState.IDLE} },
                { ExecutionState.IDLE       , new List<ExecutionState>() {ExecutionState.EXECUTE} },
                { ExecutionState.EXECUTE    , new List<ExecutionState>() {ExecutionState.SUSPENDED} }
            };
        }

        public void SetState(ExecutionState newState)
        {
            if(EXST == newState)
                return;

            if(allowedTransitions[EXST].Contains(newState))
            {
                logger.Debug($"Changed state from {EXST} to {newState}");
                EXST = newState;
            }
            else
            {
                logger.Warn($"Not allowed to change state from {EXST} to {newState}");
            }
        }
    }
}