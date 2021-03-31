using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class ControlComponentOccupationTests
    {
        ControlComponent cc;

        string OCCUPIER_A = "A";
        string OCCUPIER_B = "B";
        string CC = "CC";

        [SetUp]
        public void Setup()
        {
            var OpModes = new Collection<IOperationMode>(){ new OperationMode("OpModeOne"), new OperationMode("OpModeTwo") };
            var orderOutputs = new Collection<OrderOutput>() 
            { 
                new OrderOutput("First", new ControlComponent("CC1", OpModes, new Collection<OrderOutput>(), new Collection<string>())),
                new OrderOutput("Second", new ControlComponent("CC2", OpModes, new Collection<OrderOutput>(), new Collection<string>()))
            };
            cc = new ControlComponent(CC, OpModes, orderOutputs, new Collection<string>());
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