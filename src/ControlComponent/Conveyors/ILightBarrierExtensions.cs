using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponent
{
    public static class ILightBarrierExtensions
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task WaitForLightBarrier(this ILightBarrier lightBarrier, CancellationToken token, bool occupied)
        {
            // The use of linkedTokenSource allows a second cancellation condition to leave this method
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationTokenSource linkedTokens = CancellationTokenSource.CreateLinkedTokenSource(source.Token, token);
            EventHandler ClearToken = (object sender, EventArgs e) =>
            {
                if(lightBarrier.Occupied == occupied)
                {
                    logger.Debug($"Lightbarrier {lightBarrier.Id} triggered correctly to {(occupied ? "occupied" : "free")}");
                    linkedTokens.Cancel();
                }
                else
                {
                    logger.Debug($"Lightbarrier {lightBarrier.Id} triggered wrongly to {(!occupied ? "occupied" : "free")}");
                }
            };

            if(lightBarrier.Occupied != occupied)
            {
                lightBarrier.Hit += ClearToken;
                logger.Debug($"Wait for Lightbarrier {lightBarrier.Id} to get {(occupied ? "occupied" : "free")}");
                await Task.Delay(Timeout.Infinite, linkedTokens.Token).ContinueWith(task => { });
                lightBarrier.Hit -= ClearToken;
            }
        }

    }
}