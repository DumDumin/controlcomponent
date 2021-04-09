using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class OrderOutputOccupationTests
    {
        OrderOutput orderOutput;

        string OCCUPIER_A = "A";
        string OCCUPIER_B = "B";
        string CC = "CC";

        [SetUp]
        public void Setup()
        {
            var OpModes = new Collection<IOperationMode>(){ new OperationMode("OpModeOne"), new OperationMode("OpModeTwo") };
            orderOutput = new OrderOutput("First", new ControlComponent(CC, OpModes, new Collection<OrderOutput>(), new Collection<string>()));
        }

        [Test]
        public void Given_Free_When_Occupier_Then_None()
        {
            Assert.AreEqual("NONE", orderOutput.OCCUPIER);
        }

        [Test]
        public void Given_Free_When_IsFree_Then_True()
        {
            Assert.AreEqual(true, orderOutput.IsFree());
        }

        [Test]
        public void Given_Free_When_Occupy_Then_Occupied()
        {
            orderOutput.Occupy("TEST");
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual("TEST", orderOutput.OCCUPIER);
        }

        [Test]
        public void Given_OccupiedByA_When_FreeByA_Then_IsFree()
        {
            orderOutput.Occupy("A");
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual("A", orderOutput.OCCUPIER);

            orderOutput.Free("A");
            Assert.AreEqual(true, orderOutput.IsFree());
        }

        [Test]
        public void Given_OccupiedByA_When_FreeByB_Then_OccupiedByA()
        {
            orderOutput.Occupy("A");
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual("A", orderOutput.OCCUPIER);

            orderOutput.Free("B");
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual("A", orderOutput.OCCUPIER);
        }

        [Test]
        public void Given_OccupiedByA_When_OccupyByB_Then_OccupiedByA()
        {
            orderOutput.Occupy("A");
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual("A", orderOutput.OCCUPIER);

            orderOutput.Occupy("B");
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual("A", orderOutput.OCCUPIER);
        }
    }
}