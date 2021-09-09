using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using NLog;
using System.Linq;

namespace ControlComponents.Core.Tests
{
    public class OrderOutputTests
    {
        string ROLE = "ROLE";
        string SENDER = "SENDER";
        string CC = "CC";
        string OpModeOne = "OpModeOne";
        string OpModeTwo = "OpModeTwo";
        ControlComponent cc;
        Collection<IOrderOutput> orderOutputs;

        Mock<IControlComponentProvider> provider;

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
            provider = new Mock<IControlComponentProvider>();
            var CascadeOpModes = new Collection<IOperationMode>(){ new OperationModeCascade(OpModeOne), new OperationModeCascade(OpModeTwo) };
            var OpModes = new Collection<IOperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            orderOutputs = new Collection<IOrderOutput>() 
            { 
                new OrderOutput("ROLE_ONE", CC, provider.Object, new ControlComponent("CC1", OpModes, new Collection<IOrderOutput>(), new Collection<string>())),
                new OrderOutput("ROLE_TWO", CC, provider.Object, new ControlComponent("CC2", OpModes, new Collection<IOrderOutput>(), new Collection<string>()))
            };
            cc = new ControlComponent(CC, CascadeOpModes, orderOutputs, new Collection<string>());
        }

        // [TearDown]
        // public void TearDown()
        // {
        //     operationMode.OnTaskDone -= OnTaskDone;
        // }

        [Test]
        public void Given_EqualOrderOutput_When_Equals_Then_ReturnTrue()
        {
            var first = new OrderOutput("ROLE_ONE", "Output", provider.Object, new ControlComponent("CC1", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>()));
            var second = new OrderOutput("ROLE_TWO", "Output", provider.Object, new ControlComponent("CC1", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>()));

            Assert.AreEqual(first,second);
        }
        
        [Test]
        public void Given_NotEqualOrderOutput_When_Equals_Then_ReturnFalse()
        {
            var first = new OrderOutput("ROLE_ONE", "Output", provider.Object, new ControlComponent("CC1", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>()));
            var second = new OrderOutput("ROLE_TWO", "Output", provider.Object, new ControlComponent("CC2", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>()));

            Assert.AreNotEqual(first,second);
        }

        [Test]
        public void Given_OpModes_When_ListOpModeNames_Then_ReturnOpModeNames()
        {
            Assert.AreEqual(new Collection<string>(){OpModeOne, OpModeTwo}, orderOutputs[0].OpModes);
        }

        [Test]
        public void Given_OrderOutput_When_Role_Then_Role()
        {
            OrderOutput orderOutput = new OrderOutput(ROLE, "Output", provider.Object, cc);
            Assert.AreEqual(ROLE, orderOutput.Role);
        }

        [Test]
        public void Given_OrderOutput_When_EXST_Then_Stopped()
        {
            OrderOutput orderOutput = new OrderOutput(ROLE, "Output", provider.Object, cc);
            Assert.AreEqual(ExecutionState.STOPPED, orderOutput.EXST);
        }

        [Test]
        public void Given_OrderOutputs_When_Roles_Then_Roles()
        {
            Assert.AreEqual(new Collection<string>(){"ROLE_ONE", "ROLE_TWO"}, cc.Roles);
        }

        [Test]
        public void Given_NotAllNeededRoles_When_Create_Then_Throw()
        {
            var CascadeOpModes = new Collection<IOperationMode>(){ new OperationModeCascade(OpModeOne), new OperationModeCascade(OpModeTwo) };
            var OpModes = new Collection<IOperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            orderOutputs = new Collection<IOrderOutput>() 
            { 
                new OrderOutput("ROLE_ONE", CC, provider.Object, new ControlComponent("CC1", OpModes, new Collection<IOrderOutput>(), new Collection<string>())),
                new OrderOutput("ROLE_TWO", CC, provider.Object, new ControlComponent("CC2", OpModes, new Collection<IOrderOutput>(), new Collection<string>()))
            };
            Collection<string> neededRoles = new Collection<string>() { "ROLE_THREE" };

            Assert.Throws<ArgumentException>(() => new ControlComponent(CC, CascadeOpModes, orderOutputs, neededRoles));
        }

        [Test]
        public void Given_NeededRoles_When_Create_Then_DoNotThrow()
        {
            var CascadeOpModes = new Collection<IOperationMode>(){ new OperationModeCascade(OpModeOne), new OperationModeCascade(OpModeTwo) };
            var OpModes = new Collection<IOperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            orderOutputs = new Collection<IOrderOutput>() 
            { 
                new OrderOutput("ROLE_ONE", CC, provider.Object, new ControlComponent("CC1", OpModes, new Collection<IOrderOutput>(), new Collection<string>())),
                new OrderOutput("ROLE_TWO", CC, provider.Object, new ControlComponent("CC2", OpModes, new Collection<IOrderOutput>(), new Collection<string>()))
            };
            Collection<string> neededRoles = new Collection<string>() { "ROLE_ONE", "ROLE_TWO" };

            Assert.DoesNotThrow(() => new ControlComponent(CC, CascadeOpModes, orderOutputs, neededRoles));
        }

        [Test]
        public async Task Given_OrderOutputs_When_Reset_Then_Idle()
        {
            Task runningOpMode = cc.SelectOperationMode(OpModeOne);

            cc.Reset(SENDER);
            await Helper.WaitForState(orderOutputs[0], ExecutionState.IDLE);
            await Helper.WaitForState(orderOutputs[1], ExecutionState.IDLE);
            // Assert.AreEqual(ExecutionState.RESETTING, cc.EXST);

            Assert.AreEqual(ExecutionState.IDLE, orderOutputs[0].EXST);
            Assert.AreEqual(ExecutionState.IDLE, orderOutputs[1].EXST);
            await Helper.WaitForState(cc, ExecutionState.IDLE);


            cc.Stop(SENDER);
            await Helper.WaitForState(orderOutputs[0], ExecutionState.STOPPED);
            await Helper.WaitForState(orderOutputs[1], ExecutionState.STOPPED);
            // Check that cc is still stopping, when the outputs get stopped
            // Assert.AreEqual(ExecutionState.STOPPING, cc.EXST);
            Assert.AreEqual(ExecutionState.STOPPED, orderOutputs[0].EXST);
            Assert.AreEqual(ExecutionState.STOPPED, orderOutputs[1].EXST);

            await Helper.WaitForState(cc, ExecutionState.STOPPED);
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);

            await cc.DeselectOperationMode();
            await runningOpMode;

            Assert.AreEqual("NONE", orderOutputs[0].OpModeName);
            Assert.AreEqual("NONE", orderOutputs[1].OpModeName);
        }

        // TODO this tests seems to be not reliable
        [Test]
        public async Task Given_Idle_When_Start_Then_Completed()
        {
            // Given
            Task runningOpMode = cc.SelectOperationMode(OpModeOne);
            cc.Reset(SENDER);
            await Helper.WaitForState(cc, ExecutionState.IDLE);

            // When
            cc.Start(SENDER);

            // Then
            await Helper.WaitForState(orderOutputs[0], ExecutionState.COMPLETED);
            await Helper.WaitForState(orderOutputs[1], ExecutionState.COMPLETED);
            // Check that cc is still stopping, when the outputs get stopped

            // TODO this assert does not work in a fast paced async environment -> use ExecutionStateChangedEvent as in OperationModeBase
            // Assert.AreEqual(ExecutionState.COMPLETING, cc.EXST);
            Assert.AreEqual(ExecutionState.COMPLETED, orderOutputs[0].EXST);
            Assert.AreEqual(ExecutionState.COMPLETED, orderOutputs[1].EXST);

            await Helper.WaitForState(cc, ExecutionState.COMPLETED);
            Assert.AreEqual(ExecutionState.COMPLETED, cc.EXST);

            // Clean Up
            cc.Stop(SENDER);
            await Helper.WaitForState(cc, ExecutionState.STOPPED);
            await cc.DeselectOperationMode();
            await runningOpMode;
        }

        [Test]
        public void Given_Stopped_When_Change_CC_Then_Changed()
        {
            ControlComponent c1 = new ControlComponent("C1", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            ControlComponent c2 = new ControlComponent("C2", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            OrderOutput output = new OrderOutput("Test", "Output", provider.Object, c1);

            bool success = output.ChangeComponent(c2);

            Assert.True(success);
            Assert.AreEqual(c2.ComponentName, output.ComponentName);
        }

        [Test]
        public async Task Given_Idle_When_Change_CC_Then_NotChanged()
        {
            ControlComponent c1 = new ControlComponent("C1", new Collection<IOperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) }, new Collection<IOrderOutput>(), new Collection<string>());
            ControlComponent c2 = new ControlComponent("C2", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            OrderOutput output = new OrderOutput("Test", "Output", provider.Object, c1);
            Task runningOpMode = output.SelectOperationMode(OpModeOne);
            await output.ResetAndWaitForIdle(SENDER);

            bool success = output.ChangeComponent(c2);

            Assert.False(success);
            Assert.AreEqual(c1.ComponentName, output.ComponentName);

            await output.StopAndWaitForStopped(SENDER);
            await output.DeselectOperationMode();
            await runningOpMode;
        }

        [Test]
        public void Given_SetOrderOutput_When_RequestSet_Then_IsSet()
        {
            ControlComponent c1 = new ControlComponent("C1", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            OrderOutput output = new OrderOutput("Test", "Output", provider.Object, c1);

            Assert.True(output.IsSet);
        }

        [Test]
        public void Given_NotSetOrderOutput_When_RequestSet_Then_IsNotSet()
        {
            ControlComponent c1 = new ControlComponent("C1", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            OrderOutput output = new OrderOutput("Test", "Output", provider.Object);

            Assert.False(output.IsSet);
        }

        [Test]
        public void Given_NotSetOrderOutput_When_ChangeComponent_Then_Changed()
        {
            ControlComponent c2 = new ControlComponent("C2", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            OrderOutput output = new OrderOutput("Test", "Output", provider.Object);

            bool success = output.ChangeComponent(c2);
            Assert.True(success);
        }

        [Test]
        public void Given_SetOrderOutput_When_ClearComponent_Then_IsNotSet()
        {
            ControlComponent c1 = new ControlComponent("C1", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            OrderOutput output = new OrderOutput("Test", "Output", provider.Object, c1);

            output.ClearComponent();

            Assert.False(output.IsSet);
        }

        [Test]
        public void Given_Id_When_GetId_Then_Id()
        {
            OrderOutput output = new OrderOutput("Test", "Output", provider.Object);
            Assert.AreEqual(output.OwnerId, "Output");
        }

        [Test]
        public void Given_OrderOutput_When_ChangeComponent_Then_Changed()
        {
            ControlComponent c1 = new ControlComponent("CC1", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            ControlComponent c2 = new ControlComponent("CC2", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            provider.Setup(p => p.GetComponent<IControlComponent>("CC2")).Returns(c2);

            OrderOutput output = new OrderOutput("Test", "Output", provider.Object, c1);

            Assert.AreEqual("CC1", output.ComponentName);
            output.ChangeComponent("CC2");
            Assert.AreEqual("CC2", output.ComponentName);
        }

        [Test]
        public void Given_ControlComponent_When_ChangeOutput_Then_Changed()
        {
            ControlComponent c3 = new ControlComponent("CC3", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>());
            provider.Setup(p => p.GetComponent<IControlComponent>("CC3")).Returns(c3);
            var orderOutput = orderOutputs.First(o => o.Role == "ROLE_ONE");

            Assert.AreEqual("CC1", orderOutput.ComponentName);
            cc.ChangeOutput("ROLE_ONE", "CC3");
            Assert.AreEqual("CC3", orderOutput.ComponentName);

        }
    }
}