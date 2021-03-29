using System.Threading.Tasks;
using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class ControlComponentOccupationTests
    {
        ControlComponent cc;

        string OCCUPIER_A = "A";
        string OCCUPIER_B = "B";

        [SetUp]
        public void Setup()
        {
            cc = new ControlComponent();
        }

        [Test]
        public void Given_Free_When_Occupier_Then_None()
        {
            Assert.AreEqual("NONE", cc.OCCUPIER);
        }

        [Test]
        public void Given_Free_When_IsFree_Then_True()
        {
            Assert.AreEqual(true, cc.IsFree());
        }

        [Test]
        public void Given_Free_When_Occupy_Then_Occupied()
        {
            cc.Occupy("TEST");
            Assert.AreEqual(true, cc.IsOccupied());
            Assert.AreEqual("TEST", cc.OCCUPIER);
        }

        [Test]
        public void Given_OccupiedByA_When_FreeByA_Then_IsFree()
        {
            cc.Occupy("A");
            Assert.AreEqual(true, cc.IsOccupied());
            Assert.AreEqual("A", cc.OCCUPIER);

            cc.Free("A");
            Assert.AreEqual(true, cc.IsFree());
        }

        [Test]
        public void Given_OccupiedByA_When_FreeByB_Then_OccupiedByA()
        {
            cc.Occupy("A");
            Assert.AreEqual(true, cc.IsOccupied());
            Assert.AreEqual("A", cc.OCCUPIER);

            cc.Free("B");
            Assert.AreEqual(true, cc.IsOccupied());
            Assert.AreEqual("A", cc.OCCUPIER);
        }

        [Test]
        public void Given_OccupiedByA_When_OccupyByB_Then_OccupiedByA()
        {
            cc.Occupy("A");
            Assert.AreEqual(true, cc.IsOccupied());
            Assert.AreEqual("A", cc.OCCUPIER);

            cc.Occupy("B");
            Assert.AreEqual(true, cc.IsOccupied());
            Assert.AreEqual("A", cc.OCCUPIER);
        }
    }
}