using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class ControlComponentProviderTests
    {
        ControlComponent cc;

        string CC = "CC";

        [SetUp]
        public void Setup()
        {
            var OpModes = new Collection<IOperationMode>();
            var orderOutputs = new Collection<IOrderOutput>();
            cc = new ControlComponent(CC, OpModes, orderOutputs, new Collection<string>());
        }

        [Test]
        public void Given_TypeAvailable_When_GetComponent_Then_Return()
        {
            ControlComponentProvider provider = new ControlComponentProvider();
            provider.Add("CC", cc);
            var c = provider.GetComponent<ControlComponent>("CC");

            Assert.AreEqual("CC", cc.ComponentName);
        }

        [Test]
        public void Given_TypeNotAvailable_When_GetComponent_Then_Throw()
        {
            ControlComponentProvider provider = new ControlComponentProvider();
            Mock<IControlComponent> mock = new Mock<IControlComponent>();
            provider.Add("CC", mock.Object);

            Assert.Throws(typeof(InvalidOperationException), () => provider.GetComponent<ControlComponent>("CC"));
        }
    }
}