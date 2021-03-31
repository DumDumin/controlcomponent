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
        Collection<OrderOutput> orderOutputs;

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
            var CascadeOpModes = new Collection<IOperationMode>(){ new OperationModeCascade(OpModeOne), new OperationModeCascade(OpModeTwo) };
            var OpModes = new Collection<IOperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            orderOutputs = new Collection<OrderOutput>() 
            { 
                new OrderOutput("ROLE_ONE", new ControlComponent("CC1", OpModes, new Collection<OrderOutput>())),
                new OrderOutput("ROLE_TWO", new ControlComponent("CC2", OpModes, new Collection<OrderOutput>()))
            };
            cc = new ControlComponent(CC, CascadeOpModes, orderOutputs);
        }

        // [TearDown]
        // public void TearDown()
        // {
        //     operationMode.OnTaskDone -= OnTaskDone;
        // }


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
        public void Given_OrderOutputs_When_Roles_Then_Roles()
        {
            Assert.AreEqual(new Collection<string>(){"ROLE_ONE", "ROLE_TWO"}, cc.Roles);
        }

        [Test]
        public async Task Given_OrderOutputs_When_Reset_Then_Idle()
        {
            Task runningOpMode = cc.SelectOperationMode(OpModeOne);

            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);

            Assert.AreEqual(ExecutionState.IDLE, orderOutputs[0].EXST);
            Assert.AreEqual(ExecutionState.IDLE, orderOutputs[1].EXST);


            cc.Stop(SENDER);
            await Helper.WaitForState(orderOutputs[0], ExecutionState.STOPPED);
            await Helper.WaitForState(orderOutputs[1], ExecutionState.STOPPED);
            // Check that cc is still stopping, when the outputs get stopped
            Assert.AreEqual(ExecutionState.STOPPING, cc.EXST);
            Assert.AreEqual(ExecutionState.STOPPED, orderOutputs[0].EXST);
            Assert.AreEqual(ExecutionState.STOPPED, orderOutputs[1].EXST);

            await Helper.WaitForState(cc, ExecutionState.STOPPED);
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);

            await cc.DeselectOperationMode();
            await runningOpMode;

            Assert.AreEqual("NONE", orderOutputs[0].OpModeName);
            Assert.AreEqual("NONE", orderOutputs[1].OpModeName);
        }
    }
}