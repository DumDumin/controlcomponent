using System;

namespace ControlComponents.Core
{
    public class FrameControlComponent<T> : ControlComponent where T : IControlComponent
    {
        // This component includes the external deployed operation mode
        protected readonly T externalControlComponent;

        // TODO default name should not be used -> Component names must be unique
        public FrameControlComponent(T cc, IControlComponentProvider provider, string name = "FrameControlComponent") : base(name)
        {
            this.externalControlComponent = cc;

            // TODO create ExternalOpmodeOutput to inject it into opmode
            var output = new OrderOutput("ExternalOperationMode", ComponentName, provider, cc);
            AddOrderOutput(output);

            foreach (var operationModeName in cc.OpModes)
            {
                AddOperationMode(new ConfigOperationMode(operationModeName, output));
            }
        }

        public override TReturn ReadProperty<TReturn>(string targetRole, string propertyName)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                return ControlComponentReflection.ReadProperty<TReturn>(targetRole, propertyName, this.orderOutputs[targetRole]);
            else
                return ControlComponentReflection.ReadProperty<TReturn>(targetRole, propertyName, this);
        }

        public override void CallMethod(string targetRole, string methodName)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                ControlComponentReflection.CallMethod(targetRole, methodName, this.orderOutputs[targetRole]);
            else
                ControlComponentReflection.CallMethod(targetRole, methodName, this);
        }

        public override void CallMethod<TParam>(string targetRole, string methodName, TParam param)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                ControlComponentReflection.CallMethod<TParam>(targetRole, methodName, param, this.orderOutputs[targetRole]);
            else
                ControlComponentReflection.CallMethod<TParam>(targetRole, methodName, param, this);
        }

        public override TReturn CallMethod<TReturn>(string targetRole, string methodName)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                return ControlComponentReflection.CallMethod<TReturn>(targetRole, methodName, this.orderOutputs[targetRole]);
            else
                return ControlComponentReflection.CallMethod<TReturn>(targetRole, methodName, this);
        }

        public override TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                return ControlComponentReflection.CallMethod<TParam, TReturn>(targetRole, methodName, param, this.orderOutputs[targetRole]);
            else
                return ControlComponentReflection.CallMethod<TParam, TReturn>(targetRole, methodName, param, this);
        }

        public override void Subscribe<T>(string targetRole, string eventName, T eventHandler)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                ControlComponentReflection.Subscribe<T>(targetRole, eventName, eventHandler, this.orderOutputs[targetRole]);
            else
                ControlComponentReflection.Subscribe<T>(targetRole, eventName, eventHandler, this);
        }

        public override void Unsubscribe<T>(string targetRole, string eventName, T eventHandler)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                ControlComponentReflection.Unsubscribe<T>(targetRole, eventName, eventHandler, this.orderOutputs[targetRole]);
            else
                ControlComponentReflection.Unsubscribe<T>(targetRole, eventName, eventHandler, this);
        }
    }
}