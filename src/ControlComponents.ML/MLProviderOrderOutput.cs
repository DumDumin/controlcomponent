using System;
using System.Threading.Tasks;
using ControlComponents.Core;
using ControlComponents.ML;

namespace PTS.ControlComponents
{
    public class MLProviderOrderOutput : OrderOutput
    {
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
                await cc.StopAndWaitForStopped(Id, false);
            }
            else
            {
                throw new InvalidOperationException("Cannot end episode if ml provider not running");
            }
        }

        // TODO use MLENACT like aktion mask
        public static bool[][] ConvertToMLENACT(float[] actionMask)
        {
            // returns action mask => 1 is masked and should not be used
            var action = new bool[1][];
            action[0] = new bool[actionMask.Length];

            for (int i = 0; i < actionMask.Length; i++)
            {
                // if it is masked => false
                if (actionMask[i] == 1)
                    action[0][i] = false;
                else
                    action[0][i] = true;
            }
            return action;
        }

        // TODO seperate Inference from Training
        public async Task<float[]> Decide(float[] observations, float[] actionMask, float reward)
        {
            if(cc.OpModeName == "NONE")
            {
                _running = cc.SelectOperationMode("Inference");
                if(_running.IsFaulted)
                    throw _running.Exception;
            }

            cc.MLREWARD = reward;
            cc.MLOBSERVE = observations;
            cc.MLENACT = ConvertToMLENACT(actionMask);

            await cc.ResetAndWaitForIdle(Id);
            await cc.StartAndWaitForExecute(Id);
            // TODO TIME
            await cc.WaitForCompleted(5000);

            return cc.MLDECIDE;
        }
    }
}