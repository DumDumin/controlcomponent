using System;
using System.Collections.Generic;
using System.Reflection;

namespace ControlComponents.Core
{

    internal static class ControlComponentReflection
    {
        public static TReturn ReadProperty<TReturn>(string targetRole, string propertyName, IControlComponent instance)
        {
            // TODO extend this to use all known IControlComponent properties to make the code faster
            if(propertyName == nameof(IControlComponent.EXST))
            {
                return (TReturn)(object)instance.EXST;
            }

            return (TReturn)instance.GetType().GetProperty(propertyName).GetValue(instance);
        }

        public static void CallMethod(string targetRole, string methodName, IControlComponent instance)
        {
            instance.GetType().GetMethod(methodName).Invoke(instance, new object[]{});
        }

        public static TReturn CallMethod<TReturn>(string targetRole, string methodName, IControlComponent instance)
        {
            // TODO extend this to use all known IControlComponent methods to make the code faster
            if(methodName == nameof(IControlComponent.IsFree))
            {
                return (TReturn)(object)instance.IsFree();
            }

            return (TReturn) instance.GetType().GetMethod(methodName).Invoke(instance, new object[]{});
        }

        public static void CallMethod<TParam>(string targetRole, string methodName, TParam param, IControlComponent instance)
        {
            instance.GetType().GetMethod(methodName).Invoke(instance, new object[]{param});
        }

        public static TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param, IControlComponent instance)
        {
            return (TReturn) instance.GetType().GetMethod(methodName).Invoke(instance, new object[]{param});
        }

        public static TReturn CallMethod<TParam1, TParam2, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2, IControlComponent instance)
        {
            return (TReturn) instance.GetType().GetMethod(methodName).Invoke(instance, new object[]{param1, param2});
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