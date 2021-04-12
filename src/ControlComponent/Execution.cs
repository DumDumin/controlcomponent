using NLog;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("ControlComponent.Tests")]
namespace ControlComponent
{
    internal class Execution : IExecution
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public ExecutionState EXST { get ; private set; } = ExecutionState.STOPPED;

        public event ExecutionStateEventHandler ExecutionStateChanged;

        private readonly Dictionary<ExecutionState, List<ExecutionState>> allowedTransitions;

        public string ComponentName { get; }

        public Execution(string name)
        {
            ComponentName = name;

            allowedTransitions = new Dictionary<ExecutionState, List<ExecutionState>>()
            {
                { ExecutionState.STOPPED        , new List<ExecutionState>() {ExecutionState.RESETTING, ExecutionState.ABORTING} },
                { ExecutionState.RESETTING      , new List<ExecutionState>() {ExecutionState.IDLE, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.IDLE           , new List<ExecutionState>() {ExecutionState.STARTING, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.STARTING       , new List<ExecutionState>() {ExecutionState.EXECUTE, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.EXECUTE        , new List<ExecutionState>() {ExecutionState.SUSPENDING, ExecutionState.HOLDING, ExecutionState.COMPLETING, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.COMPLETING     , new List<ExecutionState>() {ExecutionState.COMPLETED, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.COMPLETED      , new List<ExecutionState>() {ExecutionState.ABORTING, ExecutionState.STOPPING} },

                { ExecutionState.HOLDING        , new List<ExecutionState>() {ExecutionState.HELD, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.HELD           , new List<ExecutionState>() {ExecutionState.UNHOLDING, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.UNHOLDING      , new List<ExecutionState>() {ExecutionState.EXECUTE, ExecutionState.STOPPING, ExecutionState.ABORTING} },

                { ExecutionState.SUSPENDING     , new List<ExecutionState>() {ExecutionState.SUSPENDED, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.SUSPENDED      , new List<ExecutionState>() {ExecutionState.UNSUSPENDING, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.UNSUSPENDING   , new List<ExecutionState>() {ExecutionState.EXECUTE, ExecutionState.STOPPING, ExecutionState.ABORTING} },

                { ExecutionState.ABORTING       , new List<ExecutionState>() {ExecutionState.ABORTED} },
                { ExecutionState.ABORTED        , new List<ExecutionState>() {ExecutionState.CLEARING} },
                { ExecutionState.CLEARING       , new List<ExecutionState>() {ExecutionState.STOPPED} },
                { ExecutionState.STOPPING       , new List<ExecutionState>() {ExecutionState.STOPPED} },
            };
        }

        public void SetState(ExecutionState newState)
        {
            // if(EXST == newState)
            //     return;

            if(allowedTransitions[EXST].Contains(newState))
            {
                logger.Debug($"{ComponentName} changed state from {EXST} to {newState}");
                EXST = newState;
                ExecutionStateChanged?.Invoke(this, new ExecutionStateEventArgs(EXST));
            }
            else
            {
                throw new ExecutionException($"Not allowed to change from {EXST} to {newState}");
            }
        }
    }
}