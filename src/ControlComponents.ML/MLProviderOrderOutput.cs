using System.Threading.Tasks;
using ControlComponents.Core;
using ControlComponents.ML;

namespace PTS.ControlComponents
{
    public class MLProviderOrderOutput : OrderOutput
    {
        private readonly IMLControlComponent cc;
        private Task _running = Task.CompletedTask;

        public MLProviderOrderOutput(string role, string id, IMLControlComponent cc) : base(role, id, cc)
        {
            this.cc = cc;
        }

        public async Task<float[]> Decide(float[] observations, float[] actionMask)
        {
            if(cc.OpModeName == "NONE")
            {
                _running = cc.SelectOperationMode("Inference");
                if(_running.IsFaulted)
                    throw _running.Exception;
            }

            cc.MLOBSERVE = observations;
            // cc.MLENACT = actionMask;

            await cc.ResetAndWaitForIdle(Id);
            await cc.StartAndWaitForExecute(Id);
            // TODO TIME
            await cc.WaitForCompleted(5000);

            return cc.MLDECIDE;
        }
    }
}