using System.Threading.Tasks;

namespace ControlComponent
{

    public static class IControlComponentExtensions
    {
        public static async Task ResetAndWaitForIdle(this IControlComponent cc, string occupier)
        {
            StateWaiter waiter = new StateWaiter();
            cc.ExecutionStateChanged += waiter.EventHandler;

            cc.Reset(occupier);
            await waiter.Idle();

            cc.ExecutionStateChanged -= waiter.EventHandler;
        }

        public static async Task StartAndWaitForExecute(this IControlComponent cc, string occupier)
        {
            StateWaiter waiter = new StateWaiter();
            cc.ExecutionStateChanged += waiter.EventHandler;

            cc.Start(occupier);
            await waiter.Execute();

            cc.ExecutionStateChanged -= waiter.EventHandler;
        }

        public static async Task SuspendAndWaitForSuspended(this IControlComponent cc, string occupier)
        {
            StateWaiter waiter = new StateWaiter();
            cc.ExecutionStateChanged += waiter.EventHandler;

            cc.Suspend(occupier);
            await waiter.Suspend();

            cc.ExecutionStateChanged -= waiter.EventHandler;
        }

        public static async Task HoldAndWaitForHeld(this IControlComponent cc, string occupier)
        {
            StateWaiter waiter = new StateWaiter();
            cc.ExecutionStateChanged += waiter.EventHandler;

            cc.Hold(occupier);
            await waiter.Held();

            cc.ExecutionStateChanged -= waiter.EventHandler;
        }

        public static async Task AbortAndWaitForAborted(this IControlComponent cc, string occupier)
        {
            StateWaiter waiter = new StateWaiter();
            cc.ExecutionStateChanged += waiter.EventHandler;

            cc.Abort(occupier);
            await waiter.Aborted();

            cc.ExecutionStateChanged -= waiter.EventHandler;
        }

        public static async Task StopAndWaitForStopped(this IControlComponent cc, string occupier, bool free, int delay = 1000)
        {
            // Bring control component to IDLE
            if (cc.EXST != ExecutionState.STOPPED)
            {
                // Occupy with higher priority to overwrite exisiting occupations
                cc.Prio(occupier);

                StateWaiter waiter = new StateWaiter();
                cc.ExecutionStateChanged += waiter.EventHandler;

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

            // TODO
            // Free component if desired or leave occupied by the environment
            if (free)
            {
                if(cc.IsOccupied())
                    cc.Free(occupier);
            }
            else
            {
                // Downgrade occupation from PRIO to OCCUPIED if necessary
                cc.Occupy(occupier);
            }
        }
    }
}