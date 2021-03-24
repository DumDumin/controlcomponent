using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class ControlComponentTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Given_NewControlComponent_When_EXST_Then_Stopped()
        {
            ControlComponent cc = new ControlComponent();
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public void Given_StoppedControlComponent_When_Reset_Then_Idle()
        {
            ControlComponent cc = new ControlComponent();
            cc.Reset();
            Assert.AreEqual(ExecutionState.IDLE, cc.EXST);
        }

        [Test]
        public void Given_StoppedControlComponent_When_Start_Then_Stopped()
        {
            ControlComponent cc = new ControlComponent();
            cc.Start();
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public void Given_IdleControlComponent_When_Start_Then_Execute()
        {
            ControlComponent cc = new ControlComponent();
            cc.Reset();
            cc.Start();
            Assert.AreEqual(ExecutionState.EXECUTE, cc.EXST);
        }

        [Test]
        public void Given_ExecuteControlComponent_When_Suspend_Then_Suspended()
        {
            ControlComponent cc = new ControlComponent();
            cc.Reset();
            Assert.AreEqual(ExecutionState.IDLE, cc.EXST);
            cc.Start();
            Assert.AreEqual(ExecutionState.EXECUTE, cc.EXST);
            cc.Suspend();
            Assert.AreEqual(ExecutionState.SUSPENDED, cc.EXST);
        }
    }
}