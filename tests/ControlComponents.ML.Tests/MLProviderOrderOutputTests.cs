using System;
using System.Threading.Tasks;
using ControlComponents.Core;
using Moq;
using NUnit.Framework;
using PTS.ControlComponents;

namespace ControlComponents.ML.Tests
{
    public class MLProviderOrderOutputTests
    {
        string SENDER = "SENDER";

        Mock<IControlComponentProvider> provider;

        [SetUp]
        public void Setup()
        {
            provider = new Mock<IControlComponentProvider>();
        }

        [Test]
        public async Task Given_ProviderOutput_When_Decide_Then_DecisionAvailable()
        {
            var properties = new MLProperties(1, 1);
            MLControlComponent mlprovider = new MLControlComponent("Provider", properties);
            mlprovider.AddOperationMode(new MLProviderOperationModeTest(mlprovider));

            MLProviderOrderOutput output = new MLProviderOrderOutput("Provider", "Provider", provider.Object, mlprovider);

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
            MLControlComponent mlprovider = new MLControlComponent("Provider", properties);
            mlprovider.AddOperationMode(new MLProviderOperationModeTest(mlprovider));

            MLProviderOrderOutput output = new MLProviderOrderOutput("Provider", "Provider", provider.Object, mlprovider);
            await output.Decide(new float[1], new float[1], 0);

            Assert.DoesNotThrowAsync(() => output.EndEpisode(1));
        }

        [Test]
        public void Given_NotRunningProvider_When_EndEpisode_Then_Throw()
        {
            var properties = new MLProperties(1, 1);
            MLControlComponent mlprovider = new MLControlComponent("Provider", properties);
            mlprovider.AddOperationMode(new MLProviderOperationModeTest(mlprovider));

            MLProviderOrderOutput output = new MLProviderOrderOutput("Provider", "Provider", provider.Object, mlprovider);

            Assert.ThrowsAsync(typeof(InvalidOperationException), () => output.EndEpisode(1));
        }

        [Test]
        public async Task Given__When_DecideAndEndEpisode_Then_CorrectStateFlow()
        {
            var properties = new MLProperties(1, 1);
            MLControlComponent mlprovider = new MLControlComponent("Provider", properties);
            mlprovider.AddOperationMode(new MLProviderOperationModeTest(mlprovider));
            MLProviderOrderOutput output = new MLProviderOrderOutput("Provider", "Provider", provider.Object, mlprovider);

            await output.Decide(new float[1], new float[1], 0);
            Assert.AreEqual(ExecutionState.COMPLETED, output.EXST);
            Assert.AreEqual("Inference", output.OpModeName);

            await output.EndEpisode(0);
            Assert.AreEqual(ExecutionState.STOPPED, output.EXST);
            Assert.AreEqual("NONE", output.OpModeName);
        }
    }
}