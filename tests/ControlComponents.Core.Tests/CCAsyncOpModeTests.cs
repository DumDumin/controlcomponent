using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
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
            Mock<IControlComponentProvider> provider = new Mock<IControlComponentProvider>();
            var OpModes = new Collection<IOperationMode>(){ new OperationModeAsync(OpModeOne), new OperationModeAsync(OpModeTwo) };
            var orderOutputs = new Collection<IOrderOutput>() 
            { 
                new OrderOutput("First", CC, provider.Object, new ControlComponent("CC1", OpModes, new Collection<IOrderOutput>(), new Collection<string>())),
                new OrderOutput("Second", CC, provider.Object, new ControlComponent("CC2", OpModes, new Collection<IOrderOutput>(), new Collection<string>()))
            };
            cc = new ControlComponent(CC, OpModes, orderOutputs, new Collection<string>());
            runningOpMode = cc.SelectOperationMode(OpModeOne);
        }

        [TearDown]
        public async Task TearDown()
        {
            if(cc.EXST != ExecutionState.STOPPED)
            {
                await cc.StopAndWaitForStopped(SENDER);
            }
            
            await cc.DeselectOperationMode();
            await runningOpMode;
        }

        [Test]
        public async Task Given_Execute_When_Suspend_Then_Suspended()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);

            await cc.SuspendAndWaitForSuspended(SENDER);

            Assert.AreEqual(ExecutionState.SUSPENDED, cc.EXST);
        }

        [Test]
        public async Task Given_Execute_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);

            await cc.StopAndWaitForStopped(SENDER);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Suspending_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            cc.Suspend(SENDER);

            await cc.StopAndWaitForStopped(SENDER);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Unsuspending_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            await cc.SuspendAndWaitForSuspended(SENDER);
            cc.Unsuspend(SENDER);

            await cc.StopAndWaitForStopped(SENDER);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Holding_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            cc.Hold(SENDER);

            await cc.StopAndWaitForStopped(SENDER);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Unholding_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            await cc.StartAndWaitForExecute(SENDER);
            await cc.HoldAndWaitForHeld(SENDER);
            cc.Unhold(SENDER);

            await cc.StopAndWaitForStopped(SENDER);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Starting_When_Stop_Then_Stopped()
        {
            await cc.ResetAndWaitForIdle(SENDER);
            cc.Start(SENDER);

            await cc.StopAndWaitForStopped(SENDER);

            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public async Task Given_Resetting_When_Stop_Then_Stopped()
        {
            cc.Reset(SENDER);

            await cc.StopAndWaitForStopped(SENDER);

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