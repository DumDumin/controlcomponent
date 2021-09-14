using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
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
            Mock<IControlComponentProvider> provider = new Mock<IControlComponentProvider>();
            var OpModes = new Collection<IOperationMode>(){ new OperationModeRaw(OpModeOne), new OperationModeRaw(OpModeTwo) };
            var orderOutputs = new Collection<IOrderOutput>() 
            { 
                new OrderOutput("First", CC, provider.Object, new ControlComponent("CC1", OpModes, new Collection<IOrderOutput>(), new Collection<string>())),
                new OrderOutput("Second", CC, provider.Object, new ControlComponent("CC2", OpModes, new Collection<IOrderOutput>(), new Collection<string>()))
            };
            cc = new ControlComponent(CC, OpModes, orderOutputs, new Collection<string>());
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
            
            await cc.StopAndWaitForStopped(SENDER);

            await cc.DeselectOperationMode();
            await runningOpMode;
            cc.Free(OCCUPIER_A);
        }

        [Test]
        public void Given_Stopped_When_Reset_Then_Throw()
        {
            InvalidOperationException e = Assert.Throws<InvalidOperationException>(() => cc.Reset(OCCUPIER_B));
            Assert.AreEqual($"{OCCUPIER_B} cannot change {CC} to {ExecutionState.RESETTING}, while occupied by {OCCUPIER_A}.", e.Message);
        }
    }
}