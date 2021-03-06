using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;

using AutoFixture.NUnit3;
using FluentAssertions;

namespace ControlComponents.Core.Tests
{
    public class IControlComponentExtensionsTests
    {
        string SENDER = "SENDER";
        ControlComponent cc;
        Task runningOpMode;

        
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
            var OpModes = new Collection<IOperationMode>(){ new OperationModeAsync("OpModeOne"), new OperationModeAsync("OpModeTwo") };
            cc = new ControlComponent("CC", OpModes, new Collection<IOrderOutput>() , new Collection<string>());
        }

        [TearDown]
        public async Task TearDown()
        {
            if(cc.OpModeName != "NONE")
            {
                if(cc.EXST != ExecutionState.STOPPED)
                {
                    await cc.StopAndWaitForStopped(SENDER);
                    cc.Free(SENDER);
                }
                await cc.DeselectOperationMode();
                await runningOpMode;
            }
        }

        [Test]
        public async Task Given_Stopping_When_Stop_Then_Stopped()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            cc.Stop(SENDER);
            
            // When
            await cc.StopAndWaitForStopped(SENDER);

            // Then
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Aborting_When_StopAndWaitForStopped_Then_Stopped()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            cc.Abort(SENDER);

            // When
            await cc.StopAndWaitForStopped(SENDER);

            // Then
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Clearing_When_StopAndWaitForStopped_Then_Stopped()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.AbortAndWaitForAborted(SENDER);
            cc.Clear(SENDER);

            // When
            await cc.StopAndWaitForStopped(SENDER);

            // Then
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Idle_When_Start_Then_Execute()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            
            // When
            await cc.StartAndWaitForExecute(SENDER);

            // Then
            Assert.AreEqual(ExecutionState.EXECUTE, cc.EXST);
        }

        [Test]
        public async Task Given_Idle_When_Stop_Then_Stopped()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            
            // When
            await cc.StopAndWaitForStopped(SENDER);

            // Then
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_StoppedAndNoOpModeSelected_When_Stop_Then_Stopped()
        {
            // Given
            
            // When
            await cc.StopAndWaitForStopped(SENDER);

            // Then
            Assert.AreEqual("NONE", cc.OCCUPIER);
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Idle_When_Abort_Then_Aborted()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            
            // When
            await cc.AbortAndWaitForAborted(SENDER);

            // Then
            Assert.AreEqual(ExecutionState.ABORTED, cc.EXST);
        }

        [Test]
        public async Task Given_Execute_When_SuspendAndWaitForSuspended_Then_Suspended()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            
            // When
            await cc.SuspendAndWaitForSuspended(SENDER);

            // Then
            Assert.AreEqual(ExecutionState.SUSPENDED, cc.EXST);
        }

        [Test]
        public async Task Given_Suspended_When_UnsuspendAndWaitForExecute_Then_Execute()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            await cc.SuspendAndWaitForSuspended(SENDER);

            // When
            await cc.UnsuspendAndWaitForExecute(SENDER);

            // Then
            Assert.AreEqual(ExecutionState.EXECUTE, cc.EXST);
        }

        [Test]
        public async Task Given_Execute_When_HoldAndWaitForHeld_Then_Held()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            
            // When
            await cc.HoldAndWaitForHeld(SENDER);

            // Then
            Assert.AreEqual(ExecutionState.HELD, cc.EXST);
        }

        [Test]
        public async Task Given_Execute_When_WaitForCompleted_Then_Completed()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            
            // When
            await cc.WaitForCompleted(100);

            // Then
            Assert.AreEqual(ExecutionState.COMPLETED, cc.EXST);
        }

        [Test]
        public async Task Given_Idle_When_WaitForCompleted_Then_Timeout()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            
            // When - Then
            Assert.ThrowsAsync(typeof(Exception), () => cc.WaitForCompleted(100));
        }

        [Test]
        public async Task Given_Aborted_When_AbortAndWaitForAborted_Then_Aborted()
        {
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.AbortAndWaitForAborted(SENDER);

            await cc.AbortAndWaitForAborted(SENDER);
            cc.EXST.Should().Be(ExecutionState.ABORTED);
        }

        [Test]
        public async Task Given_Aborting_When_AbortAndWaitForAborted_Then_Aborted()
        {
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            cc.Abort(SENDER);

            await cc.AbortAndWaitForAborted(SENDER);
            cc.EXST.Should().Be(ExecutionState.ABORTED);
        }

        [Test]
        public async Task Given_OccupiedAndIdle_When_StopPrioByOtherComponent_Then_StoppedAndOccupiedByOtherCompnent()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            
            // When
            await cc.StopPrioAndWaitForStopped("OTHER");

            // Then
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
            Assert.AreEqual("OTHER", cc.OCCUPIER);
        }
    }
}