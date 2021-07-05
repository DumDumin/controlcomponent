using System.Threading.Tasks;
using ControlComponents.Core;
using NUnit.Framework;
using PTS.ControlComponents;

namespace ControlComponents.ML.Tests
{
    internal class MLProviderOperationModeTest : MLProviderOperationMode
    {
        private readonly IMLControlComponent cc;

        public MLProviderOperationModeTest(string name, IMLControlComponent cc) : base(name, cc)
        {
            this.cc = cc;
        }

        protected override float[] Decide()
        {
            return cc.MLOBSERVE;
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
            MLControlComponent provider = new MLControlComponent("Provider");
            provider.AddOperationMode(new MLProviderOperationModeTest("MLProvider", provider));

            Assert.AreEqual(null, provider.MLDECIDE);
            var running = provider.SelectOperationMode("MLProvider");

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

            await provider.StopAndWaitForStopped(SENDER, false);
            await provider.DeselectOperationMode();
        }
    }
}