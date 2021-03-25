using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class ControlComponentOpModeTests
    {
        ControlComponent cc;

        [SetUp]
        public void Setup()
        {
            cc = new ControlComponent();
        }

        [Test]
        public void Given_Default_When_OpModeName_Then_Default()
        {
            Assert.AreEqual("NONE", cc.OpModeName);
        }

        [Test]
        public void Given_Default_When_SetOpMode_Then_NewOpMode()
        {
            var newOpMode = new OperationMode("NEWOPMODE");
            cc.SetOperationMode(newOpMode);

            Assert.AreEqual(newOpMode.OpModeName, cc.OpModeName);
        }

        // [Test]
        // public async Task Given_Idle_When_SetOpMode_Then_NoChange()
        // {
        //     // await cc.Reset(SENDER);
        //     // await Helper.WaitForState(cc, ExecutionState.IDLE);

        //     var newOpMode = new OperationMode("NEWOPMODE");
        //     cc.SetOperationMode(newOpMode);

        //     Assert.AreEqual("DEFAULT", cc.OpModeName);
        // }
    }
}