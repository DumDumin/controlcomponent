using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class ControlComponentSelectedOpModeTests
    {
        string SENDER = "SENDER";
        ControlComponent cc;
        Task runningOpMode;
        string CC = "CC";
        string OpModeOne = "OpModeOne";
        string OpModeTwo = "OpModeTwo";

        [SetUp]
        public void Setup()
        {
            Mock<IControlComponentProvider> provider = new Mock<IControlComponentProvider>();
            var OpModes = new Collection<IOperationMode>(){ new OperationModeAsync(OpModeOne), new OperationModeAsync(OpModeTwo) };
            var orderOutputs = new Collection<IOrderOutput>() 
            { 
                new OrderOutput("First", CC, provider.Object, new ControlComponent("CC1", OpModes, new Collection<IOrderOutput>(), new Collection<string>())),
                new OrderOutput("Second", CC, provider.Object, new ControlComponent("CC2", OpModes, new Collection<IOrderOutput>(), new Collection<string>()))
            };
            cc = new ControlComponent(CC, OpModes, orderOutputs, new Collection<string>());
            runningOpMode = cc.SelectOperationMode(OpModeOne);
        }

        [TearDown]
        public async Task TearDown()
        {
            if(cc.EXST != ExecutionState.STOPPED)
            {
                cc.Stop(SENDER);
            }
            
            await cc.StopAndWaitForStopped(SENDER);
            await cc.DeselectOperationMode();
            await runningOpMode;
        }

        [Test]
        public async Task Given_Stopped_When_Reset_Then_Idle()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            Assert.AreEqual(ExecutionState.IDLE, cc.EXST);
        }

        [Test]
        public async Task Given_Stopped_When_NormalRun_Then_Completed()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            Assert.AreEqual(ExecutionState.IDLE, cc.EXST);

            await cc.StartAndWaitForExecute(SENDER);

            await cc.WaitForCompleted();
            Assert.AreEqual(ExecutionState.COMPLETED, cc.EXST);
        }

        [Test]
        public async Task Given_Stopped_When_NormalHoldRun_Then_Completed()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            Assert.AreEqual(ExecutionState.IDLE, cc.EXST);

            await cc.StartAndWaitForExecute(SENDER);

            await cc.HoldAndWaitForHeld(SENDER);
            Assert.AreEqual(ExecutionState.HELD, cc.EXST);

            await cc.UnholdAndWaitExecute(SENDER);
            Assert.AreEqual(ExecutionState.EXECUTE, cc.EXST);

            await cc.WaitForCompleted();
            Assert.AreEqual(ExecutionState.COMPLETED, cc.EXST);
        }

        [Test]
        public async Task Given_Stopped_When_NormalSuspendRun_Then_Completed()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            Assert.AreEqual(ExecutionState.IDLE, cc.EXST);

            await cc.StartAndWaitForExecute(SENDER);

            await cc.SuspendAndWaitForSuspended(SENDER);
            Assert.AreEqual(ExecutionState.SUSPENDED, cc.EXST);

            await cc.UnsuspendAndWaitForExecute(SENDER);
            Assert.AreEqual(ExecutionState.EXECUTE, cc.EXST);

            await cc.WaitForCompleted();
            Assert.AreEqual(ExecutionState.COMPLETED, cc.EXST);
        }

        [Test]
        public async Task Given_Idle_When_Deselect_Then_Throw()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            Assert.ThrowsAsync<InvalidOperationException>(() => cc.DeselectOperationMode());
        }

        [Test]
        public async Task Given_Completed_When_Reset_Then_Resetting()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            Assert.AreEqual(ExecutionState.IDLE, cc.EXST);
            
            await cc.StartAndWaitForExecute(SENDER);
            await cc.WaitForCompleted();

            cc.Reset(SENDER);
            Assert.AreEqual(ExecutionState.RESETTING, cc.EXST);
        }

        [Test]
        public void Given_Stopped_When_NotAllowedOperations_Then_Throw()
        {
            Assert.Throws<ExecutionException>(() => cc.Start(SENDER));

            Assert.Throws<ExecutionException>(() => cc.Suspend(SENDER));

            Assert.Throws<ExecutionException>(() => cc.Unsuspend(SENDER));

            Assert.Throws<ExecutionException>(() => cc.Stop(SENDER));

            Assert.Throws<ExecutionException>(() => cc.Hold(SENDER));

            Assert.Throws<ExecutionException>(() => cc.Unhold(SENDER));
        }

        [Test]
        public void Given_RunningOpMode_When_SelectOpMode_Then_Throw()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => cc.SelectOperationMode(OpModeTwo));
        }
    }
}