using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class CCSelectedOpModeAndOccupiedTests
    {
        ControlComponent cc;
        Task runningOpMode;
        string OCCUPIER_A = "A";
        string OCCUPIER_B = "B";
        string SENDER = "SENDER";
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
            cc.Occupy(OCCUPIER_A);
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
            cc.Free(OCCUPIER_A);
        }

        [Test]
        public void Given_Stopped_When_Reset_Then_Throw()
        {
            InvalidOperationException e = Assert.Throws<InvalidOperationException>(() => cc.Reset(OCCUPIER_B));
            Assert.AreEqual($"{OCCUPIER_B} cannot change to {ExecutionState.RESETTING}, while {OCCUPIER_A} occupies cc.", e.Message);
        }
    }
}