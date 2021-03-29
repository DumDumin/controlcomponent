using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class ControlComponentNoSelectedOpModeTests
    {
        string SENDER = "SENDER";
        string OCCUPIER = "OCCUPIER";
        ControlComponent cc;

        [SetUp]
        public void Setup()
        {
            cc = new ControlComponent();
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public void Given_NewControlComponent_When_EXST_Then_Stopped()
        {
            ControlComponent cc = new ControlComponent();
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public void Given_Stopped_When_OpModeName_Then_None()
        {
            Assert.AreEqual("NONE", cc.OpModeName);
        }

        [Test]
        public async Task Given_Stopped_When_SetOpMode_Then_NewOpMode()
        {
            var newOpMode = new OperationMode("NEWOPMODE");
            Task runningOpMode = cc.SelectOperationMode(newOpMode);
            Assert.AreEqual(newOpMode.OpModeName, cc.OpModeName);

            await cc.DeselectOperationMode();
            await runningOpMode;
            Assert.AreEqual("NONE", cc.OpModeName);
        }

        [Test]
        public void Given_Stopped_When_Deselect_Then_Throw()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => cc.DeselectOperationMode());
        }

        [Test]
        public void Given_Stopped_When_UserActions_Then_Throw()
        {
            InvalidOperationException e = Assert.Throws<InvalidOperationException>(() => cc.Reset(SENDER));
            Assert.AreEqual($"Cannot change to {ExecutionState.RESETTING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Start(SENDER));
            Assert.AreEqual($"Cannot change to {ExecutionState.STARTING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Stop(SENDER));
            Assert.AreEqual($"Cannot change to {ExecutionState.STOPPING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Suspend(SENDER));
            Assert.AreEqual($"Cannot change to {ExecutionState.SUSPENDING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Unsuspend(SENDER));
            Assert.AreEqual($"Cannot change to {ExecutionState.UNSUSPENDING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Hold(SENDER));
            Assert.AreEqual($"Cannot change to {ExecutionState.HOLDING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Unhold(SENDER));
            Assert.AreEqual($"Cannot change to {ExecutionState.UNHOLDING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Abort(SENDER));
            Assert.AreEqual($"Cannot change to {ExecutionState.ABORTING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Clear(SENDER));
            Assert.AreEqual($"Cannot change to {ExecutionState.CLEARING}, if no operation mode is selected", e.Message);
        }
    }
}