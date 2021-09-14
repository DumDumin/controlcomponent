using System;
using System.Linq;
using System.Reflection;

namespace ControlComponents.Core
{

    internal static class ControlComponentReflection
    {
        public static TReturn ReadProperty<TReturn>(string targetRole, string propertyName, IControlComponent instance)
        {
            // TODO extend this to use all known IControlComponent properties to make the code faster
            if (propertyName == nameof(IControlComponent.EXST))
            {
                return (TReturn)(object)instance.EXST;
            }

            return (TReturn)instance.GetType().GetProperty(propertyName).GetValue(instance);
        }

        public static void CallMethod(string targetRole, string methodName, IControlComponent instance)
        {
            instance.GetType().GetMethod(methodName).Invoke(instance, new object[] { });
        }

        public static TReturn CallMethod<TReturn>(string targetRole, string methodName, IControlComponent instance)
        {
            // TODO extend this to use all known IControlComponent methods to make the code faster
            if (methodName == nameof(IControlComponent.IsFree))
            {
                return (TReturn)(object)instance.IsFree();
            }
            else
            {
                return (TReturn)instance.GetType().GetMethod(methodName).Invoke(instance, new object[] { });
            }
        }

        public static void CallMethod<TParam>(string targetRole, string methodName, TParam param, IControlComponent instance)
        {
            instance.GetType().GetMethod(methodName).Invoke(instance, new object[] { param });
        }

        public static void CallMethod<TParam1, TParam2>(string targetRole, string methodName, TParam1 param1, TParam2 param2, IControlComponent instance)
        {
            Type type = instance.GetType();
            var methods = type.GetMethods().Where(m => m.Name == methodName);
            MethodInfo method = methods.First(m => !m.IsGenericMethod);
            method.Invoke(instance, new object[] { param1, param2 });
        }

        public static TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param, IControlComponent instance)
        {
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(methodName);
            return (TReturn)method.Invoke(instance, new object[] { param });
        }

        public static TReturn CallMethod<TParam1, TParam2, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2, IControlComponent instance)
        {
            return (TReturn)instance.GetType().GetMethod(methodName).Invoke(instance, new object[] { param1, param2 });
        }

        public static void CallMethodGeneric<TParam1, TParam2, TParam3>(string targetRole, string methodName, TParam1 param1, TParam2 param2, TParam3 param3, IControlComponent instance)
        {
            Type type = instance.GetType();
            MethodInfo method = type.GetMethods().Where(m => m.Name == methodName && m.GetParameters().Length == 3).First();
            MethodInfo generic = method.MakeGenericMethod(typeof(TParam3));
            generic.Invoke(instance, new object[] { param1, param2, param3 });
        }
        
        public static TReturn CallMethodGeneric<TParam1, TParam2, TParam3, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2, TParam3 param3, IControlComponent instance)
        {
            Type type = instance.GetType();
            MethodInfo method = type.GetMethods().Where(m => m.Name == methodName && m.GetParameters().Length == 3 && m.ReturnType != typeof(void)).First();
            MethodInfo generic = method.MakeGenericMethod(typeof(TParam3), typeof(TReturn));
            return (TReturn)generic.Invoke(instance, new object[] { param1, param2, param3 });
        }

        public static TReturn CallMethodGeneric<TParam1, TParam2, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2, IControlComponent instance)
        {
            var type = instance.GetType();
            var method = type.GetMethods().Where(m => m.Name == methodName && m.GetParameters().Length == 2 && m.ReturnType != typeof(void)).First();
            var generic = method.MakeGenericMethod(typeof(TReturn));
            var result = generic.Invoke(instance, new object[] { param1, param2 });
            return (TReturn)result;
        }

        public static void Subscribe<T>(string targetRole, string eventName, T eventHandler, IControlComponent instance)
        {
            EventInfo eventInfo = instance.GetType().GetEvent(eventName);
            Action<T> addEventHandler = (Action<T>)eventInfo.GetAddMethod().CreateDelegate(typeof(Action<T>), instance);
            addEventHandler(eventHandler);
        }
        public static void Unsubscribe<T>(string targetRole, string eventName, T eventHandler, IControlComponent instance)
        {
            EventInfo eventInfo = instance.GetType().GetEvent(eventName);
            Action<T> removeEventHandler = (Action<T>)eventInfo.GetRemoveMethod().CreateDelegate(typeof(Action<T>), instance);
            removeEventHandler(eventHandler);
        }
    }
}