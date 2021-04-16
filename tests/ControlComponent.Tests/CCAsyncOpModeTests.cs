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
    public class CCAsyncOpModeTests
    {
        string SENDER = "SENDER";
        ControlComponent cc;
        Task runningOpMode;
        string CC = "CC";
        string OpModeOne = "OpModeOne";
        string OpModeTwo = "OpModeTwo";

        
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
            var OpModes = new Collection<IOperationMode>(){ new OperationModeAsync(OpModeOne), new OperationModeAsync(OpModeTwo) };
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
                await cc.StopAndWaitForStopped(SENDER, false);
            }
            
            await cc.DeselectOperationMode();
            await runningOpMode;
        }

        [Test]
        public async Task Given_Execute_When_Suspend_Then_Suspended()
        {
            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);
            cc.Start(SENDER);
            await Helper.WaitForState(cc, ExecutionState.EXECUTE);

            cc.Suspend(SENDER);
            await Helper.WaitForState(cc, ExecutionState.SUSPENDED);

            Assert.AreEqual(ExecutionState.SUSPENDED, cc.EXST);
        }

        [Test]
        public async Task Given_Execute_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);

            await cc.StopAndWaitForStopped(SENDER, false);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Suspending_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            cc.Suspend(SENDER);

            await cc.StopAndWaitForStopped(SENDER, false);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Unsuspending_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            await cc.SuspendAndWaitForSuspended(SENDER);
            cc.Unsuspend(SENDER);

            await cc.StopAndWaitForStopped(SENDER, false);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Holding_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            cc.Hold(SENDER);

            await cc.StopAndWaitForStopped(SENDER, false);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Unholding_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            await cc.HoldAndWaitForHeld(SENDER);
            cc.Unhold(SENDER);

            await cc.StopAndWaitForStopped(SENDER, false);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Starting_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            cc.Start(SENDER);

            await cc.StopAndWaitForStopped(SENDER, false);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Resetting_When_Stop_Then_Stopped()
        {
            cc.Reset(SENDER);

            await cc.StopAndWaitForStopped(SENDER, false);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Stopping_When_Abort_Then_Aborted()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            cc.Stop(SENDER);

            await cc.AbortAndWaitForAborted(SENDER);

            Assert.AreEqual(ExecutionState.ABORTED, cc.EXST);
        }

        [Test]
        public async Task Given_Clearing_When_Abort_Then_Aborted()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.AbortAndWaitForAborted(SENDER);
            cc.Clear(SENDER);

            await cc.AbortAndWaitForAborted(SENDER);

            Assert.AreEqual(ExecutionState.ABORTED, cc.EXST);
        }
    }
}