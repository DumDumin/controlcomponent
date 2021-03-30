using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class ControlComponentSelectedOpModeTests
    {
        string SENDER = "SENDER";
        ControlComponent cc;
        Task runningOpMode;
        string CC = "CC";
        string OpModeOne = "OpModeOne";
        string OpModeTwo = "OpModeTwo";

        private IDictionary<string, OrderOutput> creatOutputs()
        {
            var OpModes = new Collection<OperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            return new Dictionary<string, OrderOutput>() {
                { OpModeOne, new OrderOutput("First", new ControlComponent("CC1", OpModes)) },
                { OpModeTwo, new OrderOutput("Second", new ControlComponent("CC2", OpModes)) }
            };
        }

        [SetUp]
        public void Setup()
        {
            var OpModes = new Collection<OperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            cc = new ControlComponent(CC, OpModes);
            runningOpMode = cc.SelectOperationMode(OpModeOne, creatOutputs());
        }

        [TearDown]
        public async Task TearDown()
        {
            if(cc.EXST != ExecutionState.STOPPED)
            {
                cc.Stop(SENDER);
            }
            
            await Helper.WaitForState(cc, ExecutionState.STOPPED);
            await cc.DeselectOperationMode();
            await runningOpMode;
        }

        [Test]
        public async Task Given_Stopped_When_Reset_Then_Idle()
        {
            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);

            Assert.AreEqual(ExecutionState.IDLE, cc.EXST);
        }

        [Test]
        public async Task Given_Stopped_When_NormalRun_Then_Completed()
        {
            cc.Reset(SENDER);

            await Helper.WaitForState(cc, ExecutionState.IDLE);
            Assert.AreEqual(ExecutionState.IDLE, cc.EXST);

            cc.Start(SENDER);
            await Helper.WaitForState(cc, ExecutionState.COMPLETED);
            Assert.AreEqual(ExecutionState.COMPLETED, cc.EXST);
        }

        [Test]
        public async Task Given_Idle_When_Deselect_Then_Throw()
        {
            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);

            Assert.ThrowsAsync<InvalidOperationException>(() => cc.DeselectOperationMode());
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
    }
}