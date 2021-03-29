using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using NLog;

namespace ControlComponent.Tests
{
    public class OrderOutputTests
    {
        string ROLE = "ROLE";
        string SENDER = "SENDER";
        string CC = "CC";

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

        // [SetUp]
        // public void Setup()
        // {
        //     operationMode = new OperationMode("DEFAULT");
        //     tokenOwner = new CancellationTokenSource();

        //     called = false;
        //     operationMode.OnTaskDone += OnTaskDone;
        // }

        // [TearDown]
        // public void TearDown()
        // {
        //     operationMode.OnTaskDone -= OnTaskDone;
        // }

        private Collection<OrderOutput> creatOutputs()
        {
            return new Collection<OrderOutput>() {
                new OrderOutput("First", new ControlComponent("CC1")),
                new OrderOutput("Second", new ControlComponent("CC2"))
            };
        }

        [Test]
        public void Given_OrderOutput_When_Role_Then_Role()
        {
            OrderOutput orderOutput = new OrderOutput(ROLE, new ControlComponent(CC));
            Assert.AreEqual(ROLE, orderOutput.Role);
        }

        [Test]
        public void Given_OrderOutput_When_EXST_Then_Stopped()
        {
            OrderOutput orderOutput = new OrderOutput(ROLE, new ControlComponent(CC));
            Assert.AreEqual(ExecutionState.STOPPED, orderOutput.EXST);
        }

        [Test]
        public async Task Given()
        {
            ControlComponent cc = new ControlComponent(CC);
            var newOpMode = new OperationMode("NEWOPMODE");


            var outputs = creatOutputs();
            Task runningOpMode = cc.SelectOperationMode(newOpMode, outputs);

            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);

            Assert.AreEqual(ExecutionState.IDLE, outputs[0].EXST);
            Assert.AreEqual(ExecutionState.IDLE, outputs[1].EXST);


            cc.Stop(SENDER);
            await Helper.WaitForState(outputs[0], ExecutionState.STOPPED);
            await Helper.WaitForState(outputs[1], ExecutionState.STOPPED);
            // Check that cc is still stopping, when the outputs get stopped
            Assert.AreEqual(ExecutionState.STOPPING, cc.EXST);
            Assert.AreEqual(ExecutionState.STOPPED, outputs[0].EXST);
            Assert.AreEqual(ExecutionState.STOPPED, outputs[1].EXST);

            await Helper.WaitForState(cc, ExecutionState.STOPPED);
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);

            await cc.DeselectOperationMode();
            await runningOpMode;
        }
    }
}