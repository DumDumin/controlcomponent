using System;
using System.Reflection;

namespace ControlComponents.Core
{
    public static class PropertyCache
    {
        public static Func<TReturn> BuildTypedGetter<TReturn>(PropertyInfo propertyInfo, object instance)
        {
            MethodInfo methodInfo = propertyInfo.GetGetMethod();
            var reflGet = Delegate.CreateDelegate(typeof(Func<TReturn>), instance, methodInfo);
            return (Func<TReturn>)reflGet;
        }

        public static Func<TReturn> BuildTypedFunc<TReturn>(MethodInfo methodInfo, object instance)
        {
            var reflGet = Delegate.CreateDelegate(typeof(Func<TReturn>), instance, methodInfo);
            return (Func<TReturn>)reflGet;
        }
        public static Func<TParam, TReturn> BuildTypedFunc<TParam, TReturn>(MethodInfo methodInfo, object instance)
        {
            var reflGet = Delegate.CreateDelegate(typeof(Func<TParam, TReturn>), instance, methodInfo);
            return (Func<TParam, TReturn>)reflGet;
        }

        public static Action<TParam> BuildTypedAction<TParam>(MethodInfo methodInfo, object instance)
        {
            var reflGet = Delegate.CreateDelegate(typeof(Action<TParam>), instance, methodInfo);
            return (Action<TParam>)reflGet;
        }
        public static Action BuildTypedAction(MethodInfo methodInfo, object instance)
        {
            var reflGet = Delegate.CreateDelegate(typeof(Action), instance, methodInfo);
            return (Action)reflGet;
        }

        // public static Action<T, TProperty> BuildTypedSetter<T, TProperty>(PropertyInfo propertyInfo)
        // {
        //     Action<T, TProperty> reflSet = (Action<T, TProperty>)Delegate.CreateDelegate(typeof(Action<T, TProperty>), propertyInfo.GetSetMethod());
        //     return reflSet;
        // }
    }
}