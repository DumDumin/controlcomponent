using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NLog;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class FrameControlComponentTests
    {
        string CC = "CC";
        string ESE = "ESE";
        string FRAMECC = "FRAMECC";
        string SENDER = "SENDER";
        string ROLE = "ROLE";
        string OPMODE = "OPMODE";
        string NormalOpMode = "CONFIGURE";
        string FailingOpModeReset = "FAILING-Reset";
        string FailingOpModeStart = "FAILING-Start";
        string FailingOpModeExecute = "FAILING-Execute";

        ControlComponent ese;
        ControlComponent externalCC;
        ExtendedOrderOutput externalCCOutput;
        FrameControlComponent<IControlComponent> sut;
        Task running;
        Mock<IControlComponentProvider> provider;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
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
            provider = new Mock<IControlComponentProvider>();

            // ESE
            ese = new ControlComponent(ESE);
            ese.AddOperationMode(new OperationMode(OPMODE));

            provider.Setup(p => p.GetComponent<IControlComponent>(ese.ComponentName)).Returns(ese);

            // EXTERNAL CC
            externalCC = new ControlComponent(CC);

            // external controlcomponent has different opmodes to test with
            externalCC.AddOperationMode(new FailingOperationModeReset(FailingOpModeReset));
            externalCC.AddOperationMode(new FailingOperationModeStart(FailingOpModeStart));
            externalCC.AddOperationMode(new FailingOperationModeExecute(FailingOpModeExecute));
            externalCC.AddOperationMode(new OperationModeAsync(NormalOpMode));

            // external controlcomponent has one output
            externalCCOutput = new ExtendedOrderOutput(ROLE, externalCC.ComponentName, provider.Object);
            externalCC.AddOrderOutput(externalCCOutput);

            provider.Setup(p => p.GetComponent<IControlComponent>(externalCC.ComponentName)).Returns(externalCC);

            // FRAME CC
            sut = FrameControlComponent<IControlComponent>.Create(FRAMECC, externalCC, provider.Object);
            sut.ChangeOutput(ROLE, ese.ComponentName);

            provider.Setup(p => p.GetComponent<IControlComponent>(sut.ComponentName)).Returns(sut);
        }

        [TearDown]
        public async Task TearDown()
        {
            await sut.StopAndWaitForStopped(SENDER);
            sut.EXST.Should().Be(ExecutionState.STOPPED);
            externalCC.EXST.Should().Be(ExecutionState.STOPPED);

            if(sut.OpModeName != "NONE")
            {
                await sut.DeselectOperationMode();
                await running;
            }
            sut.OpModeName.Should().Be("NONE");
            externalCC.OpModeName.Should().Be("NONE");

            externalCCOutput.IsSet.Should().BeFalse();
        }

        [Test]
        public void When_Created_Then_OutputsCreated()
        {
            sut.Roles.Should().Contain(ROLE);
        }

        [Test]
        public async Task Given_MissingOutput_When_SelectOperationMode_Then_Abort()
        {
            sut.ClearOutput(ROLE);
            running = sut.SelectOperationMode(NormalOpMode);
            await sut.WaitForAborted();
            sut.EXST.Should().Be(ExecutionState.ABORTED);
        }

        [Test]
        public void Given_Output_When_SelectOperationMode_Then_FrameComponentAtOutputConfigured()
        {
            // Output is not "filled" before opmode is started
            Assert.Throws(typeof(NullReferenceException), () => externalCCOutput.ComponentName.ToString());

            running = sut.SelectOperationMode(NormalOpMode);

            externalCCOutput.ComponentName.Should().Be(ese.ComponentName);
            externalCCOutput.GetControlComponent.ComponentName.Should().Be(sut.ComponentName);
        }

        [Test]
        public async Task Given_ExternalCC_When_NormalRun_Then_CorrectStateFlow()
        {
            running = sut.SelectOperationMode(NormalOpMode);
            sut.OpModeName.Should().Be(NormalOpMode);
            externalCC.OpModeName.Should().Be(NormalOpMode);

            await sut.ResetAndWaitForIdle(SENDER);
            sut.EXST.Should().Be(ExecutionState.IDLE);
            externalCC.EXST.Should().Be(ExecutionState.IDLE);

            await sut.StartAndWaitForExecute(SENDER);
            sut.EXST.Should().Be(ExecutionState.EXECUTE);
            externalCC.EXST.Should().Be(ExecutionState.EXECUTE);

            await sut.WaitForCompleted();
            sut.EXST.Should().Be(ExecutionState.COMPLETED);
            externalCC.EXST.Should().Be(ExecutionState.COMPLETED);
        }

        [Test]
        public async Task Given_ExternalOpMode_When_ResetFails_Then_Aborted()
        {
            running = sut.SelectOperationMode(FailingOpModeReset);
            sut.Reset(SENDER);

            await sut.WaitForAborted();
            externalCC.EXST.Should().Be(ExecutionState.ABORTED);
            sut.EXST.Should().Be(ExecutionState.ABORTED);
        }

        [Test]
        public async Task Given_ExternalOpMode_When_StartFails_Then_Aborted()
        {
            running = sut.SelectOperationMode(FailingOpModeStart);

            await sut.ResetAndWaitForIdle(SENDER);
            sut.EXST.Should().Be(ExecutionState.IDLE);
            externalCC.EXST.Should().Be(ExecutionState.IDLE);

            sut.Start(SENDER);

            await sut.WaitForAborted();
            externalCC.EXST.Should().Be(ExecutionState.ABORTED);
            sut.EXST.Should().Be(ExecutionState.ABORTED);
        }

        [Test]
        public async Task Given_ExternalOpMode_When_ExecuteFails_Then_Aborted()
        {
            running = sut.SelectOperationMode(FailingOpModeExecute);

            await sut.ResetAndWaitForIdle(SENDER);
            sut.EXST.Should().Be(ExecutionState.IDLE);
            externalCC.EXST.Should().Be(ExecutionState.IDLE);

            await sut.StartAndWaitForExecute(SENDER);
            sut.EXST.Should().Be(ExecutionState.EXECUTE);
            externalCC.EXST.Should().Be(ExecutionState.EXECUTE);

            await sut.WaitForAborted();
            externalCC.EXST.Should().Be(ExecutionState.ABORTED);
            sut.EXST.Should().Be(ExecutionState.ABORTED);
        }

        ///////////////

        [Test]
        public async Task Given_CCWithOutput_When_GetProperty_Then_ReturnOutputProperty()
        {
            // put the ese in a different state than STOPPED to see a difference between sut
            Task running = ese.SelectOperationMode(OPMODE);
            await ese.ResetAndWaitForIdle(SENDER);
            ese.EXST.Should().Be(ExecutionState.IDLE);

            // request property of ese through sut
            ExecutionState result = sut.ReadProperty<ExecutionState>(externalCCOutput.Role, nameof(IControlComponent.EXST));
            result.Should().Be(ese.EXST);

            await ese.StopAndWaitForStopped(SENDER);
            await ese.DeselectOperationMode();
            await running;
        }

        [Test]
        public void Given_CCWithOutput_When_SubscribeEvent_Then_SubscribedToOutput()
        {
            running = sut.SelectOperationMode(NormalOpMode);
            int i = 0;
            sut.Subscribe<OccupationEventHandler>(externalCCOutput.Role, nameof(sut.OccupierChanged), (object sender, OccupationEventArgs e) => i++);
            externalCCOutput.Occupy("SENDER");
            i.Should().Be(1);
            externalCCOutput.Prio("OCCUPIER");
            i.Should().Be(2);
        }

        [Test]
        public void Given_CCWithOutput_When_UnsubscribeEvent_Then_UnsubscribedFromOutput()
        {
            running = sut.SelectOperationMode(NormalOpMode);
            int i = 0;
            OccupationEventHandler eventHandler = (object sender, OccupationEventArgs e) => i++;
            sut.Subscribe<OccupationEventHandler>(externalCCOutput.Role, nameof(sut.OccupierChanged), eventHandler);
            externalCCOutput.Occupy("SENDER");
            i.Should().Be(1);
            sut.Unsubscribe<OccupationEventHandler>(externalCCOutput.Role, nameof(sut.OccupierChanged), eventHandler);
            externalCCOutput.Prio("OCCUPIER");
            i.Should().Be(1);
        }

        interface IExtendedOrderOutput : IExtendedControlComponent
        {
            string TestString { get; }
        }

        interface IExtendedControlComponent : IControlComponent { }

        internal class ExtendedOrderOutput : OrderOutput, IExtendedOrderOutput
        {
            public int TestValue => 100;
            public string TestString => "Test";
            public IControlComponent GetControlComponent => this.controlComponent;
            public ExtendedOrderOutput(string role, string id, IControlComponentProvider provider) : base(role, id, provider)
            {
            }

        }
    }
}