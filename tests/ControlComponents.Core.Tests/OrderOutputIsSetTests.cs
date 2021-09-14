using System.Collections.ObjectModel;
using NUnit.Framework;
using Moq;
using NLog;
using FluentAssertions;

namespace ControlComponents.Core.Tests
{
    public class OrderOutputIsSetTests
    {
        string ROLE = "ROLE";
        string SENDER = "SENDER";
        string CC = "CC";
        string OpModeOne = "OpModeOne";
        string OpModeTwo = "OpModeTwo";
        ControlComponent cc;
        IOrderOutput output;
        IOrderOutput outputOne;
        IOrderOutput outputTwo;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: Console
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            logconsole.Layout = new NLog.Layouts.SimpleLayout("${longdate} ${message} ${exception:format=ToString}");
            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        [SetUp]
        public void Setup()
        {
            ControlComponentProvider provider = new ControlComponentProvider();
            var CascadeOpModes = new Collection<IOperationMode>() { new OperationModeCascade(OpModeOne), new OperationModeCascade(OpModeTwo) };
            var OpModes = new Collection<IOperationMode>() { new OperationModeRaw(OpModeOne), new OperationModeRaw(OpModeTwo) };

            var cc1 = new ControlComponent("CC1", OpModes, new Collection<IOrderOutput>(), new Collection<string>());
            var cc2 = new ControlComponent("CC2", OpModes, new Collection<IOrderOutput>(), new Collection<string>());
            provider.Add("CC1", cc1);
            provider.Add("CC2", cc2);

            outputOne = new OrderOutput("ROLE_ONE", CC, provider, cc1);
            outputTwo = new OrderOutput("ROLE_TWO", CC, provider, cc2);
            Collection<IOrderOutput> orderOutputs = new Collection<IOrderOutput>()
            {
                outputOne,
                outputTwo
            };
            cc = new ControlComponent(CC, CascadeOpModes, orderOutputs, new Collection<string>());
            output = new OrderOutput(ROLE, CC, provider, cc);
        }

        [Test]
        public void When_OCCUPIER_Then_Return()
        {
            output.OCCUPIER.Should().Be("NONE");
        }
        [Test]
        public void When_ComponentName_Then_Return()
        {
            output.ComponentName.Should().Be(CC);
        }
        [Test]
        public void When_WORKST_Then_Return()
        {
            output.WORKST.Should().Be("BSTATE");
        }

        [Test]
        public void When_ChangeOutput_Then_OutputOfControlComponentChanged()
        {
            output.ChangeOutput("ROLE_ONE", "CC2");
            outputOne.ComponentName.Should().Be("CC2");
            output.ChangeOutput("ROLE_TWO", "CC1");
            outputTwo.ComponentName.Should().Be("CC1");
        }
        [Test]
        public void When_ClearOutput_Then_OutputOfControlComponentCleared()
        {
            output.ClearOutput("ROLE_ONE");
            outputOne.IsSet.Should().BeFalse();
        }

        [Test]
        public void When_ChangeOutputViaCallMethod_Then_OutputOfControlComponentChanged()
        {
            bool success = output.CallMethod<string,string,bool>("", nameof(output.ChangeOutput), "ROLE_ONE", "CC2");
            success.Should().BeTrue();
            outputOne.ComponentName.Should().Be("CC2");
        }
    }
}