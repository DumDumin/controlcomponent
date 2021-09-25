using System.Threading.Tasks;
using AutoFixture.NUnit3;
using ControlComponents.Core;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Frame.Tests
{
    public class ConfigOperationModeTests
    {
        Mock<IControlComponentProvider> provider;

        [SetUp]
        public void Setup()
        {   
            provider = new Mock<IControlComponentProvider>();
        }

        [Test, AutoData]
        public async Task _df(ControlComponent cc, ControlComponent externalCC)
        {
            var sut = new ConfigOperationMode("CONFIG", externalCC);
            cc.AddOperationMode(sut);
            externalCC.AddOrderOutput(new OrderOutput("ROLE", externalCC.ComponentName, provider.Object));

            Task running = cc.SelectOperationMode("CONFIG");
            await cc.WaitForAborted();
            cc.EXST.Should().Be(ExecutionState.ABORTED);
        }
    }
}