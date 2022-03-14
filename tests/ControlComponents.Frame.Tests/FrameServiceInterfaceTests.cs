using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using ControlComponents.Core;
using ControlComponents.Core.Tests;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Frame.Tests
{
    public class FrameServiceInterfaceTests
    {
        string CC = "CC";
        string FRAMECC = "FRAMECC";
        string OPMODE = "OPMODE";
        string ROLE = "ROLE";
        private Mock<IControlComponentProvider> provider;
        private FrameControlComponent<IControlComponent> sut;

        [SetUp]
        public void Setup()
        {
            provider = new Mock<IControlComponentProvider>();
            ControlComponent externalCC = new ControlComponent(CC);
            sut = FrameControlComponent<IControlComponent>.Create(FRAMECC, externalCC, provider.Object);
        }

        [Test]
        public void Given_CC_When_GetProperty_Then_Return()
        {
            ExecutionState result = sut.ReadProperty<ExecutionState>(ROLE, nameof(IControlComponent.EXST));
            result.Should().Be(sut.EXST);
        }

        [Test]
        public void Given_CC_When_CallMethod_Then_Return()
        {
            bool result = sut.CallMethod<bool>(ROLE, nameof(IControlComponent.IsFree));
            result.Should().Be(true);
        }

        [Test]
        public async Task Given_CC_When_CallMethodWithParam_Then_Return()
        {
            sut.AddOperationMode(new OperationModeAsync(OPMODE));

            Task running = sut.CallMethod<string, Task>(ROLE, nameof(IControlComponent.SelectOperationMode), OPMODE);
            sut.CallMethod<string>(ROLE, nameof(IControlComponent.Reset), "SENDER");
            sut.EXST.Should().Be(ExecutionState.RESETTING);
            sut.CallMethod<string>(ROLE, nameof(IControlComponent.Stop), "SENDER");
            ExecutionState exst = sut.ReadProperty<ExecutionState>(ROLE, nameof(IControlComponent.EXST));

            await sut.WaitForStopped();

            await sut.CallMethod<Task>(ROLE, nameof(IControlComponent.DeselectOperationMode));
            await running;
        }

        [Test]
        public void Given_CC_When_SubscribeEvent_Then_Subscribed()
        {
            int i = 0;
            sut.Subscribe<OccupationEventHandler>("", nameof(sut.OccupierChanged), (object sender, OccupationEventArgs e) => i++);
            sut.Occupy("SENDER");
            i.Should().Be(1);
        }

        [Test]
        public void Given_CC_When_UnsubscribeEvent_Then_Unsubscribed()
        {
            int i = 0;
            OccupationEventHandler eventHandler = (object sender, OccupationEventArgs e) => i++;
            sut.Subscribe<OccupationEventHandler>("", nameof(sut.OccupierChanged), eventHandler);
            sut.Unsubscribe<OccupationEventHandler>("", nameof(sut.OccupierChanged), eventHandler);
            sut.Occupy("SENDER");
            i.Should().Be(0);
        }
    }
}
