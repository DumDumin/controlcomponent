using System.Threading.Tasks;
using ControlComponents.Core;
using NUnit.Framework;

namespace ControlComponents.ML.Tests
{
    internal class MLProviderOperationModeTest : MLProviderOperationMode
    {
        private readonly IMLControlComponent cc;

        public MLProviderOperationModeTest(IMLControlComponent cc) : base(cc)
        {
            this.cc = cc;
        }

        protected override Task<float[]> Decide()
        {
            // Return Observations as actions
            return Task.FromResult(cc.MLOBSERVE);
        }

        protected override Task EndEpisode()
        {
            cc.MLREWARD = 1;
            return Task.CompletedTask;
        }
    }

    public class MLProviderTests
    {
        string SENDER = "SENDER";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Given_Provider_When_ExecuteProvider_Then_DecisionAvailable()
        {
            var properties = new MLProperties(1, 1);
            MLControlComponent provider = new MLControlComponent("Provider", properties);
            provider.AddOperationMode(new MLProviderOperationModeTest(provider));

            Assert.AreEqual(new float[1], provider.MLDECIDE);
            var running = provider.SelectOperationMode("Inference");

            provider.MLOBSERVE = new float[1] { 1 };
            await provider.ResetAndWaitForIdle(SENDER);
            await provider.StartAndWaitForExecute(SENDER);
            await provider.WaitForCompleted();
            Assert.AreEqual(provider.MLOBSERVE, provider.MLDECIDE);

            provider.MLOBSERVE = new float[1] { 2 };
            await provider.ResetAndWaitForIdle(SENDER);
            await provider.StartAndWaitForExecute(SENDER);
            await provider.WaitForCompleted();
            Assert.AreEqual(provider.MLOBSERVE, provider.MLDECIDE);

            await provider.StopAndWaitForStopped(SENDER);
            Assert.AreEqual(1, provider.MLREWARD);

            await provider.DeselectOperationMode();
        }
    }
}