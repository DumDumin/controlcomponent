using System;
using System.Threading.Tasks;
using ControlComponents.Core;
using NUnit.Framework;
using PTS.ControlComponents;

namespace ControlComponents.ML.Tests
{
    public class MLProviderOrderOutputTests
    {
        string SENDER = "SENDER";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Given_ProviderOutput_When_Decide_Then_DecisionAvailable()
        {
            var properties = new MLProperties(1, 1);
            MLControlComponent provider = new MLControlComponent("Provider", properties);
            provider.AddOperationMode(new MLProviderOperationModeTest(provider));

            MLProviderOrderOutput output = new MLProviderOrderOutput("Provider", "Provider", provider);

            await output.Decide(new float[1], new float[1], 0);


            // Assert.AreEqual(null, provider.MLDECIDE);
            // var running = provider.SelectOperationMode("MLProvider");

            // await provider.StopAndWaitForStopped(SENDER, false);
            // await provider.DeselectOperationMode();
        }

        [Test]
        public async Task Given_RunningProvider_When_EndEpisode_Then_DoNotThrow()
        {
            var properties = new MLProperties(1, 1);
            MLControlComponent provider = new MLControlComponent("Provider", properties);
            provider.AddOperationMode(new MLProviderOperationModeTest(provider));

            MLProviderOrderOutput output = new MLProviderOrderOutput("Provider", "Provider", provider);
            await output.Decide(new float[1], new float[1], 0);

            Assert.DoesNotThrowAsync(() => output.EndEpisode(1));
        }

        [Test]
        public void Given_NotRunningProvider_When_EndEpisode_Then_Throw()
        {
            var properties = new MLProperties(1, 1);
            MLControlComponent provider = new MLControlComponent("Provider", properties);
            provider.AddOperationMode(new MLProviderOperationModeTest(provider));

            MLProviderOrderOutput output = new MLProviderOrderOutput("Provider", "Provider", provider);

            Assert.ThrowsAsync(typeof(InvalidOperationException), () => output.EndEpisode(1));
        }
    }
}