using System;
using System.Reflection;

namespace ControlComponents.Core
{
    // https://web.archive.org/web/20141020092917/http://flurfunk.sdx-ag.de/2012/05/c-performance-bei-der-befullungmapping.html
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
    }
}