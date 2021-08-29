using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class ControlComponentServiceInterfaceTests
    {
        string CC = "CC";
        string SENDER = "SENDER";
        string OPMODE = "OPMODE";
        string ROLE = "ROLE";

        [Test, AutoData]
        public void Given_CC_When_GetProperty_Then_Return(ControlComponent sut)
        {
            ExecutionState result = sut.ReadProperty<ExecutionState>(ROLE, nameof(IControlComponent.EXST));
            result.Should().Be(sut.EXST);
        }

        [Test, AutoData]
        public void Given_CC_When_CallMethod_Then_Return(ControlComponent sut)
        {
            bool result = sut.CallMethod<bool>(ROLE, nameof(IControlComponent.IsFree));
            result.Should().Be(true);
            // sut.EXST.Should().Be(ExecutionState.RESETTING);
            // result.Should().Be(sut.EXST);
        }
        [Test, AutoData]
        public async Task Given_CC_When_CallMethod_Then_Returnm(ControlComponent sut)
        {
            sut.AddOperationMode(new OperationModeAsync(OPMODE));

            Task running = sut.CallMethod<string, Task>(ROLE, nameof(IControlComponent.SelectOperationMode), OPMODE);
            sut.CallMethod<string, object>(ROLE, nameof(IControlComponent.Reset), "SENDER");
            sut.EXST.Should().Be(ExecutionState.RESETTING);
            sut.CallMethod<string, object>(ROLE, nameof(IControlComponent.Stop), "SENDER");
            ExecutionState exst = sut.ReadProperty<ExecutionState>(ROLE, nameof(IControlComponent.EXST));

            // TODO StopAndWait is not usable in this context => create new tests with 3 components to emulate correct behaviour
            while (exst != ExecutionState.STOPPED)
            {
                await Task.Delay(1);
                exst = sut.ReadProperty<ExecutionState>(ROLE, nameof(IControlComponent.EXST));
            }

            await sut.CallMethod<Task>(ROLE, nameof(IControlComponent.DeselectOperationMode));
            await running;
        }

        [Test, AutoData]
        public async Task Given_CCWithOutput_When_GetProperty_Then_ReturnOutputProperty(ControlComponent output)
        {
            Mock<IControlComponentProvider> provider = new Mock<IControlComponentProvider>();
            IOrderOutput orderOutput = new ExtendedOrderOutput(ROLE, CC, provider.Object, output);
            output.AddOperationMode(new OperationMode(OPMODE));

            FrameControlComponent sut = new FrameControlComponent(output, provider.Object, CC);
            sut.AddOrderOutput(orderOutput);

            Task running = output.SelectOperationMode(OPMODE);
            await output.ResetAndWaitForIdle(SENDER);

            ExecutionState result = sut.ReadProperty<ExecutionState>(orderOutput.Role, nameof(IControlComponent.EXST));
            result.Should().Be(output.EXST);

            await output.StopAndWaitForStopped(SENDER);
            await output.DeselectOperationMode();
            await running;
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
            public ExtendedOrderOutput(string role, string id, IControlComponentProvider provider, IControlComponent cc) : base(role, id, provider, cc)
            {
            }

        }

        public class ExtendedControlComponent : ControlComponent, IExtendedControlComponent
        {
            public ExtendedControlComponent(string name) : base(name)
            {
            }
        }
    }
}
