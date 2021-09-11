using System;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class ControlComponentProviderTests
    {
        [Test, AutoData]
        public void Given_TypeAvailable_When_GetComponent_Then_Return(ControlComponentProvider provider, ControlComponent cc)
        {
            provider.Add(cc.ComponentName, cc);
            
            var c = provider.GetComponent<ControlComponent>(cc.ComponentName);

            c.ComponentName.Should().Be(cc.ComponentName);
        }

        [Test, AutoData]
        public void Given_TypeNotAvailable_When_GetComponent_Then_Throw(ControlComponentProvider provider, ControlComponent cc, ControlComponent ccc)
        {
            provider.Add(cc.ComponentName, cc);

            Action act = () => provider.GetComponent<ControlComponent>(ccc.ComponentName);

            act.Should().Throw<InvalidOperationException>();
        }

        [Test, AutoData]
        public void Given_TypeAvailable_When_GetComponents_Then_ReturnComponents(ControlComponentProvider provider, ControlComponent cc)
        {
            provider.Add(cc.ComponentName, cc);
            
            var c = provider.GetComponents<ControlComponent>();

            c.Count().Should().Be(1);
            c.First().ComponentName.Should().Be(cc.ComponentName);
        }
    }
}