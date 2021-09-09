using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ControlComponents.Core
{
    public class FrameControlComponent<T> : ControlComponent where T : IControlComponent
    {
        // This component includes the external deployed operation mode
        protected readonly T externalControlComponent;

        protected FrameControlComponent(string name, T cc, ICollection<IOperationMode> opModes, ICollection<IOrderOutput> orderOutputs, ICollection<string> neededRoles)
        : base(name, opModes, orderOutputs, neededRoles)
        {
            this.externalControlComponent = cc;
        }

        public static FrameControlComponent<T> Create(string name, T cc, IControlComponentProvider provider)
        {
            // TODO create ExternalOpmodeOutput to inject it into opmode
            var output = new OrderOutput("ExternalOperationMode", name, provider, cc);
            Collection<IOperationMode> opmodes = CreateConfigOperationModes(cc, output);
            Collection<IOrderOutput> outputs = CreateOrderOutputs(name, cc, provider);
            outputs.Add(output);

            return new FrameControlComponent<T>(name, cc, opmodes, outputs, new Collection<string>());
        }

        protected static Collection<IOrderOutput> CreateOrderOutputs(string name, T cc, IControlComponentProvider provider)
        {
            var outputs = new Collection<IOrderOutput>();
            foreach (var role in cc.Roles)
            {
                outputs.Add(new OrderOutput(role, name, provider));
            }
            return outputs;
        }

        protected static Collection<IOperationMode> CreateConfigOperationModes(T cc, OrderOutput output)
        {
            var opmodes = new Collection<IOperationMode>();
            foreach (var operationModeName in cc.OpModes)
            {
                opmodes.Add(new ConfigOperationMode(operationModeName, output));
            }
            return opmodes;
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

        public override void Subscribe<THandler>(string targetRole, string eventName, THandler eventHandler)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                ControlComponentReflection.Subscribe<THandler>(targetRole, eventName, eventHandler, this.orderOutputs[targetRole]);
            else
                ControlComponentReflection.Subscribe<THandler>(targetRole, eventName, eventHandler, this);
        }

        public override void Unsubscribe<THandler>(string targetRole, string eventName, THandler eventHandler)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                ControlComponentReflection.Unsubscribe<THandler>(targetRole, eventName, eventHandler, this.orderOutputs[targetRole]);
            else
                ControlComponentReflection.Unsubscribe<THandler>(targetRole, eventName, eventHandler, this);
        }
    }
}