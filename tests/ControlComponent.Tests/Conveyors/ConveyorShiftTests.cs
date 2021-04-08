using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Moq;
using System.Threading;

namespace ControlComponent.Tests
{

    public class ConveyorShiftTests
    {
        ConveyorShift cc;
        Mock<IMotor> motor;
        Mock<IShiftPosition> shiftPositionMock;

        StateWaiter state;

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
            var leftStop = new Mock<ILightBarrier>();
            var leftSlow = new Mock<ILightBarrier>();
            var rightSlow = new Mock<ILightBarrier>();
            var rightStop = new Mock<ILightBarrier>();
            var beltMotor = new Mock<IMotor>();

            state = new StateWaiter();

            motor = new Mock<IMotor>();
            shiftPositionMock = new Mock<IShiftPosition>();

            cc = new ConveyorShift("ConveyorShift", motor.Object, shiftPositionMock.Object, beltMotor.Object, leftStop.Object, leftSlow.Object, rightSlow.Object, rightStop.Object);
            cc.ExecutionStateChanged += state.EventHandler;
        }

        [TearDown]
        public async Task TearDown()
        {
            if(cc.OpModeName != "NONE")
            {
                if(cc.EXST != ExecutionState.STOPPED)
                {
                    if(cc.EXST == ExecutionState.ABORTED)
                    {
                        cc.Clear(SENDER);
                    }
                    else 
                    {
                        cc.Stop(SENDER);
                    }
                }
                
                await Helper.WaitForState(cc, ExecutionState.STOPPED);
                await cc.DeselectOperationMode();
                Assert.DoesNotThrowAsync(() => runningOpMode);
                motor.VerifySet(m => m.Speed = 0);
            }

            cc.ExecutionStateChanged -= state.EventHandler;
        }



        [Test]
        public void Given_Position_When_RequestPosition_Then_Return_Position()
        {
            shiftPositionMock.SetupGet(m => m.Position).Returns(0);
            Assert.AreEqual(0, cc.Position);
        }

#region PositionZero

        [Test]
        public async Task Given_PositionZero_When_SelectDSHIFT_Then_MotorWasUsed()
        {
            shiftPositionMock.SetupGet(m => m.Position).Returns(0);

            runningOpMode = cc.SelectOperationMode("DSHIFT");

            cc.Reset(SENDER);
            await state.Idle();
            cc.Start(SENDER);
            await state.Execute();

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = 1);
            shiftPositionMock.SetupGet(m => m.Position).Returns(1);
            shiftPositionMock.Raise(s => s.PositionChanged += null, EventArgs.Empty);

            await state.Completed();
        }

        [Test]
        public async Task Given_PositionZeroNoPositionSensor_When_SelectDSHIFT_Then_Aborted()
        {
            shiftPositionMock.SetupGet(m => m.Position).Returns(0);

            runningOpMode = cc.SelectOperationMode("DSHIFT");

            cc.Reset(SENDER);
            await state.Idle();
            cc.Start(SENDER);
            await state.Execute();

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = 1);

            await state.Aborted();
        }

        [Test]
        public async Task Given_PositionZero_When_SelectUSHIFT_Then_DoNotUseMotor()
        {
            shiftPositionMock.SetupGet(m => m.Position).Returns(0);

            runningOpMode = cc.SelectOperationMode("USHIFT");

            cc.Reset(SENDER);
            await state.Idle();
            cc.Start(SENDER);
            await state.Execute();

            await Task.Delay(1);
            motor.VerifyNoOtherCalls();

            await Helper.WaitForState(cc, ExecutionState.COMPLETED);
        }

#endregion
#region PositionLast
        [Test]
        public async Task Given_PositionLast_When_SelectUSHIFT_Then_MotorWasUsed()
        {
            shiftPositionMock.SetupGet(m => m.Position).Returns(2);
            shiftPositionMock.SetupGet(m => m.Positions).Returns(new List<float>(){0,1,2});

            runningOpMode = cc.SelectOperationMode("USHIFT");

            cc.Reset(SENDER);
            await state.Idle();
            cc.Start(SENDER);
            await state.Execute();

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = -1);
            shiftPositionMock.SetupGet(m => m.Position).Returns(1);
            shiftPositionMock.Raise(s => s.PositionChanged += null, EventArgs.Empty);

            await state.Completed();
        }

        [Test]
        public async Task Given_PositionLastNoPositionSensor_When_SelectUSHIFT_Then_Aborted()
        {
            shiftPositionMock.SetupGet(m => m.Position).Returns(2);
            shiftPositionMock.SetupGet(m => m.Positions).Returns(new List<float>(){0,1,2});

            runningOpMode = cc.SelectOperationMode("USHIFT");

            cc.Reset(SENDER);
            await state.Idle();
            cc.Start(SENDER);
            await state.Execute();

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = -1);

            await state.Aborted();
        }

        [Test]
        public async Task Given_PositionLast_When_SelectDSHIFT_Then_DoNotUseMotor()
        {
            shiftPositionMock.SetupGet(m => m.Position).Returns(2);
            shiftPositionMock.SetupGet(m => m.Positions).Returns(new List<float>(){0,1,2});

            runningOpMode = cc.SelectOperationMode("DSHIFT");

            cc.Reset(SENDER);
            await state.Idle();
            cc.Start(SENDER);
            await state.Execute();

            await Task.Delay(1);
            motor.VerifyNoOtherCalls();

            await state.Completed();
        }
#endregion
#region PositionOne
        [Test]
        public async Task Given_PositionOne_When_SelectUSHIFT_Then_MotorWasUsed()
        {
            shiftPositionMock.SetupGet(m => m.Position).Returns(1);
            shiftPositionMock.SetupGet(m => m.Positions).Returns(new List<float>(){0,1,2});

            runningOpMode = cc.SelectOperationMode("USHIFT");

            cc.Reset(SENDER);
            await state.Idle();
            cc.Start(SENDER);
            await state.Execute();

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = -1);
            shiftPositionMock.SetupGet(m => m.Position).Returns(0);
            shiftPositionMock.Raise(s => s.PositionChanged += null, EventArgs.Empty);

            await state.Completed();
        }
        [Test]
        public async Task Given_PositionOne_When_SelectDSHIFT_Then_MotorWasUsed()
        {
            shiftPositionMock.SetupGet(m => m.Position).Returns(1);
            shiftPositionMock.SetupGet(m => m.Positions).Returns(new List<float>(){0,1,2});

            runningOpMode = cc.SelectOperationMode("DSHIFT");

            cc.Reset(SENDER);
            await state.Idle();
            cc.Start(SENDER);
            await state.Execute();

            await Task.Delay(1);
            motor.VerifySet(m => m.Speed = 1);
            motor.VerifySet(m => m.Direction = 1);
            shiftPositionMock.SetupGet(m => m.Position).Returns(2);
            shiftPositionMock.Raise(s => s.PositionChanged += null, EventArgs.Empty);

            await state.Completed();
        }
#endregion
    }
}