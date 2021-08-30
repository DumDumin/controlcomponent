using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class ControlComponentServiceInterfaceTests
    {
        string CC = "CC";
        string SENDER = "SENDER";
        string OPMODE = "OPMODE";
        string ROLE = "ROLE";

        [Test, AutoData]
        public void Given_CC_When_GetProperty_Then_Return(ControlComponent sut)
        {
            ExecutionState result = sut.ReadProperty<ExecutionState>(ROLE, nameof(IControlComponent.EXST));
            result.Should().Be(sut.EXST);
        }

        [Test, AutoData]
        public void Given_CC_When_CallMethod_Then_Return(ControlComponent sut)
        {
            bool result = sut.CallMethod<bool>(ROLE, nameof(IControlComponent.IsFree));
            result.Should().Be(true);
        }

        [Test, AutoData]
        public async Task Given_CC_When_CallMethodWithParam_Then_Return(ControlComponent sut)
        {
            sut.AddOperationMode(new OperationModeAsync(OPMODE));

            Task running = sut.CallMethod<string, Task>(ROLE, nameof(IControlComponent.SelectOperationMode), OPMODE);
            sut.CallMethod<string>(ROLE, nameof(IControlComponent.Reset), "SENDER");
            sut.EXST.Should().Be(ExecutionState.RESETTING);
            sut.CallMethod<string>(ROLE, nameof(IControlComponent.Stop), "SENDER");
            ExecutionState exst = sut.ReadProperty<ExecutionState>(ROLE, nameof(IControlComponent.EXST));

            await sut.WaitForStopped();
            // TODO StopAndWait is not usable in this context => create new tests with 3 components to emulate correct behaviour
            // while (exst != ExecutionState.STOPPED)
            // {
            //     await Task.Delay(1);
            //     exst = sut.ReadProperty<ExecutionState>(ROLE, nameof(IControlComponent.EXST));
            // }

            await sut.CallMethod<Task>(ROLE, nameof(IControlComponent.DeselectOperationMode));
            await running;
        }

        [Test, AutoData]
        [Ignore("Test Timings")]
        public void Test_Timings(ControlComponent sut)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ExecutionState test;
            // bool test;
            for (int i = 0; i < 10000000; i++)
            {
                // test = sut.EXST; // 0.06 sec
                // test = sut.ReadProperty<ExecutionState>("", nameof(sut.EXST)); // 0.51 sec
                // test = sut.ReadPropertyyy<ExecutionState>("", nameof(sut.EXST)); // 1.88 sec

                //test = sut.CallMethod<bool>("", nameof(sut.IsFree)); // 0.46 sec   (without cache was 4.87 sec)
                // test = sut.IsFree(); // 0.09 sec
            }

            sw.Stop();

            System.Console.WriteLine("Elapsed={0}",sw.Elapsed);
        }
    }
}
