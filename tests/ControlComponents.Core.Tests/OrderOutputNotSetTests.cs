using System.Collections.ObjectModel;
using NUnit.Framework;
using Moq;
using NLog;
using FluentAssertions;

namespace ControlComponents.Core.Tests
{
    public class OrderOutputNotSetTests
    {
        string ROLE = "ROLE";
        string SENDER = "SENDER";
        string CC = "CC";
        string OpModeOne = "OpModeOne";
        IOrderOutput output;

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
            var provider = new Mock<IControlComponentProvider>();
            output = new OrderOutput(ROLE, CC, provider.Object);
        }

        [Test]
        public void When_OCCUPIER_Then_Throw()
        {
            output.Invoking(o => o.OCCUPIER)
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_ComponentName_Then_Throw()
        {
            output.Invoking(o => o.ComponentName)
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_WORKST_Then_Throw()
        {
            output.Invoking(o => o.WORKST)
                .Should().Throw<OrderOutputException>();
        }


        [Test]
        public void When_SelectOperationMode_Then_Throw()
        {
            output.Invoking(o => o.SelectOperationMode(OpModeOne))
                .Should().Throw<OrderOutputException>();
        }

        [Test]
        public void When_DeselectOperationMode_Then_TaskCompleted()
        {
            output.Invoking(o => o.DeselectOperationMode())
                .Should().NotThrow();
        }

        [Test]
        public void When_Reset_Then_Throw()
        {
            output.Invoking(o => o.Reset(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Start_Then_Throw()
        {
            output.Invoking(o => o.Start(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Stop_Then_Throw()
        {
            output.Invoking(o => o.Stop(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Suspend_Then_Throw()
        {
            output.Invoking(o => o.Suspend(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Unsuspend_Then_Throw()
        {
            output.Invoking(o => o.Unsuspend(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Hold_Then_Throw()
        {
            output.Invoking(o => o.Hold(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Unhold_Then_Throw()
        {
            output.Invoking(o => o.Unhold(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Abort_Then_Throw()
        {
            output.Invoking(o => o.Abort(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Clear_Then_Throw()
        {
            output.Invoking(o => o.Clear(CC))
                .Should().Throw<OrderOutputException>();
        }

        ////////////////////

        [Test]
        public void When_ReadProperty_Then_Throw()
        {
            output.Invoking(o => o.ReadProperty<int>(CC, CC))
                .Should().Throw<OrderOutputException>();
        }

        [Test]
        public void When_CallMethod_Then_Throw()
        {
            output.Invoking(o => o.CallMethod(CC, CC))
                .Should().Throw<OrderOutputException>();

            output.Invoking(o => o.CallMethod<int>(CC, CC, 1))
                .Should().Throw<OrderOutputException>();

            output.Invoking(o => o.CallMethod<int>(CC, CC))
                .Should().Throw<OrderOutputException>();

            output.Invoking(o => o.CallMethod<int, int>(CC, CC, 1))
                .Should().Throw<OrderOutputException>();

            output.Invoking(o => o.CallMethod<int, int, int>(CC, CC, 1, 1))
                .Should().Throw<OrderOutputException>();
        }

        [Test]
        public void When_ChangeOutput_Then_Throw()
        {
            output.Invoking(o => o.ChangeOutput(CC, CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_ClearOutput_Then_Throw()
        {
            output.Invoking(o => o.ClearOutput(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Auto_Then_Throw()
        {
            output.Invoking(o => o.Auto(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_SemiAuto_Then_Throw()
        {
            output.Invoking(o => o.SemiAuto(CC))
                .Should().Throw<OrderOutputException>();
        }

        [Test]
        public void When_Occupy_Then_Throw()
        {
            output.Invoking(o => o.Occupy(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Prio_Then_Throw()
        {
            output.Invoking(o => o.Prio(CC))
                .Should().Throw<OrderOutputException>();
        }
        [Test]
        public void When_Free_Then_Throw()
        {
            output.Invoking(o => o.Free(CC))
                .Should().Throw<OrderOutputException>();
        }

        [Test]
        public void When_EXST_Then_Throw()
        {
            output.Invoking(o => o.EXST)
                .Should().Throw<OrderOutputException>();
        }

        [Test]
        public void When_EXMODE_Then_Throw()
        {
            output.Invoking(o => o.EXMODE)
                .Should().Throw<OrderOutputException>();
        }

        [Test]
        public void When_OpModeName_Then_Throw()
        {
            output.Invoking(o => o.OpModeName)
                .Should().Throw<OrderOutputException>();
        }

        [Test]
        public void When_OpModes_Then_Throw()
        {
            output.Invoking(o => o.OpModes)
                .Should().Throw<OrderOutputException>();
        }

        [Test]
        public void When_Roles_Then_Throw()
        {
            output.Invoking(o => o.Roles)
                .Should().Throw<OrderOutputException>();
        }
    }
}