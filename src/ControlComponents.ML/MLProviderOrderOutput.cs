using System.Threading.Tasks;
using ControlComponents.Core;
using ControlComponents.ML;

namespace PTS.ControlComponents
{
    public class MLProviderOrderOutput : OrderOutput
    {
        private readonly IMLControlComponent cc;

        public MLProviderOrderOutput(string role, IMLControlComponent cc) : base(role, cc)
        {
            this.cc = cc;
        }

        public async Task<float[]> Decide(float[] observations, float[] actionMask)
        {
            cc.MLOBSERVE = observations;
            // cc.MLENACT = actionMask;

            // TODO Output needs OCCUPIER
            await cc.ResetAndWaitForIdle("OCCUPIER");
            await cc.StartAndWaitForExecute("OCCUPIER");
            // TODO TIME
            await cc.WaitForCompleted(5000);

            return cc.MLDECIDE;
        }
    }
}