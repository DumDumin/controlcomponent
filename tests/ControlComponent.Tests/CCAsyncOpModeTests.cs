using System;
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
            cc = new ControlComponent();
            var newOpMode = new OperationModeAsync("NEWOPMODE");
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
        }

        [Test]
        public async Task Given_ExecuteControlComponent_When_Suspend_Then_Suspended()
        {
            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);
            cc.Start(SENDER);
            await Helper.WaitForState(cc, ExecutionState.EXECUTE);
            cc.Suspend(SENDER);
            await Helper.WaitForState(cc, ExecutionState.SUSPENDED);
            
            Assert.AreEqual(ExecutionState.SUSPENDED, cc.EXST);
        }
    }
}