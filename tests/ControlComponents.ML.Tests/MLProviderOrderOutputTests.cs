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
            MLControlComponent provider = new MLControlComponent("Provider");
            provider.AddOperationMode(new MLProviderOperationModeTest(provider));

            MLProviderOrderOutput output = new MLProviderOrderOutput("Provider", "Provider", provider);

            await output.Decide(new float[1], new float[1]);


            // Assert.AreEqual(null, provider.MLDECIDE);
            // var running = provider.SelectOperationMode("MLProvider");

            // await provider.StopAndWaitForStopped(SENDER, false);
            // await provider.DeselectOperationMode();
        }
    }
}