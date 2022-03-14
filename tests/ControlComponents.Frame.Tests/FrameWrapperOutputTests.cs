using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ControlComponents.Core;
using ControlComponents.Core.Tests;
using FluentAssertions;
using Moq;
using NLog;
using NUnit.Framework;

namespace ControlComponents.Frame.Tests
{
    public class FrameWrapperOutputTests
    {
        string ROLE = "ROLE";
        string SENDER = "SENDER";
        string CC = "CC";
        string OpModeOne = "OpModeOne";
        string OpModeTwo = "OpModeTwo";
        ControlComponent ccWrapped;
        IOrderOutput outputWrapped;
        FrameWrapperOutput outputPointingToFrame;

        [SetUp]
        public void Setup()
        {
            ControlComponentProvider provider = new ControlComponentProvider();
            var CascadeOpModes = new Collection<IOperationMode>() { new OperationModeCascade(OpModeOne), new OperationModeCascade(OpModeTwo) };
            var OpModes = new Collection<IOperationMode>() { new OperationModeRaw(OpModeOne), new OperationModeRaw(OpModeTwo) };
            
            ControlComponent cc2 = new ControlComponent("CC2");
            ControlComponent cc1 = new ControlComponent("CC1");
            provider.Add("CC2", cc2);
            provider.Add("CC1", cc1);

            outputWrapped = new OrderOutput(ROLE, "CCWRAPPED", provider, cc1);
            ccWrapped = new ControlComponent("CCWRAPPED", OpModes, new Collection<IOrderOutput>(){outputWrapped}, new Collection<string>());
            
            var ccFrame = FrameControlComponent<ControlComponent>.Create("CCFRAME", ccWrapped, provider);

            outputPointingToFrame = new FrameWrapperOutput(ROLE, CC, provider, ccFrame);
        }

        [Test]
        public void When_ChangeOutputViaCallMethod_Then_OutputOfControlComponentChanged()
        {
            outputWrapped.ComponentName.Should().Be("CC1");
            bool success = outputPointingToFrame.ChangeOutput(ROLE, "CC2");
            success.Should().BeTrue();
            outputWrapped.ComponentName.Should().Be("CC2");
        }
        
        [Test]
        public async Task When_SubscribeAndUnsubcribe_Then_ReceiveCorrectEvents()
        {
            int i = 0;
            ExecutionStateEventHandler handler = (object sender, ExecutionStateEventArgs e) => i++;
            outputPointingToFrame.ExecutionStateChanged += handler;

            Task running = ccWrapped.SelectOperationMode(OpModeOne);
            await ccWrapped.ResetAndWaitForIdle(SENDER);

            outputPointingToFrame.ExecutionStateChanged -= handler;
            await ccWrapped.StopAndWaitForStopped(SENDER);
            await ccWrapped.DeselectOperationMode();

            i.Should().Be(2);
        }
    }
}