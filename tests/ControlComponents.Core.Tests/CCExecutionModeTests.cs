using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NLog;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class CCExecutionModeTests
    {
        string SENDER = "SENDER";

        [Test, AutoData]
        public void Given_AUTO_When_EXMODE_Then_AUTO(ControlComponent cc)
        {
            cc.EXMODE.Should().Be(ExecutionMode.AUTO);
        }

        [Test, AutoData]
        public void Given_AUTO_When_SemiAuto_Then_SEMIAUTO(ControlComponent cc)
        {
            cc.SemiAuto(SENDER);

            cc.EXMODE.Should().Be(ExecutionMode.SEMIAUTO);
        }

        [Test, AutoData]
        public void Given_SEMIAUTO_When_Auto_Then_AUTO(ControlComponent cc)
        {
            cc.SemiAuto(SENDER);
            cc.Auto(SENDER);

            cc.EXMODE.Should().Be(ExecutionMode.AUTO);
        }

        [Test, AutoData]
        public void Given_AUTO_When_SemiAuto_Then_RaiseEvent(ControlComponent cc)
        {
            ExecutionMode newMode = ExecutionMode.AUTO;
            cc.ExecutionModeChanged += (object sender, ExecutionModeEventArgs e) => newMode = e.ExecutionMode;

            cc.SemiAuto(SENDER);
            newMode.Should().Be(ExecutionMode.SEMIAUTO);
            cc.Auto(SENDER);
            newMode.Should().Be(ExecutionMode.AUTO);
        }
    }
}