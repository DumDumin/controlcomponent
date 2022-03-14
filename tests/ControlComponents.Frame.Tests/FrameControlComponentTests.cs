using System;
using System.Threading.Tasks;
using ControlComponents.Core;
using ControlComponents.Core.Tests;
using FluentAssertions;
using Moq;
using NLog;
using NUnit.Framework;

namespace ControlComponents.Frame.Tests
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
        ExtendedControlComponent externalCC;
        OrderOutput externalCCOutput;
        FrameControlComponent<IExtendedControlComponent> sut;
        Task running;
        Mock<IControlComponentProvider> provider;
        private FrameWrapperOutput wrapperOutput;

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
            provider.Setup(p => p.GetComponent<IControlComponent>(ese.ComponentName)).Returns(ese);
            ese.AddOperationMode(new OperationModeRaw(OPMODE));

            // EXTERNAL CC
            externalCC = new ExtendedControlComponent(CC);
            provider.Setup(p => p.GetComponent<IControlComponent>(externalCC.ComponentName)).Returns(externalCC);

            // external controlcomponent has different opmodes to test with
            externalCC.AddOperationMode(new FailingOperationModeReset(FailingOpModeReset));
            externalCC.AddOperationMode(new FailingOperationModeStart(FailingOpModeStart));
            externalCC.AddOperationMode(new FailingOperationModeExecute(FailingOpModeExecute));
            externalCC.AddOperationMode(new OperationModeAsync(NormalOpMode));

            // external controlcomponent has one output
            externalCCOutput = new OrderOutput(ROLE, externalCC.ComponentName, provider.Object, ese);
            externalCC.AddOrderOutput(externalCCOutput);

            // FRAME CC
            sut = FrameControlComponent<IExtendedControlComponent>.Create(FRAMECC, externalCC, provider.Object);
            provider.Setup(p => p.GetComponent<IControlComponent>(sut.ComponentName)).Returns(sut);

            wrapperOutput = new FrameWrapperOutput(ROLE, "GSE", provider.Object, sut);
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
        }

        [Test]
        public void When_Created_Then_OutputsCreated()
        {
            sut.Roles.Should().Contain(ROLE);
        }

        [Test]
        public void When_IsUsableBy_WithReflection_Then_ReturnTrue()
        {
            externalCC.Occupy(sut.ComponentName);
            wrapperOutput.IsUsableBy(CC).Should().BeTrue();
        }

        [Test]
        public void When_Occupy_WithReflection_Then_OccupiedBySUT()
        {
            wrapperOutput.Occupy(CC);
            externalCC.OCCUPIER.Should().Be(FRAMECC);
        }

        [Test]
        public async Task When_Reset_WithReflection_Then_EseIsResetting()
        {
            Task running = wrapperOutput.SelectOperationMode(NormalOpMode);

            wrapperOutput.Reset(CC);
            externalCC.EXST.Should().Be(ExecutionState.RESETTING);
            externalCC.OCCUPIER.Should().Be(sut.ComponentName);
            wrapperOutput.Stop(CC);
            // This is usually done by the output which used the sut (FrameControlComponent)
            await externalCC.WaitForStopped();

            await wrapperOutput.DeselectOperationMode();
            await running;
        }

        [Test]
        public void Given_OutputIsNotSet_When_Create_Then_OutputNotSetAtExternal()
        {
            externalCCOutput.IsSet.Should().BeTrue();
            sut.ClearOutput(ROLE);
            externalCCOutput.IsSet.Should().BeFalse();
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
        public async Task Given_ExternalCC_When_NormalRunCalledByExternalItself_Then_CorrectStateFlow()
        {
            running = sut.SelectOperationMode(NormalOpMode);
            sut.OpModeName.Should().Be(NormalOpMode);
            externalCC.OpModeName.Should().Be(NormalOpMode);

            // IMPORTANT: This is an attempt to verify, that externalCC changed state to IDLE before sut does
            externalCC.Reset(sut.ComponentName);
            await sut.WaitForIdle();
            externalCC.EXST.Should().Be(ExecutionState.IDLE);

            // IMPORTANT: This is an attempt to verify, that externalCC changed state to EXECUTE before sut does
            externalCC.Start(sut.ComponentName);
            await sut.WaitForExecute();
            externalCC.EXST.Should().Be(ExecutionState.EXECUTE);

            await sut.WaitForCompleted();
            sut.EXST.Should().Be(ExecutionState.COMPLETED);
            externalCC.EXST.Should().Be(ExecutionState.COMPLETED);

            // IMPORTANT: This is an attempt to verify, that externalCC changed state to STOPPED before sut does
            externalCC.Stop(sut.ComponentName);
            await sut.WaitForStopped();
            externalCC.EXST.Should().Be(ExecutionState.STOPPED);
        }

        [Test]
        public async Task Given_ExternalCC_When_NormalSuspendRun_Then_CorrectStateFlow()
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

            await sut.SuspendAndWaitForSuspended(SENDER);
            sut.EXST.Should().Be(ExecutionState.SUSPENDED);
            externalCC.EXST.Should().Be(ExecutionState.SUSPENDED);

            await sut.UnsuspendAndWaitForExecute(SENDER);
            sut.EXST.Should().Be(ExecutionState.EXECUTE);
            externalCC.EXST.Should().Be(ExecutionState.EXECUTE);

            await sut.WaitForCompleted();
            sut.EXST.Should().Be(ExecutionState.COMPLETED);
            externalCC.EXST.Should().Be(ExecutionState.COMPLETED);
        }

        [Test]
        public async Task Given_ExternalCC_When_SuspendCalledByExternalItself_Then_CorrectStateFlow()
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

            // IMPORTANT: This is an attempt to verify, that externalCC changed state to SUSPENDED before sut does
            externalCC.Suspend(sut.ComponentName);
            await sut.WaitForSuspended();
            externalCC.EXST.Should().Be(ExecutionState.SUSPENDED);

            // IMPORTANT: This is an attempt to verify, that externalCC changed state to EXECUTE before sut does
            externalCC.Unsuspend(sut.ComponentName);
            await sut.WaitForExecute();
            externalCC.EXST.Should().Be(ExecutionState.EXECUTE);

            await sut.WaitForCompleted();
            sut.EXST.Should().Be(ExecutionState.COMPLETED);
            externalCC.EXST.Should().Be(ExecutionState.COMPLETED);
        }

        [Test]
        public async Task Given_ExternalCC_When_Abort_Then_Aborted()
        {
            running = sut.SelectOperationMode(NormalOpMode);
            sut.OpModeName.Should().Be(NormalOpMode);
            externalCC.OpModeName.Should().Be(NormalOpMode);

            await sut.ResetAndWaitForIdle(SENDER);
            sut.EXST.Should().Be(ExecutionState.IDLE);
            externalCC.EXST.Should().Be(ExecutionState.IDLE);

            await sut.AbortAndWaitForAborted(SENDER);
            sut.EXST.Should().Be(ExecutionState.ABORTED);
            externalCC.EXST.Should().Be(ExecutionState.ABORTED);
        }

        [Test]
        public async Task Given_ExternalCC_When_AbortCalledByExternalItself_Then_Aborted()
        {
            running = sut.SelectOperationMode(NormalOpMode);
            sut.OpModeName.Should().Be(NormalOpMode);
            externalCC.OpModeName.Should().Be(NormalOpMode);

            await sut.ResetAndWaitForIdle(SENDER);
            sut.EXST.Should().Be(ExecutionState.IDLE);
            externalCC.EXST.Should().Be(ExecutionState.IDLE);

            // IMPORTANT: This is an attempt to verify, that externalCC changed state to ABORTED before sut does
            externalCC.Abort(sut.ComponentName);
            await sut.WaitForAborted();
            externalCC.EXST.Should().Be(ExecutionState.ABORTED);

            // IMPORTANT: This is an attempt to verify, that externalCC changed state to STOPPED before sut does
            externalCC.Clear(sut.ComponentName);
            await sut.WaitForStopped();
            externalCC.EXST.Should().Be(ExecutionState.STOPPED);
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
            Task running = externalCC.SelectOperationMode(NormalOpMode);
            await externalCC.ResetAndWaitForIdle(SENDER);
            externalCC.EXST.Should().Be(ExecutionState.IDLE);

            // request property of ese through sut
            ExecutionState result = wrapperOutput.EXST;
            // ExecutionState result = sut.ReadProperty<ExecutionState>(externalCCOutput.Role, nameof(IControlComponent.EXST));
            result.Should().Be(externalCC.EXST);

            await externalCC.StopAndWaitForStopped(SENDER);
            await externalCC.DeselectOperationMode();
            await running;
        }

        [Test]
        public void Given_CCWithOutput_When_SubscribeEvent_Then_SubscribedToOutput()
        {
            running = wrapperOutput.SelectOperationMode(NormalOpMode);
            int i = 0;
            wrapperOutput.OccupierChanged += (object sender, OccupationEventArgs e) => i++;
            externalCC.Occupy("SENDER");
            i.Should().Be(1);
            externalCC.Prio("OCCUPIER");
            i.Should().Be(2);
        }

        [Test]
        public void Given_CCWithOutput_When_UnsubscribeEvent_Then_UnsubscribedFromOutput()
        {
            running = wrapperOutput.SelectOperationMode(NormalOpMode);
            int i = 0;
            OccupationEventHandler eventHandler = (object sender, OccupationEventArgs e) => i++;
            wrapperOutput.OccupierChanged += eventHandler;
            externalCC.Occupy("SENDER");
            i.Should().Be(1);
            wrapperOutput.OccupierChanged -= eventHandler;
            externalCC.Prio("OCCUPIER");
            i.Should().Be(1);
        }

        [Test]
        public void TestReflectionMethods()
        {
            sut.ReadProperty<string>(ROLE, nameof(IExtendedControlComponent.TestString)).Should().Be("TestString");

            sut.CallMethod(ROLE, nameof(IExtendedControlComponent.TestMethod));
            externalCC.TestString.Should().Be("ChangedTestString");

            sut.CallMethod<string>(ROLE, nameof(IExtendedControlComponent.ChangedTestString), "NewTestString");
            externalCC.TestString.Should().Be("NewTestString");

            sut.CallMethod<string,string>(ROLE, nameof(IExtendedControlComponent.ChangedAndReturnTestString), "NextTestString").Should().Be("NextTestString");

            sut.CallMethod<string>(ROLE, nameof(IExtendedControlComponent.GetTestString)).Should().Be("NextTestString");
        }

        interface IExtendedControlComponent : IControlComponent 
        { 
            string TestString { get; }
            void TestMethod();
            void ChangedTestString(string newString);
            string ChangedAndReturnTestString(string newString);
            string GetTestString();
        }

        internal class ExtendedControlComponent : ControlComponents.Core.ControlComponent, IExtendedControlComponent
        {
            private string testString = "TestString";
            public ExtendedControlComponent(string name) : base(name)
            {
            }

            public string TestString => testString;

            public void TestMethod()
            {
                testString = "ChangedTestString";
            }

            public void ChangedTestString(string newString)
            {
                testString = newString;
            }

            public string ChangedAndReturnTestString(string newString)
            {
                testString = newString;
                return testString;
            }
            public string GetTestString()
            {
                return TestString;
            }
        }
    }
}