using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
            var OpModes = new Collection<IOperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            var orderOutputs = new Collection<OrderOutput>() 
            { 
                new OrderOutput("First", new ControlComponent("CC1", OpModes, new Collection<OrderOutput>(), new Collection<string>())),
                new OrderOutput("Second", new ControlComponent("CC2", OpModes, new Collection<OrderOutput>(), new Collection<string>()))
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
            
            await cc.StopAndWaitForStopped(SENDER, false);
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