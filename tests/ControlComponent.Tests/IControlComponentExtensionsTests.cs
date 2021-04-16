using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;

namespace ControlComponent.Tests
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
            cc = new ControlComponent("CC", OpModes, new Collection<OrderOutput>() , new Collection<string>());
        }

        [TearDown]
        public async Task TearDown()
        {
            if(cc.OpModeName != "NONE")
            {
                if(cc.EXST != ExecutionState.STOPPED)
                {
                    await cc.StopAndWaitForStopped(SENDER, true);
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
            await cc.StopAndWaitForStopped(SENDER, false);

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
        public async Task Given_Idle_When_StopAndNotFree_Then_StoppedAndOccupied()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            
            // When
            await cc.StopAndWaitForStopped(SENDER, false);

            // Then
            Assert.AreEqual(SENDER, cc.OCCUPIER);
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Idle_When_StopAndFree_Then_StoppedAndFree()
        {
            // Given
            runningOpMode = cc.SelectOperationMode("OpModeOne");
            await cc.ResetAndWaitForIdle(SENDER);
            
            // When
            await cc.StopAndWaitForStopped(SENDER, true);

            // Then
            Assert.AreEqual("NONE", cc.OCCUPIER);
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_StoppedAndNoOpModeSelected_When_StopAndFree_Then_StoppedAndFree()
        {
            // Given
            
            // When
            await cc.StopAndWaitForStopped(SENDER, true);

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
    }
}