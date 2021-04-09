using System.Threading.Tasks;

namespace ControlComponent
{

    public static class ExecutionStateHelper
    {
        public static async Task StopAndWaitForStopped(IControlComponent cc, string occupier, bool free)
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
                    await waiter.Stopped();
                }
                else if (cc.EXST == ExecutionState.ABORTED)
                {
                    cc.Clear(occupier);
                    await waiter.Stopped();
                }
                else if (cc.EXST == ExecutionState.CLEARING)
                {
                    await waiter.Stopped();
                }
                else
                {
                    // All other states are allowed to exit via stop
                    cc.Stop(occupier);
                    await waiter.Stopped();
                }

                cc.ExecutionStateChanged -= waiter.EventHandler;
            }

            // TODO
            // // Free component if desired or leave occupied by the environment
            // if (free)
            // {
            //     if(cc.IsOccupied())
            //         cc.Free(name);
            // }
            // else
            // {
            //     // Downgrade occupation from PRIO to OCCUPIED if necessary
            //     cc.Occupy(name);
            // }
        }
    }
}