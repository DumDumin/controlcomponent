using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        [SetUp]
        public void Setup()
        {
            var OpModes = new Collection<IOperationMode>(){ new OperationMode(OpModeOne), new OperationMode(OpModeTwo) };
            var orderOutputs = new Collection<OrderOutput>() 
            { 
                new OrderOutput("First", new ControlComponent("CC1", OpModes, new Collection<OrderOutput>(), new Collection<string>())),
                new OrderOutput("Second", new ControlComponent("CC2", OpModes, new Collection<OrderOutput>(), new Collection<string>()))
            };
            cc = new ControlComponent(CC, OpModes, orderOutputs, new Collection<string>());
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }

        [Test]
        public void Given_Stopped_When_AddNewOpMode_Then_Added()
        {
            var newOpMode = new OperationMode("NewOpMode");
            cc.AddOperationMode(newOpMode);

            Assert.True(cc.OpModes.Contains(newOpMode.OpModeName));
        }

        [Test]
        public void Given_Stopped_When_AddNewOutput_Then_Added()
        {
            var newOrderOutput = new OrderOutput("OrderOutput");
            cc.AddOrderOutput(newOrderOutput);

            Assert.True(cc.Roles.Contains(newOrderOutput.Role));
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

        // [Test]
        // public void Given_OrderOuputs_When_ListRoles_Then_ReturnRoles()
        // {
        //     Assert.AreEqual(new List<string>(){"OrderOuputOne", "OrderOutput2"}, cc.Roles);
        // }

        [Test]
        public async Task Given_Stopped_When_SelectOpMode_Then_NewOpMode()
        {
            // var newOpMode = new OperationMode("NEWOPMODE");
            Task runningOpMode = cc.SelectOperationMode(OpModeOne);
            Assert.AreEqual(OpModeOne, cc.OpModeName);

            await cc.DeselectOperationMode();
            await runningOpMode;
            Assert.AreEqual("NONE", cc.OpModeName);
        }

        [Test]
        public void Given_Stopped_When_Deselect_Then_Throw()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => cc.DeselectOperationMode());
        }

        [Test]
        public void Given_Stopped_When_UserActions_Then_Throw()
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