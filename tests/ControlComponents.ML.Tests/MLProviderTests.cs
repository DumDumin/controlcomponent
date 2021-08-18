using System.Threading.Tasks;
using ControlComponents.Core;
using FluentAssertions;
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

        protected override Task<float[][]> Decide()
        {
            // Return Observations as actions
            var result = new float[1][];
            result[0] = cc.MLOBSERVE;
            return Task.FromResult(result);
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
            var properties = new MLProperties(1, new int[1] { 1 });
            MLControlComponent provider = new MLControlComponent("Provider", properties);
            provider.AddOperationMode(new MLProviderOperationModeTest(provider));

            Assert.AreEqual(1, provider.MLDECIDE.Length);
            provider.MLDECIDE[0].Should().BeEquivalentTo(0);
            // Assert.AreEqual(0.0f, provider.MLDECIDE[0]);
            var running = provider.SelectOperationMode("Inference");

            provider.MLOBSERVE = new float[1] { 1 };
            await provider.ResetAndWaitForIdle(SENDER);
            await provider.StartAndWaitForExecute(SENDER);
            await provider.WaitForCompleted();
            provider.MLDECIDE[0].Should().BeEquivalentTo(provider.MLOBSERVE);

            provider.MLOBSERVE = new float[1] { 2 };
            await provider.ResetAndWaitForIdle(SENDER);
            await provider.StartAndWaitForExecute(SENDER);
            await provider.WaitForCompleted();
            provider.MLDECIDE[0].Should().BeEquivalentTo(provider.MLOBSERVE);

            await provider.StopAndWaitForStopped(SENDER);
            Assert.AreEqual(1, provider.MLREWARD);

            await provider.DeselectOperationMode();
        }
    }
}