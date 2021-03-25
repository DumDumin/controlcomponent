using System.Threading;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class OperationModeTests
    {
        string OPMODENAME = "DEFAULT";
        CancellationTokenSource tokenOwner;


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
        }

        [Test]
        public void Given_Stopped_When_Select_Then_Stopped()
        {
            var operationMode = new OperationMode(OPMODENAME);

            Assert.AreEqual(OPMODENAME, operationMode.OpModeName);
        }

        [Test]
        public async Task Given_Stopped_When_Reset_Then_Idle()
        {
            OperationMode operation = new OperationMode(OPMODENAME);
            Execution execution = new Execution();

            Task select = operation.Select(execution);
            execution.SetState(ExecutionState.RESETTING);
            await Helper.WaitForState(execution, ExecutionState.IDLE);
            Assert.AreEqual(ExecutionState.IDLE, execution.EXST);

            operation.Deselect();
            await select;
        }

        [Test]
        public async Task Given_Idle_When_Start_Then_Completed()
        {
            OperationMode operation = new OperationMode(OPMODENAME);
            IExecution execution = new Execution();

            
            Task select = operation.Select(execution);
            execution.SetState(ExecutionState.RESETTING);
            await Helper.WaitForState(execution, ExecutionState.IDLE);
            execution.SetState(ExecutionState.STARTING);
            await Helper.WaitForState(execution, ExecutionState.COMPLETED);

            operation.Deselect();
            await select;

            Assert.AreEqual(ExecutionState.COMPLETED, execution.EXST);
        }
    }
}