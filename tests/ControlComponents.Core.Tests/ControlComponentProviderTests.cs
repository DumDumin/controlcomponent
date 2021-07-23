using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
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
        public void Given_TypeNotAvailable_When_GetComponent_Then_Throw(ControlComponentProvider provider, ControlComponent cc)
        {
            provider.Add(cc.ComponentName, cc);

            Action act = () => provider.GetComponent<ControlComponentProvider>(cc.ComponentName);

            act.Should().Throw<InvalidOperationException>();
        }
    }
}