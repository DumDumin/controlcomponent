using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using NLog;
using System.Linq;

namespace ControlComponent.Tests
{
    public class OrderOutputTests
    {
        string ROLE = "ROLE";
        string SENDER = "SENDER";
        string CC = "CC";
        string OpModeOne = "OpModeOne";
        string OpModeTwo = "OpModeTwo";
        ControlComponent cc;

        [OneTimeSetUp]
        public void OneTimeSetUp(){
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: Console
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            logconsole.Layout = new NLog.Layouts.SimpleLayout("${longdate} ${message} ${exception:format=ToString}");
            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        [SetUp]
        public void Setup()
        {
            var OpModes = new Collection<OperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            cc = new ControlComponent(CC, OpModes);
        }

        // [TearDown]
        // public void TearDown()
        // {
        //     operationMode.OnTaskDone -= OnTaskDone;
        // }

        private IDictionary<string, OrderOutput> creatOutputs()
        {
            var OpModes = new Collection<OperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            return new Dictionary<string, OrderOutput>() {
                { OpModeOne, new OrderOutput("First", new ControlComponent("CC1", OpModes)) },
                { OpModeTwo, new OrderOutput("Second", new ControlComponent("CC2", OpModes)) }
            };
        }

        [Test]
        public void Given_OrderOutput_When_Role_Then_Role()
        {
            OrderOutput orderOutput = new OrderOutput(ROLE, cc);
            Assert.AreEqual(ROLE, orderOutput.Role);
        }

        [Test]
        public void Given_OrderOutput_When_EXST_Then_Stopped()
        {
            OrderOutput orderOutput = new OrderOutput(ROLE, cc);
            Assert.AreEqual(ExecutionState.STOPPED, orderOutput.EXST);
        }

        [Test]
        public async Task Given()
        {
            var opModeForOutput = creatOutputs();
            var output = opModeForOutput.Values.ToList();
            Task runningOpMode = cc.SelectOperationMode(OpModeOne, opModeForOutput);

            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);

            Assert.AreEqual(ExecutionState.IDLE, output[0].EXST);
            Assert.AreEqual(ExecutionState.IDLE, output[1].EXST);


            cc.Stop(SENDER);
            await Helper.WaitForState(output[0], ExecutionState.STOPPED);
            await Helper.WaitForState(output[1], ExecutionState.STOPPED);
            // Check that cc is still stopping, when the outputs get stopped
            Assert.AreEqual(ExecutionState.STOPPING, cc.EXST);
            Assert.AreEqual(ExecutionState.STOPPED, output[0].EXST);
            Assert.AreEqual(ExecutionState.STOPPED, output[1].EXST);

            await Helper.WaitForState(cc, ExecutionState.STOPPED);
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);

            await cc.DeselectOperationMode();
            await runningOpMode;
        }
    }
}