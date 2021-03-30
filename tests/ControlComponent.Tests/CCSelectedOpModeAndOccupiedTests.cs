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

        [SetUp]
        public void Setup()
        {
            var OpModes = new Collection<OperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            var orderOutputs = new Collection<OrderOutput>() 
            { 
                new OrderOutput("First", new ControlComponent("CC1", OpModes, new Collection<OrderOutput>())),
                new OrderOutput("Second", new ControlComponent("CC2", OpModes, new Collection<OrderOutput>()))
            };
            cc = new ControlComponent(CC, OpModes, orderOutputs);
            cc.Occupy(OCCUPIER_A);
            runningOpMode = cc.SelectOperationMode(OpModeOne);
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