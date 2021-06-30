using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
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
            orderOutput = new OrderOutput("First", new ControlComponent(CC, OpModes, new Collection<IOrderOutput>(), new Collection<string>()));
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
            orderOutput.Occupy(OCCUPIER_A);
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual(OCCUPIER_A, orderOutput.OCCUPIER);

            orderOutput.Free(OCCUPIER_A);
            Assert.AreEqual(true, orderOutput.IsFree());
        }

        [Test]
        public void Given_OccupiedByA_When_FreeByB_Then_OccupiedByA()
        {
            orderOutput.Occupy(OCCUPIER_A);
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual(OCCUPIER_A, orderOutput.OCCUPIER);

            orderOutput.Free(OCCUPIER_B);
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual(OCCUPIER_A, orderOutput.OCCUPIER);
        }

        [Test]
        public void Given_OccupiedByA_When_OccupyByB_Then_OccupiedByA()
        {
            orderOutput.Occupy(OCCUPIER_A);
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual(OCCUPIER_A, orderOutput.OCCUPIER);

            orderOutput.Occupy(OCCUPIER_B);
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual(OCCUPIER_A, orderOutput.OCCUPIER);
        }

        [Test]
        public void Given_OccupiedByA_When_PrioByB_Then_OccupiedByB()
        {
            orderOutput.Occupy(OCCUPIER_A);
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual(OCCUPIER_A, orderOutput.OCCUPIER);

            orderOutput.Prio(OCCUPIER_B);
            Assert.AreEqual(true, orderOutput.IsOccupied());
            Assert.AreEqual(OCCUPIER_B, orderOutput.OCCUPIER);
        }
    }
}