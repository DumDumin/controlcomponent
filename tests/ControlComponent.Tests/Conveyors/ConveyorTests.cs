using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Moq;

namespace ControlComponent.Tests
{
    public class ConveyorTests
    {
        Conveyor cc;

        Mock<ILightBarrier> leftStop;
        Mock<ILightBarrier> leftSlow;
        Mock<ILightBarrier> rightSlow;
        Mock<ILightBarrier> rightStop;
        Mock<IMotor> motor;

        Task runningOpMode;

        string SENDER = "SENDER";

        [OneTimeSetUp]
        public void OneTimeSetUp(){
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: Console
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");   
            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);     
            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        [SetUp]
        public void Setup()
        {
            leftStop = new Mock<ILightBarrier>();
            leftSlow = new Mock<ILightBarrier>();
            rightSlow = new Mock<ILightBarrier>();
            rightStop = new Mock<ILightBarrier>();
            motor = new Mock<IMotor>();

            cc = new Conveyor("Conveyor", new Collection<IOperationMode>(),  motor.Object, leftStop.Object, leftSlow.Object, rightSlow.Object, rightStop.Object);
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
            Assert.DoesNotThrowAsync(() => runningOpMode);
            motor.VerifySet(m => m.Speed = 0);
        }

        [Test]
        public async Task Given_Conveyor_When_SelectFPASS_Then_DoNotThrow()
        {
            //  ___ 
            // |___| --> forward pass
            rightSlow.SetupGet(l => l.Occupied).Returns(true);
            rightStop.SetupGet(l => l.Occupied).Returns(true);
            runningOpMode = cc.SelectOperationMode("FPASS");

            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);
            cc.Start(SENDER);
            await Helper.WaitForState(cc, ExecutionState.EXECUTE);

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = 1);

            rightSlow.SetupGet(l => l.Occupied).Returns(false);
            rightSlow.Raise(l => l.Hit += null, EventArgs.Empty);
            await Task.Delay(1);
            rightStop.SetupGet(l => l.Occupied).Returns(false);
            rightStop.Raise(l => l.Hit += null, EventArgs.Empty);

            await Helper.WaitForState(cc, ExecutionState.COMPLETED);
        }

        [Test]
        public async Task Given_Conveyor_When_SelectFTAKE_Then_DoNotThrow()
        {
            //  ___ 
            // |___| --> forward take
            rightSlow.SetupGet(l => l.Occupied).Returns(false);
            rightStop.SetupGet(l => l.Occupied).Returns(false);
            runningOpMode = cc.SelectOperationMode("FTAKE");

            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);
            cc.Start(SENDER);
            await Helper.WaitForState(cc, ExecutionState.EXECUTE);

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = 1);

            rightSlow.SetupGet(l => l.Occupied).Returns(true);
            rightSlow.Raise(l => l.Hit += null, EventArgs.Empty);
            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 0.5f);
            rightStop.SetupGet(l => l.Occupied).Returns(true);
            rightStop.Raise(l => l.Hit += null, EventArgs.Empty);

            await Helper.WaitForState(cc, ExecutionState.COMPLETED);
        }

        [Test]
        public async Task Given_Conveyor_When_SelectBPASS_Then_DoNotThrow()
        {
            leftStop.SetupGet(l => l.Occupied).Returns(true);
            leftSlow.SetupGet(l => l.Occupied).Returns(true);
            //  ___ 
            // |___| <-- backward pass
            runningOpMode = cc.SelectOperationMode("BPASS");

            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);
            cc.Start(SENDER);
            await Helper.WaitForState(cc, ExecutionState.EXECUTE);

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = -1);

            leftSlow.SetupGet(l => l.Occupied).Returns(false);
            leftSlow.Raise(l => l.Hit += null, EventArgs.Empty);
            await Task.Delay(1);
            leftStop.SetupGet(l => l.Occupied).Returns(false);
            leftStop.Raise(l => l.Hit += null, EventArgs.Empty);

            await Helper.WaitForState(cc, ExecutionState.COMPLETED);
        }

        [Test]
        public async Task Given_Conveyor_When_SelectBTAKE_Then_DoNotThrow()
        {
            //  ___ 
            // |___| <-- backward take
            leftStop.SetupGet(l => l.Occupied).Returns(false);
            leftSlow.SetupGet(l => l.Occupied).Returns(false);
            runningOpMode = cc.SelectOperationMode("BTAKE");

            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);
            cc.Start(SENDER);
            await Helper.WaitForState(cc, ExecutionState.EXECUTE);

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = -1);

            leftSlow.SetupGet(l => l.Occupied).Returns(true);
            leftSlow.Raise(l => l.Hit += null, EventArgs.Empty);
            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 0.5f);
            leftStop.SetupGet(l => l.Occupied).Returns(true);
            leftStop.Raise(l => l.Hit += null, EventArgs.Empty);

            await Helper.WaitForState(cc, ExecutionState.COMPLETED);
        }

        [Test]
        public async Task Given_ConveyorWithPalette()
        {
            
        }

        [Test]
        public async Task Given_Execute_When_Hold_Then_StopMotor()
        {
            //  ___ 
            // |___| <-- backward take
            runningOpMode = cc.SelectOperationMode("BTAKE");

            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);
            cc.Start(SENDER);
            await Helper.WaitForState(cc, ExecutionState.EXECUTE);

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = -1);

            cc.Hold(SENDER);
            await Helper.WaitForState(cc, ExecutionState.HELD);
            motor.VerifySet(m => m.Speed = 0);
            // motor.VerifyNoOtherCalls();
            cc.Unhold(SENDER);

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = -1);

            leftSlow.Raise(l => l.Hit += null, EventArgs.Empty);
            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 0.5f);
            leftStop.Raise(l => l.Hit += null, EventArgs.Empty);

            await Helper.WaitForState(cc, ExecutionState.COMPLETED);
        }


    }
}