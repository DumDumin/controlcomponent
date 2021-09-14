using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class ControlComponentNoSelectedOpModeTests
    {
        string SENDER = "SENDER";
        string CC = "CC";
        string OpModeOne = "OpModeOne";
        string OpModeTwo = "OpModeTwo";
        ControlComponent cc;
        Mock<IControlComponentProvider> provider;

        [SetUp]
        public void Setup()
        {
            provider = new Mock<IControlComponentProvider>();
            var OpModes = new Collection<IOperationMode>(){ new OperationModeRaw(OpModeOne), new OperationModeRaw(OpModeTwo) };
            var orderOutputs = new Collection<IOrderOutput>() 
            { 
                new OrderOutput("First", CC, provider.Object, new ControlComponent("CC1", OpModes, new Collection<IOrderOutput>(), new Collection<string>())),
                new OrderOutput("Second", CC, provider.Object, new ControlComponent("CC2", OpModes, new Collection<IOrderOutput>(), new Collection<string>()))
            };
            cc = new ControlComponent(CC, OpModes, orderOutputs, new Collection<string>());
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public void Given_Stopped_When_AddNewOpMode_Then_Added()
        {
            var newOpMode = new OperationModeRaw("NewOpMode");
            cc.AddOperationMode(newOpMode);

            Assert.True(cc.OpModes.Contains(newOpMode.OpModeName));
        }

        [Test]
        public void Given_Stopped_When_AddNewOpMode_Then_Added_2()
        {
            cc = new ControlComponent(CC);
            var newOpMode = new OperationModeRaw("NewOpMode");
            cc.AddOperationMode(newOpMode);

            Assert.True(cc.OpModes.Contains(newOpMode.OpModeName));
        }

        [Test]
        public void Given_Stopped_When_AddNewOutput_Then_Added()
        {
            var newOrderOutput = new OrderOutput("OrderOutput", CC, provider.Object);
            cc.AddOrderOutput(newOrderOutput);

            Assert.True(cc.Roles.Contains(newOrderOutput.Role));
        }

        [Test]
        public void Given_NotEqualId_When_AddNewOrderOutput_Then_Throw()
        {
            var newOrderOutput = new OrderOutput("OrderOutput", "Output", provider.Object);
            Assert.Throws(typeof(ArgumentException), () => cc.AddOrderOutput(newOrderOutput));
        }

        [Test]
        public void Given_NotEqualOutputId_When_CreateCC_Then_Throw()
        {
            var newOrderOutput = new OrderOutput("OrderOutput", "Output", provider.Object);
            Assert.Throws(
                typeof(ArgumentException), 
                () => new ControlComponent("CC", new Collection<IOperationMode>(), new Collection<IOrderOutput>(){newOrderOutput}, new Collection<string>()));
        }

        [Test]
        public void Given_NewControlComponent_When_EXST_Then_Stopped()
        {
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public void Given_Stopped_When_OpModeName_Then_None()
        {
            Assert.AreEqual("NONE", cc.OpModeName);
        }

        [Test]
        public void Given_Stopped_When_ComponentName_Then_CC()
        {
            Assert.AreEqual(CC, cc.ComponentName);
        }

        [Test]
        public void Given_OpModes_When_ListOpModeNames_Then_ReturnOpModeNames()
        {
            Assert.AreEqual(new Collection<string>(){OpModeOne, OpModeTwo}, cc.OpModes);
        }

        [Test]
        public async Task Given_Stopped_When_SelectOpMode_Then_NewOpMode()
        {
            Task runningOpMode = cc.SelectOperationMode(OpModeOne);
            Assert.AreEqual(OpModeOne, cc.OpModeName);

            await cc.DeselectOperationMode();
            await runningOpMode;
            Assert.AreEqual("NONE", cc.OpModeName);
        }

        [Test]
        [Ignore("Cannot be tested, because this state is not reachable")]
        public async Task Given_Idle_When_SelectOpMode_Then_Throw()
        {
            Task runningOpMode = cc.SelectOperationMode(OpModeOne);
            await cc.ResetAndWaitForIdle(SENDER);

            await Task.WhenAll(
                cc.SelectOperationMode(OpModeOne),
                cc.SelectOperationMode(OpModeTwo)
            );
            // cc.Invoking(async c => await c.SelectOperationMode(OpModeTwo)).Should().Throw<InvalidOperationException>();

            await cc.StopAndWaitForStopped(SENDER);
            await cc.DeselectOperationMode();
            await runningOpMode;
            Assert.AreEqual("NONE", cc.OpModeName);
        }

        [Test]
        public void Given_NoOperationModeSelected_When_Deselect_Then_DoNotThrow()
        {
            Assert.DoesNotThrowAsync(() => cc.DeselectOperationMode());
        }

        [Test]
        public void Given_NoOperationModeSelected_When_UserActions_Then_Throw()
        {
            InvalidOperationException e = Assert.Throws<InvalidOperationException>(() => cc.Reset(SENDER));
            Assert.AreEqual($"{cc.ComponentName} cannot change to {ExecutionState.RESETTING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Start(SENDER));
            Assert.AreEqual($"{cc.ComponentName} cannot change to {ExecutionState.STARTING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Stop(SENDER));
            Assert.AreEqual($"{cc.ComponentName} cannot change to {ExecutionState.STOPPING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Suspend(SENDER));
            Assert.AreEqual($"{cc.ComponentName} cannot change to {ExecutionState.SUSPENDING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Unsuspend(SENDER));
            Assert.AreEqual($"{cc.ComponentName} cannot change to {ExecutionState.UNSUSPENDING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Hold(SENDER));
            Assert.AreEqual($"{cc.ComponentName} cannot change to {ExecutionState.HOLDING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Unhold(SENDER));
            Assert.AreEqual($"{cc.ComponentName} cannot change to {ExecutionState.UNHOLDING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Abort(SENDER));
            Assert.AreEqual($"{cc.ComponentName} cannot change to {ExecutionState.ABORTING}, if no operation mode is selected", e.Message);
            e = Assert.Throws<InvalidOperationException>(() => cc.Clear(SENDER));
            Assert.AreEqual($"{cc.ComponentName} cannot change to {ExecutionState.CLEARING}, if no operation mode is selected", e.Message);
        }

        // [Test]
        // public void Given_Stopped_When_
    }
}