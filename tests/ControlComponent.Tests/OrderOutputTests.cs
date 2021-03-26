using System;
using System.Threading;
using NUnit.Framework;

namespace ControlComponent.Tests
{
    public class OrderOutputTests
    {
        string ROLE = "ROLE";

        // [SetUp]
        // public void Setup()
        // {
        //     operationMode = new OperationMode("DEFAULT");
        //     tokenOwner = new CancellationTokenSource();

        //     called = false;
        //     operationMode.OnTaskDone += OnTaskDone;
        // }

        // [TearDown]
        // public void TearDown()
        // {
        //     operationMode.OnTaskDone -= OnTaskDone;
        // }

        [Test]
        public void Given_OrderOutput_When_Role_Then_Role()
        {
            OrderOutput orderOutput = new OrderOutput(ROLE);
            Assert.AreEqual(ROLE, orderOutput.Role);
        }

        [Test]
        public void Given()
        {
            
        }
    }
}