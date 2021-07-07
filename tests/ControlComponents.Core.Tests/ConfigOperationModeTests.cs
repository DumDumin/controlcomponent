using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class ConfigOperationModeTests
    {
        OrderOutput orderOutput;
        string CC = "CC";
        string SENDER = "SENDER";
        string OpModeName = "CONFIGURE";

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
            var OpModes = new Collection<IOperationMode>() { new OperationMode("OpModeOne"), new OperationMode("OpModeTwo") };
            orderOutput = new OrderOutput("First", "Output", provider.Object, new ControlComponent(CC, OpModes, new Collection<IOrderOutput>(), new Collection<string>()));
        }

        [Test]
        public async Task Given_ExternalCC_When_NormalRun_Then_CorrectStateFlow()
        {
            ControlComponent externalCC = new ControlComponent("CC1");
            OperationMode externalOpmode = new OperationModeAsync(OpModeName);
            externalCC.AddOperationMode(externalOpmode);
            ConfigOperationMode configureOpmode = new ConfigOperationMode(OpModeName, externalCC);
            ControlComponent configureCC = new ControlComponent("CC2");
            configureCC.AddOperationMode(configureOpmode);

            Task running = configureCC.SelectOperationMode(OpModeName);
            Assert.AreEqual(OpModeName, configureCC.OpModeName);
            Assert.AreEqual(OpModeName, externalCC.OpModeName);

            await configureCC.ResetAndWaitForIdle(SENDER);
            Assert.AreEqual(ExecutionState.IDLE, configureCC.EXST);
            Assert.AreEqual(ExecutionState.IDLE, externalCC.EXST);

            await configureCC.StartAndWaitForExecute(SENDER);
            Assert.AreEqual(ExecutionState.EXECUTE, configureCC.EXST);
            Assert.AreEqual(ExecutionState.EXECUTE, externalCC.EXST);

            await configureCC.WaitForCompleted();
            Assert.AreEqual(ExecutionState.COMPLETED, configureCC.EXST);
            Assert.AreEqual(ExecutionState.COMPLETED, externalCC.EXST);

            await configureCC.StopAndWaitForStopped(SENDER, false);
            Assert.AreEqual(ExecutionState.STOPPED, configureCC.EXST);
            Assert.AreEqual(ExecutionState.STOPPED, externalCC.EXST);

            await configureCC.DeselectOperationMode();
            await running;
            Assert.AreEqual("NONE", configureCC.OpModeName);
            Assert.AreEqual("NONE", externalCC.OpModeName);
        }
    }
}