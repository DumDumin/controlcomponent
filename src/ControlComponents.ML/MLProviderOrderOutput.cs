using System;
using System.Threading.Tasks;
using ControlComponents.Core;

namespace ControlComponents.ML
{
    public class MLProviderOrderOutput : OrderOutput
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IMLControlComponent cc;
        private Task _running = Task.CompletedTask;

        public MLProviderOrderOutput(string role, string id, IControlComponentProvider provider, IMLControlComponent cc) : base(role, id, provider, cc)
        {
            this.cc = cc;
        }

        public async Task EndEpisode(float reward)
        {
            if(_running.Status == TaskStatus.WaitingForActivation)
            {
                cc.MLREWARD = reward;
                await cc.StopAndWaitForStopped(Id);
                await cc.DeselectOperationMode();
            }
            else
            {
                logger.Warn("Cannot end episode if ml provider not running");
            }
        }

        // TODO use MLENACT like aktion mask


        // TODO seperate Inference from Training
        public async Task<float[][]> Decide(float[] observations, float[][] actionMask, float reward)
        {
            if(cc.OpModeName == "NONE")
            {
                await _running;
                _running = cc.SelectOperationMode("Inference");
                if(_running.IsFaulted)
                    throw _running.Exception;
            }

            cc.MLREWARD = reward;
            cc.MLOBSERVE = observations;
            cc.MLENACT = MLControlComponent.ConvertToMLENACT(actionMask);

            await cc.ResetAndWaitForIdle(Id);
            await cc.StartAndWaitForExecute(Id);
            // TODO TIME
            await cc.WaitForCompleted(10000);

            return cc.MLDECIDE;
        }
    }
}