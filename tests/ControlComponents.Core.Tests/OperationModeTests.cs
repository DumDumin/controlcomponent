using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NLog;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class OperationModeTests
    {
        string OPMODENAME = "DEFAULT";
        CancellationTokenSource tokenOwner;
        IDictionary<string, IOrderOutput> outputs;

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
            // var execution = new Mock<IExecution>();
            // execution.Setup(p => p.)
            tokenOwner = new CancellationTokenSource();
            outputs = new Dictionary<string, IOrderOutput>();
        }

        [Test]
        public void Given_Stopped_When_WORKST_Then_None()
        {
            var operationMode = new OperationModeRaw(OPMODENAME, new Collection<string>(){"ROLE_ONE", "ROLE_TWO"});
            Assert.AreEqual("NONE", operationMode.WORKST);
        }

        [Test]
        public void Given_NotNeededRoles_When_Select_Then_OK()
        {
            var operationMode = new OperationModeRaw(OPMODENAME, new Collection<string>(){"ROLE_ONE", "ROLE_TWO"});
            Execution execution = new Execution("Execution");

            Assert.ThrowsAsync<Exception>(() => operationMode.Select(execution, outputs));
        }

        [Test]
        public void Given_NeededRoles_When_Select_Then_OK()
        {
            Mock<IControlComponentProvider> provider = new Mock<IControlComponentProvider>();
            outputs = new Dictionary<string, IOrderOutput>()
            {
                {"ROLE_ONE", new OrderOutput("ROLE_ONE", "Output", provider.Object, new ControlComponent("CC1", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>())) },
                {"ROLE_TWO", new OrderOutput("ROLE_TWO", "Output", provider.Object, new ControlComponent("CC2", new Collection<IOperationMode>(), new Collection<IOrderOutput>(), new Collection<string>())) },
            };
            var operationMode = new OperationModeRaw(OPMODENAME, new Collection<string>(){"ROLE_ONE", "ROLE_TWO"});
            Execution execution = new Execution("Execution");

            Assert.DoesNotThrowAsync(async () => {
                Task select = operationMode.Select(execution, outputs);
                await Task.Delay(5);
                operationMode.Deselect();
                await select;
            });
        }

        [Test]
        public void Given_Stopped_When_Select_Then_Stopped()
        {
            var operationMode = new OperationModeRaw(OPMODENAME);

            Assert.AreEqual(OPMODENAME, operationMode.OpModeName);
        }

        [Test]
        public async Task Given_Stopped_When_Reset_Then_Idle()
        {
            OperationModeRaw operation = new OperationModeRaw(OPMODENAME);
            Execution execution = new Execution("Execution");

            Task select = operation.Select(execution, outputs);

            execution.SetState(ExecutionState.RESETTING);
            await Helper.WaitForState(execution, ExecutionState.IDLE);
            Assert.AreEqual(ExecutionState.IDLE, execution.EXST);

            operation.Deselect();
            await select;
        }

        [Test]
        public async Task Given_Idle_When_Start_Then_Completed()
        {
            OperationModeRaw operation = new OperationModeRaw(OPMODENAME);
            IExecution execution = new Execution("Execution");

            
            Task select = operation.Select(execution, outputs);
            execution.SetState(ExecutionState.RESETTING);
            await Helper.WaitForState(execution, ExecutionState.IDLE);
            execution.SetState(ExecutionState.STARTING);
            await Helper.WaitForState(execution, ExecutionState.COMPLETED);

            operation.Deselect();
            await select;

            Assert.AreEqual(ExecutionState.COMPLETED, execution.EXST);
        }

        [Test, AutoData]
        public async Task Given_NoneOpmodeSelected_When_SelectOpmode_Then_RaiseEvent(ControlComponent cc)
        {
            OperationModeRaw operation = new OperationModeRaw(OPMODENAME);
            cc.AddOperationMode(operation);

            string newOperationMode = cc.OpModeName;
            cc.OperationModeChanged += (object sender, OperationModeEventArgs e) => newOperationMode = e.OperationModeName;
            Task running = cc.SelectOperationMode(OPMODENAME);

            newOperationMode.Should().Be(OPMODENAME);

            await cc.DeselectOperationMode();
            await running;
        }
    }
}