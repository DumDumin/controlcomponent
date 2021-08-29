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

        // public static Action<T, object> BuildUntypedSetter<T>(PropertyInfo propertyInfo)
        // {
        //     var targetType = propertyInfo.DeclaringType;
        //     var methodInfo = propertyInfo.GetSetMethod();
        //     var exTarget = Expression.Parameter(targetType, "t");
        //     var exValue = Expression.Parameter(typeof(object), "p");
        //     // wir betreiben ein anObject.SetPropertyValue(object)
        //     var exBody = Expression.Call(exTarget, methodInfo,

        //                                Expression.Convert(exValue, propertyInfo.PropertyType));

        //     var lambda = Expression.Lambda<Action<T, object>>(exBody, exTarget, exValue);
        //     // (t, p) => t.set_StringValue(Convert(p))

        //     var action = lambda.Compile();
        //     return action;
        // }



        // public static Func<T, object> BuildUntypedGetter<T>(PropertyInfo propertyInfo)
        // {
        //     var targetType = propertyInfo.ReflectedType;
        //     var methodInfo = propertyInfo.GetGetMethod();
        //     var returnType = methodInfo.ReturnType;

        //     var exTarget = Expression.Parameter(targetType, "t");
        //     var exBody = Expression.Call(exTarget, methodInfo);
        //     var exBody2 = Expression.Convert(exBody, typeof(object));

        //     var lambda = Expression.Lambda<Func<T, object>>(exBody2, exTarget);

        //     // t => Convert(t.get_Foo())

        //     var action = lambda.Compile();
        //     return action;
        // }
    }
}