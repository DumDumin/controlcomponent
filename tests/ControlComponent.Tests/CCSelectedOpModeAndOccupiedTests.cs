using System;
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

        [SetUp]
        public void Setup()
        {
            cc = new ControlComponent();
            cc.Occupy(OCCUPIER_A);
            var newOpMode = new OperationMode("NEWOPMODE");
            runningOpMode = cc.SelectOperationMode(newOpMode);
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