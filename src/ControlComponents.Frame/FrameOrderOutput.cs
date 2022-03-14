using ControlComponents.Core;

namespace ControlComponents.Frame
{
    public class FrameOrderOutput : OrderOutput, IFrameControlComponent
    {
        public FrameOrderOutput(string role, string id, IControlComponentProvider provider) : base(role, id, provider) { }

        public FrameOrderOutput(string role, string id, IControlComponentProvider provider, IControlComponent cc) : base(role, id, provider, cc) { }

        public TReturn ReadProperty<TReturn>(string targetRole, string propertyName)
        {
            return ControlComponentReflection.ReadProperty<TReturn>(targetRole, propertyName, controlComponent);
        }

        public void CallMethod(string targetRole, string methodName)
        {
            ControlComponentReflection.CallMethod(targetRole, methodName, controlComponent);
        }

        public void CallMethod<TParam>(string targetRole, string methodName, TParam param)
        {
            ControlComponentReflection.CallMethod<TParam>(targetRole, methodName, param, controlComponent);
        }

        public TReturn CallMethod<TReturn>(string targetRole, string methodName)
        {
            return ControlComponentReflection.CallMethod<TReturn>(targetRole, methodName, controlComponent);
        }

        public TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param)
        {
            return ControlComponentReflection.CallMethod<TParam, TReturn>(targetRole, methodName, param, controlComponent);
        }

        public TReturn CallMethod<TParam1, TParam2, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2)
        {
            return ControlComponentReflection.CallMethod<TParam1, TParam2, TReturn>(targetRole, methodName, param1, param2, controlComponent);
        }

        public void Subscribe<THandler>(string targetRole, string eventName, THandler eventHandler)
        {
            ControlComponentReflection.Subscribe<THandler>(targetRole, eventName, eventHandler, controlComponent);
        }

        public void Unsubscribe<THandler>(string targetRole, string eventName, THandler eventHandler)
        {
            ControlComponentReflection.Unsubscribe<THandler>(targetRole, eventName, eventHandler, controlComponent);
        }

    }
}