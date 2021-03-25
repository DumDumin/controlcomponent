using System.Threading.Tasks;

namespace ControlComponent.Tests
{
    public static class Helper
    {
        public static async Task WaitForState(IExecution cc, ExecutionState state)
        {
            int counter = 0;
            while (cc.EXST != state)
            {
                if (counter++ > 100)
                {
                    throw new TaskCanceledException("Took too long to change state");
                }
                await Task.Delay(1);
            }
        }
    }
}