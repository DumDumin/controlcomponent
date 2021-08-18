// using System;
// using System.Collections;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using ControlComponents.Core;

// namespace ControlComponents.ML
// {
//     /// <summary>
//     /// This is a abstract <see cref="OperationMode"/> for ML.
//     /// It dispatches between the <see cref="ControlComponent"/> via <see cref="OperationMode.control"/>
//     /// and the <see cref="MLModel"/> via <see cref="IMLOperationMode"/>.
//     /// </summary>
//     /// <remarks>
//     /// To implement a specific MLOperationMode one should at least override the
//     /// <see cref="MapDecisionToAction"/> and the <see cref="CollectObservations"/> function.
//     /// Additionally terminal conditions have to be checked and reported by setting
//     /// <c>cc.EXST = ExecutionState.COMPLETING;</c>
//     /// in case of success or by setting
//     /// <c>cc.EXST = ExecutionState.ABORTING;</c>
//     /// in case of failure.
//     /// This can be done by overriding <see cref="OperationMode.OnSuspend"/>.
//     /// <para/>
//     /// Generic rewards can be configured via parameters starting with "R".
//     /// Additional task (=operation mode) specific rewards can be applied by setting
//     /// <c>MLREWARD += value;</c>
//     /// </remarks>
//     public abstract class MLUsingOperationMode : OperationModeWaitOutputs
//     {
//         private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

//         IMLControlComponent cc;

//         public MLUsingOperationMode(string name, IMLControlComponent cc) : base(name)
//         {
//             this.cc = cc;
//         }

//         // TODO: Maybe its usefull to have another action value for that(0 =Don't change, -1 = stop, 1 = operation mode1 , ...)
//         // [Header("Action Settings")]
//         // [Tooltip("Stop the execution of the order output operation mode on action value of 0(=BSTATE).")]
//         public bool StopExecutionOnAction0 = false;
//         // [Tooltip("Allow action every step count number of steps, even if some order output is executing." +
//         //     " Allows agent to change operation mode or stop operation mode if StopExecutionOnAction0 is true." +
//         //     " 0 means actions are allowed in every step." +
//         //     " -1 or negative value means don't allow new action, before all outputs are idle.")]
//         public int ActionTimeout = -1;
//         // [Tooltip("Maximum number of steps to wait, till action should have started after selection (during unsuspending)." +
//         //     " Negative values may lead to locks in UNSUSPENDING, e.g. an unknown operation mode was selected.")]
//         public int ActionStartTimeout = -1;

//         // [Header("Generic Rewards")]
//         // positive:
//         public float RTaskCompleted = 1f; // Ends Episode
//         public float ROrderCompleted = 0.001f;
//         // negative:
//         public float RActionTimeout = -0.001f;
//         public float ROrderRejected = -0.001f;
//         public float ROutputOccupied = -0.001f;
//         public float RTaskFailed = -1f; // Ends Episode

//         uint actionStepCount = 0;
//         uint actionStartCount = 0;

//         /// <summary>
//         /// Generic ML operation mode execution cycle.
//         /// Handles the communication with the <see cref="MLModel"/>
//         /// via the <see cref="IMLOperationMode"/> interface
//         /// identified by <see cref="MLMODEL"/> string value.
//         /// </summary>
//         /// <remarks>
//         /// At first a communication to the MLModel is build up during selection phase and STARTING state.
//         /// Basicly the SUSPENDING, SUSPENDED, UNSUSPENDING, EXECUTE state
//         /// cycle is used to observe, let MLModel decide, carry out actions and add rewards.
//         /// The HOLDING, HELD, UNHOLDING cycle is ignored and may be overriden.
//         /// The MLModel is notified about an episode end,if the execution is compledted, aborted or stopped.
//         /// Necessary OnSTATEXY functions, e.g. <see cref="OperationMode.OnSuspended()"/>
//         /// and virtual functions for observations and actions are called.
//         /// <para/>
//         /// In this generic operation mode execution some generic (task agnostic) rewards are applied.
//         /// See properties starting with "R...", e.g. <see cref="RTaskCompleted"/>.
//         /// TODO Check MLSC for requested changes from the MLModel (e.g. abort , stop)
//         /// </remarks>
//         /// <returns>Is executed as Unity Co-Routine, hence it returns an iterator.</returns>
//         protected override void Selected()
//         {
//             // Check that control component is a group control component and has order outputs.
//             if (base.outputs.Count == 0)
//             {
//                 base.WORKST = "OrderOutputError"; //TODO use ER state of CC instead
//             }
//             else
//             {
//                 base.WORKST = "READY"; // TODO ControlComponent.WORKST_READY;
//             }
//         }

//         /// <summary>
//         /// Indicates the MLModel to start its execution via <see cref="MLSC"/>.
//         /// </summary>
//         protected override async Task Starting(CancellationToken token)
//         {
//             logger.Debug("MLOPMODE - Starting");
//             // TOBI this is a reset of the MLSC input
//             // For real application: OCCUPY, (set all to SIMULATE execution mode, depending on EXMODE)
//             cc.MLSC = ExecutionState.STARTING;
//             // Check connection to MLModel and start new episode --> Handle in OnSelected cycle, so don't call base.OnStart();

//             while (!token.IsCancellationRequested)
//             {
//                 if (cc.MLSC == ExecutionState.EXECUTE)
//                 {
//                     // The ML-Model is ready the component can leave STARTING state and enter Execute
//                     await base.Starting(token);
//                     break;
//                 }
//                 else
//                 {
//                     logger.Info($"Waiting for MLSC to Execute is = {cc.MLSC}");
//                     await Task.Delay(10);
//                 }
//             }

//             // Wo do not change state here, because the token cancellation should be trigged by a state change externally
//             await Task.CompletedTask;
//         }

//         protected abstract bool TargetReached();

//         protected override async Task Execute(CancellationToken token)
//         {
//             logger.Warn("Enter Execute - MLOperationMode");
//             while (!token.IsCancellationRequested)
//             {
//                 // If all outputs did the job
//                 if (base.outputs.Values.All(output =>
//                     output == null
//                     || output.IsFree()
//                     || output.OCCUPIER == base.execution.ComponentName
//                         && (output.EXST == ExecutionState.COMPLETED
//                         || output.EXST == ExecutionState.STOPPED
//                         // TOBI TODO IDLE is not correct anymore
//                         || output.EXST == ExecutionState.IDLE)))
//                 {
//                     // Check if target is reached
//                     if (TargetReached())
//                     {
//                         if (!token.IsCancellationRequested)
//                         {
//                             base.WORKST = "TargetReached";
//                             base.execution.SetState(ExecutionState.COMPLETING);
//                             await Task.CompletedTask;
//                         }
//                     }
//                     else
//                     {
//                         if (!token.IsCancellationRequested)
//                         {
//                             base.execution.SetState(ExecutionState.SUSPENDING);
//                             await Task.CompletedTask;
//                         }
//                     }
//                 }
//                 // Check if ActionTimeout is reached
//                 else if (ActionTimeout > 0 && actionStepCount++ > ActionTimeout)
//                 {
//                     if (!token.IsCancellationRequested)
//                     {
//                         cc.MLREWARD += RActionTimeout;
//                         base.execution.SetState(ExecutionState.SUSPENDING);
//                         await Task.CompletedTask;
//                     }
//                 }
//                 else
//                 {
//                     logger.Warn("Execute - MLOperationMode - Wait for Outputs");
//                     await Task.Delay(10);
//                 }
//                 // TOBI do we need any completing condition ??
//             }
//         }

//         protected override async Task Suspending(CancellationToken token)
//         {
//             foreach (var output in base.outputs.Values.Where(o => o != null))
//             {
//                 // Check order outputs for errors
//                 // TODO check correct reset of OrderOutputs to OrderOutputError.OK
//                 switch (output.Error)
//                 {
//                     case OrderOutputError.NotAccepted:
//                     case OrderOutputError.NotExecuting:
//                     case OrderOutputError.NotExisting:
//                     case OrderOutputError.NullRequested:
//                         cc.MLREWARD += ROrderRejected;
//                         break;
//                     case OrderOutputError.Occupied:
//                         cc.MLREWARD += ROutputOccupied;
//                         break;
//                     case OrderOutputError.Stopped:
//                     case OrderOutputError.Completed:
//                         cc.MLREWARD += ROrderCompleted;
//                         break;
//                 }

//                 // Reset and free stopped or completed occupied outputs
//                 if (output != null && output.OCCUPIER == base.execution.ComponentName &&
//                     (output.EXST == ExecutionState.COMPLETED
//                     || output.EXST == ExecutionState.STOPPED
//                     || output.EXST == ExecutionState.IDLE))
//                 {
//                     await output.StopAndWaitForStopped(base.execution.ComponentName);
//                     // if (output.EXST != ExecutionState.IDLE)
//                     //     output.Reset(base.execution.ComponentName);
//                     // // TODO maybe its better to free only in UNSUSPENDING or add a call for FreeOutputs() and overwrite it application specific
//                     // output.Free(base.execution.ComponentName);
//                 }
//             }
//             cc.MLSC = ExecutionState.SUSPENDED;
//             // Get observations and mask out currently available options
//             CollectObservations();
//             SetEnabledActions();
//             //TODO Call a new virtual function to check for goal reached?

//             // OnSuspending();
//             // Reset the MLSC value, to correctly identify changes in the next phase (SUSPENDED)
//             await base.Suspending(token);
//         }

//         protected override async Task Suspended(CancellationToken token)
//         {
//             while (!token.IsCancellationRequested)
//             {
//                 // TODO in SEMI-AUTO Betriebsart (operating mode) there is an acceptance input required to execute the decision
//                 if (cc.MLSC == ExecutionState.UNSUSPENDING && cc.MLDECIDE != null)
//                 {
//                     logger.Warn("Received Decisions");
//                     // Execute decisions as actions
//                     await MapDecisionToAction();
//                     // Reset reward for next observation, action, reward cycle
//                     cc.MLREWARD = 0;
//                     if (!token.IsCancellationRequested)
//                     {
//                         execution.SetState(ExecutionState.UNSUSPENDING);
//                     }
//                     await Task.CompletedTask;
//                 }
//                 else
//                 {
//                     logger.Debug("Wait in Suspended");
//                     await Task.Delay(10);
//                 }
//             }
//         }

//         private bool OutputsRunning(CancellationToken token)
//         {
//             // Wait till any orderOutput is started
//             foreach (var output in base.outputs.Values)
//             {
//                 if (output != null
//                     && output.OCCUPIER == base.execution.ComponentName
//                     // TOBI TODO outout might be done before it is requested here -> MLOpModePalette line 250
//                     && (output.EXST == ExecutionState.EXECUTE || output.EXST == ExecutionState.COMPLETED))
//                 {
//                     actionStepCount = 0;
//                     actionStartCount = 0;

//                     return true;
//                 }
//             }
//             return false;
//         }

//         protected override async Task Unsuspending(CancellationToken token)
//         {
//             while (!token.IsCancellationRequested)
//             {
//                 if (OutputsRunning(token))
//                 {
//                     await base.Unsuspending(token);
//                 }
//                 // Alternatively check if action start timeout is reached.
//                 else if (ActionStartTimeout > 0 && actionStartCount++ > ActionStartTimeout)
//                 {
//                     cc.MLREWARD += RActionTimeout;
//                     actionStepCount = 0;
//                     actionStartCount = 0;
//                     await base.Unsuspending(token);
//                 }
//                 else
//                 {
//                     logger.Warn("Wait for Outputs to enter EXECUTE");
//                     await Task.Delay(10);
//                 }
//             }
//         }

//         protected override async Task Completing(CancellationToken token)
//         {
//             cc.MLSTATS = null;
//             // Indicate, that completing can be done
//             cc.MLSC = ExecutionState.COMPLETING;
//             cc.MLREWARD = RTaskCompleted;
//             // Wait for EndEpisode and write back MLSTATS
//             // yield return new WaitUntil(() => MLSC == ExecutionState.COMPLETED);
//             while (cc.MLSC != ExecutionState.COMPLETED && !token.IsCancellationRequested)
//             {
//                 await Task.Delay(25);
//             }

//             await base.Completing(token);
//         }

//         protected override async Task Stopping(CancellationToken token)
//         {
//             logger.Debug("MLOPMODE - Stopping");
//             //TODO distinguish between STOP and ABORT? (e.g. don't apply RTaskFailed on STOP)
//             cc.MLREWARD = RTaskFailed;
//             await base.Stopping(token);
//         }

//         protected override async Task Aborting(CancellationToken token)
//         {
//             cc.MLREWARD = RTaskFailed;
//             await base.Aborting(token);
//         }

//         protected override async Task Resetting(CancellationToken token)
//         {
//             cc.MLREWARD = 0;
//             await base.Resetting(token);
//         }

//         #region Observation and action functions
//         /// <summary>
//         /// Triggers the collection of observations to update <see cref="MLOBSERVE"/> variable.
//         /// Resets all observations if not overwritten via <see cref="CollectObservations"/> or <see cref="OperationMode.OnSuspend"/>.
//         /// </summary>
//         /// <remarks>
//         /// <c>MLOBSERVE = null</c> tells the <see cref="MLModel"/> that no observations where made.
//         /// TODO Discuss whether more (generic) information (of OrderOutputs) should be added, e.g. OPMODE, EXST.
//         /// </remarks>
//         protected virtual void CollectObservations()
//         {
//             cc.MLOBSERVE = null;
//         }

//         /// <summary>
//         /// Initiates the requested decision as new actions.
//         /// This function is called if the decisions in <see cref="MLDECIDE"/> should be carried 
//         /// out as actions. It corresponds to <see cref="Unity.MLAgents.Agent.OnActionReceived(float[])"/>
//         /// and can be overriden in ML operation modes to use application specific
//         /// decision (action) spaces.
//         /// <para/>
//         /// If it is not overriden a group control component is assumed.
//         /// Then every action branch is mapped to an order output and 
//         /// each action value to an operation mode of the corresponding control
//         /// component that is currently mapped to the order output.
//         /// </summary>
//         /// <remarks>
//         /// Group control components may
//         /// <list type="bullet">
//         /// <item>use the ExecuteOpMode macro </item>
//         /// <item>directly call orders on the order outputs</item>
//         /// <item>use control component specific skills(NotImplemented)</item>
//         /// <item>use other operation modes of this control component</item>
//         /// </list>
//         /// Single control components may use their field devices directly, e.g. read 
//         /// sensors or change set points.
//         /// </remarks>
//         protected virtual async Task MapDecisionToAction()
//         {
//             for (int branch = 0; branch < cc.MLDECIDE.Length; branch++)
//             {
//                 if (branch >= 0 && branch < base.outputs.Count)
//                 {
//                     int decision = (int)Math.Round(cc.MLDECIDE[branch]);
//                     IOrderOutput output = base.outputs.Values.ElementAt(branch);
//                     if (output != null && decision >= 0 && decision < output.OpModes.Count)
//                     {
//                         // TODO how to handle Task, which is returned here? "await cc.RunningOpMode" ??
//                         Task a = output.SelectOperationMode(output.OpModes.ElementAt(decision));
//                         await output.ResetAndWaitForIdle(base.execution.ComponentName);
//                         await output.StartAndWaitForExecute(base.execution.ComponentName);
//                         // control.ExecuteOpMode(
//                         //     control.OrderOutputs.ElementAt(branch).Key,
//                         //     cc.OperationModes.ElementAt(decision).Key);
//                     }
//                 }
//             }
//         }

//         /// <summary>
//         /// Is called while SUSPENDING to indicate the currently available actions
//         /// to the <see cref="MLModel"/> object before it chooses the next actions
//         /// via the <see cref="IMLOperationMode.MLENACT"/> array.
//         /// Enables all actions if not overwritten via <see cref="SetEnabledActions"/>
//         /// or <see cref="OperationMode.OnSuspend"/>.
//         /// </summary>
//         /// <remarks>
//         /// <c>MLENACT = null</c> tells the <see cref="MLModel"/> that no action (of
//         /// its configured action space) is forbidden.
//         /// </remarks>
//         protected virtual void SetEnabledActions()
//         {
//             cc.MLENACT = null;
//         }
//         #endregion Observation and action functions
//     }
// }