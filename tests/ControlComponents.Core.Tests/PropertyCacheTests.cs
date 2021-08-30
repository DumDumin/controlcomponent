using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ControlComponents.Core.Tests
{
    public class PropertyCacheTests
    {
        string ROLE = "ROLE";
        string OPMODE = "OPMODE";

        [Test, AutoData]
        public void Test_GetProperty(ControlComponent sut)
        {
            var propertyInfo = sut.GetType().GetProperty(nameof(sut.EXST));
            Func<ExecutionState> func = PropertyCache.BuildTypedGetter<ExecutionState>(propertyInfo, sut);
            func().Should().Be(ExecutionState.STOPPED);
        }

        [Test, AutoData]
        public void Test_CallMethod(ControlComponent sut)
        {
            sut.AddOperationMode(new OperationModeAsync(OPMODE));

            MethodInfo free = sut.GetType().GetMethod(nameof(sut.Reset));
            MethodInfo select = sut.GetType().GetMethod(nameof(sut.SelectOperationMode));

            var selectFunc = PropertyCache.BuildTypedFunc<string,Task>(select, sut);
            Task running = selectFunc(OPMODE);
            var freeFunc = PropertyCache.BuildTypedAction<string>(free, sut);
            freeFunc("SENDER");

            sut.EXST.Should().Be(ExecutionState.RESETTING);
        }

        [Test, AutoData]
        public void Test_Method_Delegate(ControlComponent sut)
        {
            MethodInfo free = typeof(ControlComponent).GetMethod(nameof(sut.IsFree));
            Func<ControlComponent, bool> d = (Func<ControlComponent, bool>)free.CreateDelegate(typeof(Func<ControlComponent, bool>));
            d(sut).Should().Be(true);

            d = (Func<ControlComponent, bool>)Delegate.CreateDelegate(typeof(Func<ControlComponent, bool>), free);
            d(sut).Should().Be(true);

            Func<bool> dd = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), sut, free);
            dd().Should().Be(true); 
        }

        [Test, AutoData]
        public void Test_Void_Method_Delegate(ControlComponent sut)
        {
            MethodInfo free = sut.GetType().GetMethod(nameof(sut.Reset));
            Action<string> a = free.CreateDelegate<Action<string>>(sut);

            a = (Action<string>) Delegate.CreateDelegate(typeof(Action<string>), sut, free);
        }

        [Test, AutoData]
        public void Test_Method_Delegate_Interface(ControlComponent sut)
        {
            IControlComponent sut_interface = sut;
            MethodInfo free = sut_interface.GetType().GetMethod(nameof(sut_interface.IsFree));
            Func<bool> d = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), sut_interface, free);
            d().Should().Be(true);

            // This is not working
            // Func<IControlComponent,bool> dd = (Func<IControlComponent,bool>) Delegate.CreateDelegate(typeof(Func<IControlComponent,bool>), free);
            // dd(sut_interface).Should().Be(true);

            Func<bool> dd = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), sut_interface, free);
            dd().Should().Be(true);
        }

        [Test, AutoData]
        public void Test_Property_Delegate(ControlComponent sut)
        {
            PropertyInfo EXST = typeof(ControlComponent).GetProperty(nameof(sut.EXST));
            MethodInfo EXST_get = EXST.GetGetMethod();
            Func<ControlComponent, ExecutionState> d = (Func<ControlComponent, ExecutionState>)EXST_get.CreateDelegate(typeof(Func<ControlComponent, ExecutionState>));
            d(sut).Should().Be(ExecutionState.STOPPED);

            d = (Func<ControlComponent, ExecutionState>)Delegate.CreateDelegate(typeof(Func<ControlComponent, ExecutionState>), EXST_get);
            d(sut).Should().Be(ExecutionState.STOPPED);

            Func<ExecutionState> dd = (Func<ExecutionState>)Delegate.CreateDelegate(typeof(Func<ExecutionState>), sut, EXST_get);
            dd().Should().Be(ExecutionState.STOPPED);
        }

        public interface TestValueA{
            int Test { get; }
        }
        public interface TestValueB{
            int Test { get; }
        }

        public class TestImplementation : TestValueA, TestValueB
        {
            int TestValueA.Test => 0;

            int TestValueB.Test => 1;
        }


        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation
        [Test, AutoData]
        public void Test_Explicit(TestImplementation sut)
        {
            TestValueA testA = sut;
            testA.Test.Should().Be(0);
            TestValueB testB = sut;
            testB.Test.Should().Be(1);
        }
    }
}
