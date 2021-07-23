using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class ControlComponentOccupationTests
    {
        string OCCUPIER_A = "A";
        string OCCUPIER_B = "B";

        [Test, AutoData]
        public void Given_Free_When_Occupier_Then_None(ControlComponent cc)
        {
            Assert.AreEqual("NONE", cc.OCCUPIER);
        }

        [Test, AutoData]
        public void Given_Free_When_IsFree_Then_True(ControlComponent cc)
        {
            cc.IsFree().Should().BeTrue();
        }

        [Test, AutoData]
        public void Given_Free_When_Occupy_Then_Occupied(ControlComponent cc)
        {
            cc.Occupy(OCCUPIER_A);
            cc.IsOccupied().Should().BeTrue();
            cc.OCCUPIER.Should().Be(OCCUPIER_A);
        }

        [Test, AutoData]
        public void Given_OccupiedByA_When_FreeByA_Then_IsFree(ControlComponent cc)
        {
            cc.Occupy(OCCUPIER_A);
            cc.Free(OCCUPIER_A);
            cc.IsFree().Should().BeTrue();
        }

        [Test, AutoData]
        public void Given_OccupiedByA_When_FreeByB_Then_OccupiedByA(ControlComponent cc)
        {
            cc.Occupy(OCCUPIER_A);
            cc.Free(OCCUPIER_B);
            cc.IsOccupied().Should().BeTrue();
            cc.OCCUPIER.Should().Be(OCCUPIER_A);
        }

        [Test, AutoData]
        public void Given_OccupiedByA_When_OccupyByB_Then_OccupiedByA(ControlComponent cc)
        {
            cc.Occupy(OCCUPIER_A);
            cc.Occupy(OCCUPIER_B);
            cc.IsOccupied().Should().BeTrue();
            cc.OCCUPIER.Should().Be(OCCUPIER_A);
        }

        [Test, AutoData]
        public void Given_OccupiedByA_When_PrioByB_Then_OccupiedByB(ControlComponent cc)
        {
            cc.Occupy(OCCUPIER_A);
            cc.Prio(OCCUPIER_B);
            cc.IsOccupied().Should().BeTrue();
            cc.OCCUPIER.Should().Be(OCCUPIER_B);
        }
    }
}