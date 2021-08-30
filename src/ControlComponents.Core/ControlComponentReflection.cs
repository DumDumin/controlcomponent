using System;
using System.Collections.Generic;
namespace ControlComponents.Core
{

    internal static class ControlComponentReflection
    {
        static IDictionary<string, object> methodCache = new Dictionary<string, object>();
        static IDictionary<string, object> propertyCache = new Dictionary<string, object>();

        public static TReturn ReadProperty<TReturn>(string targetRole, string propertyName, IControlComponent instance)
        {
            var propertyId = propertyName + targetRole + instance.ComponentName;
            if (!propertyCache.ContainsKey(propertyId))
            {
                // do not use typeof, but GetType to get dynamic type
                var propertyInfo = instance.GetType().GetProperty(propertyName);
                Func<TReturn> propertyDelegate = PropertyCache.BuildTypedGetter<TReturn>(propertyInfo, instance);
                propertyCache.Add(propertyId, propertyDelegate);
            }

            return (propertyCache[propertyId] as Func<TReturn>).Invoke();
        }

        public static void CallMethod(string targetRole, string methodName, IControlComponent instance)
        {
            var methodId = methodName + targetRole + instance.ComponentName;
            if (!methodCache.ContainsKey(methodId))
            {
                var methodInfo = instance.GetType().GetMethod(methodName);
                Action propertyDelegate = PropertyCache.BuildTypedAction(methodInfo, instance);
                methodCache.Add(methodId, propertyDelegate);
            }

            (methodCache[methodId] as Action).Invoke();
        }

        public static TReturn CallMethod<TReturn>(string targetRole, string methodName, IControlComponent instance)
        {
            var methodId = methodName + targetRole + instance.ComponentName;
            if (!methodCache.ContainsKey(methodId))
            {
                var methodInfo = instance.GetType().GetMethod(methodName);
                Func<TReturn> propertyDelegate = PropertyCache.BuildTypedFunc<TReturn>(methodInfo, instance);
                methodCache.Add(methodId, propertyDelegate);
            }

            return (methodCache[methodId] as Func<TReturn>).Invoke();
        }

        public static void CallMethod<TParam>(string targetRole, string methodName, TParam param, IControlComponent instance)
        {
            var methodId = methodName + targetRole + instance.ComponentName;
            if (!methodCache.ContainsKey(methodId))
            {
                var methodInfo = instance.GetType().GetMethod(methodName);
                Action<TParam> propertyDelegate = PropertyCache.BuildTypedAction<TParam>(methodInfo, instance);
                methodCache.Add(methodId, propertyDelegate);
            }

            (methodCache[methodId] as Action<TParam>).Invoke(param);
        }

        public static TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param, IControlComponent instance)
        {
            var methodId = methodName + targetRole + instance.ComponentName;
            if (!methodCache.ContainsKey(methodId))
            {
                var methodInfo = instance.GetType().GetMethod(methodName);
                Func<TParam, TReturn> propertyDelegate = PropertyCache.BuildTypedFunc<TParam, TReturn>(methodInfo, instance);
                methodCache.Add(methodId, propertyDelegate);
            }

            return (methodCache[methodId] as Func<TParam, TReturn>).Invoke(param);
        }

    }
}