using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponents.Core.Tests
{
    // TODO extend IControlComponentExtension to remove this Helper class
    public static class Helper
    {
        public static async Task WaitForState(IExecutionState cc, ExecutionState state)
        {
            int counter = 0;
            while (cc.EXST != state)
            {
                if (counter++ > 100)
                {
                    throw new TaskCanceledException($"Took too long to change to state {state}. Last state was {cc.EXST}");
                }
                await Task.Delay(1);
            }
        }

        public static async Task WaitForState(ControlComponent cc, ExecutionState state)
        {
            int counter = 0;
            while (cc.EXST != state)
            {
                if (counter++ > 100)
                {
                    throw new TaskCanceledException($"Took too long to change to state {state}. Last state was {cc.EXST}");
                }
                await Task.Delay(1);
            }
        }
    }
}