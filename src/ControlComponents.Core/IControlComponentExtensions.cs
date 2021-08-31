using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public static class IControlComponentExtensions
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task WaitForCompleted(this IControlComponent cc, int delay = 1000)
        {
            StateWaiter waiter = new StateWaiter();
            cc.ExecutionStateChanged += waiter.EventHandler;

            if (cc.EXST != ExecutionState.COMPLETED)
            {
                await waiter.Completed(delay);
            }

            cc.ExecutionStateChanged -= waiter.EventHandler;
        }

        public static async Task WaitForAborted(this IControlComponent cc, int delay = 1000)
        {
            StateWaiter waiter = new StateWaiter();
            cc.ExecutionStateChanged += waiter.EventHandler;

            if (cc.EXST != ExecutionState.ABORTED)
            {
                await waiter.Aborted(delay);
            }

            cc.ExecutionStateChanged -= waiter.EventHandler;
        }

        public static async Task WaitForStopped(this IControlComponent cc, int delay = 1000)
        {
            StateWaiter waiter = new StateWaiter();
            cc.ExecutionStateChanged += waiter.EventHandler;

            if (cc.EXST != ExecutionState.STOPPED)
            {
                await waiter.Stopped(delay);
            }

            cc.ExecutionStateChanged -= waiter.EventHandler;
        }
        public static async Task WaitForIdle(this IControlComponent cc, int delay = 1000)
        {
            StateWaiter waiter = new StateWaiter();
            cc.ExecutionStateChanged += waiter.EventHandler;

            if (cc.EXST != ExecutionState.IDLE)
            {
                await waiter.Idle();
            }

            cc.ExecutionStateChanged -= waiter.EventHandler;
        }

        public static async Task ResetAndWaitForIdle(this IControlComponent cc, string occupier)
        {
            if (cc.EXST == ExecutionState.STOPPED || cc.EXST == ExecutionState.COMPLETED || cc.EXST == ExecutionState.RESETTING)
            {
                StateWaiter waiter = new StateWaiter();
                cc.ExecutionStateChanged += waiter.EventHandler;

                if (cc.EXST != ExecutionState.RESETTING)
                {
                    cc.Reset(occupier);
                }
                await waiter.Idle();

                cc.ExecutionStateChanged -= waiter.EventHandler;
            }
            else
            {
                logger.Warn($"{cc.ComponentName} is in state {cc.EXST}, but must be in STOPPED, COMPLETED OR RESETTING");
            }
        }

        public static async Task StartAndWaitForExecute(this IControlComponent cc, string occupier)
        {
            if (cc.EXST == ExecutionState.IDLE || cc.EXST == ExecutionState.STARTING)
            {
                StateWaiter waiter = new StateWaiter();
                cc.ExecutionStateChanged += waiter.EventHandler;

                if (cc.EXST == ExecutionState.IDLE)
                {
                    cc.Start(occupier);
                }
                await waiter.Execute();

                cc.ExecutionStateChanged -= waiter.EventHandler;
            }
            else
            {
                logger.Warn($"{cc.ComponentName} is in state {cc.EXST}, but must be in IDLE OR STARTING");
            }
        }

        public static async Task SuspendAndWaitForSuspended(this IControlComponent cc, string occupier)
        {
            if (cc.EXST == ExecutionState.EXECUTE || cc.EXST == ExecutionState.SUSPENDING)
            {
                StateWaiter waiter = new StateWaiter();
                cc.ExecutionStateChanged += waiter.EventHandler;

                cc.Suspend(occupier);
                await waiter.Suspend();

                cc.ExecutionStateChanged -= waiter.EventHandler;
            }
            else
            {
                logger.Warn($"{cc.ComponentName} is in state {cc.EXST}, but must be in EXECUTING OR SUSPENDING");
            }
        }

        public static async Task HoldAndWaitForHeld(this IControlComponent cc, string occupier)
        {
            if (cc.EXST == ExecutionState.EXECUTE || cc.EXST == ExecutionState.HOLDING)
            {
                StateWaiter waiter = new StateWaiter();
                cc.ExecutionStateChanged += waiter.EventHandler;

                if (cc.EXST == ExecutionState.EXECUTE)
                {
                    cc.Hold(occupier);
                }
                await waiter.Held();

                cc.ExecutionStateChanged -= waiter.EventHandler;
            }
            else
            {
                logger.Warn($"{cc.ComponentName} is in state {cc.EXST}, but must be in EXECUTING OR HOLDING");
            }
        }

        public static async Task AbortAndWaitForAborted(this IControlComponent cc, string occupier)
        {
            if (cc.EXST != ExecutionState.ABORTED)
            {
                StateWaiter waiter = new StateWaiter();
                cc.ExecutionStateChanged += waiter.EventHandler;

                if (cc.EXST != ExecutionState.ABORTING)
                {
                    cc.Abort(occupier);
                }

                await waiter.Aborted();

                cc.ExecutionStateChanged -= waiter.EventHandler;
            }
            else
            {
                logger.Warn($"{cc.ComponentName} is already in state {cc.EXST}");
            }
        }

        public static async Task StopPrioAndWaitForStopped(this IControlComponent cc, string occupier, int delay = 1000)
        {
            if (cc.EXST != ExecutionState.STOPPED)
            {
                // Occupy with higher priority to overwrite exisiting occupations
                cc.Prio(occupier);
                await cc.StopAndWaitForStopped(occupier, delay);
                // Downgrade occupation from PRIO to OCCUPIED if necessary
                cc.Occupy(occupier);
            }
        }

        public static async Task StopAndWaitForStopped(this IControlComponent cc, string occupier, int delay = 1000)
        {
            // Bring control component to IDLE
            if (cc.EXST != ExecutionState.STOPPED)
            {
                StateWaiter waiter = new StateWaiter();
                cc.ExecutionStateChanged += waiter.EventHandler;

                // TODO might not be good to automatically move from aborted to stopped
                if (cc.EXST == ExecutionState.ABORTING)
                {
                    await waiter.Aborted();
                    cc.Clear(occupier);
                    await waiter.Stopped(delay);
                }
                else if (cc.EXST == ExecutionState.ABORTED)
                {
                    cc.Clear(occupier);
                    await waiter.Stopped(delay);
                }
                else if (cc.EXST == ExecutionState.CLEARING)
                {
                    await waiter.Stopped(delay);
                }
                else if (cc.EXST == ExecutionState.STOPPING)
                {
                    await waiter.Stopped(delay);
                }
                else
                {
                    // All other states are allowed to exit via stop
                    cc.Stop(occupier);
                    await waiter.Stopped(delay);
                }

                cc.ExecutionStateChanged -= waiter.EventHandler;
            }

        }
    }
}