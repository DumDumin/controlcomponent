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

        private string name;

        public Execution(string name)
        {
            this.name = name;

            allowedTransitions = new Dictionary<ExecutionState, List<ExecutionState>>()
            {
                { ExecutionState.STOPPED        , new List<ExecutionState>() {ExecutionState.RESETTING, ExecutionState.ABORTING} },
                { ExecutionState.RESETTING      , new List<ExecutionState>() {ExecutionState.IDLE} },
                { ExecutionState.IDLE           , new List<ExecutionState>() {ExecutionState.STARTING, ExecutionState.STOPPING, ExecutionState.ABORTING} },
                { ExecutionState.STARTING       , new List<ExecutionState>() {ExecutionState.EXECUTE} },
                { ExecutionState.EXECUTE        , new List<ExecutionState>() {ExecutionState.SUSPENDING, ExecutionState.HOLDING, ExecutionState.COMPLETING, ExecutionState.STOPPING} },
                { ExecutionState.COMPLETING     , new List<ExecutionState>() {ExecutionState.COMPLETED} },
                { ExecutionState.COMPLETED      , new List<ExecutionState>() {ExecutionState.ABORTING, ExecutionState.STOPPING} },

                { ExecutionState.HOLDING        , new List<ExecutionState>() {ExecutionState.HELD} },
                { ExecutionState.HELD           , new List<ExecutionState>() {ExecutionState.UNHOLDING} },
                { ExecutionState.UNHOLDING      , new List<ExecutionState>() {ExecutionState.EXECUTE} },

                { ExecutionState.SUSPENDING     , new List<ExecutionState>() {ExecutionState.SUSPENDED} },
                { ExecutionState.SUSPENDED      , new List<ExecutionState>() {ExecutionState.UNSUSPENDING, ExecutionState.STOPPING} },
                { ExecutionState.UNSUSPENDING   , new List<ExecutionState>() {ExecutionState.EXECUTE} },

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
                logger.Debug($"{name} changed state from {EXST} to {newState}");
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