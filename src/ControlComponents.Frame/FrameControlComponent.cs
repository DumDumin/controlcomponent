using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ControlComponents.Core
{
    public class FrameControlComponent<T> : ControlComponent where T : IControlComponent
    {
        // This component includes the external deployed operation mode
        protected readonly T externalControlComponent;
        protected readonly IEnumerable<string> OccupationMethods;

        protected FrameControlComponent(string name, T cc, ICollection<IOperationMode> opModes, ICollection<IOrderOutput> orderOutputs, ICollection<string> neededRoles)
        : base(name, opModes, orderOutputs, neededRoles)
        {
            this.externalControlComponent = cc;
            OccupationMethods = new List<string>(){
                nameof(Reset),
                nameof(Start),
                nameof(Suspend),
                nameof(Unsuspend),
                nameof(Stop),
                nameof(Hold),
                nameof(Unhold),
                nameof(Abort),
                nameof(Clear),
                nameof(Occupy),
                nameof(Free),
                nameof(Prio)
            };
        }


        // factory method is bad if class should be inherited from
        public static FrameControlComponent<T> Create(string name, T cc, IControlComponentProvider provider)
        {
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

        public override bool ChangeOutput(string role, string id)
        {
            bool success = orderOutputs[role].ChangeComponent(id);

            // After setting the output of the FrameControlComponent, set the correct output of the ExternalControlComponent
            if (externalControlComponent.Roles.Contains(role))
                success &= externalControlComponent.ChangeOutput(role, ComponentName);

            return success;
        }

        public override void ClearOutput(string role)
        {
            // Before clearing the output of the FrameControlComponent, clear the correct output of the ExternalControlComponent
            if (externalControlComponent.Roles.Contains(role))
                externalControlComponent.ClearOutput(role);

            orderOutputs[role].ClearComponent();
        }

        public override TReturn ReadProperty<TReturn>(string targetRole, string propertyName)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
            {
                return ControlComponentReflection.CallMethodGeneric<string, string, TReturn>(targetRole, nameof(ReadProperty), targetRole, propertyName, this.orderOutputs[targetRole]);
            }
            else
                return ControlComponentReflection.ReadProperty<TReturn>(targetRole, propertyName, this);
        }

        public override void CallMethod(string targetRole, string methodName)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                ControlComponentReflection.CallMethod<string, string>(targetRole, nameof(CallMethod), targetRole, methodName, this.orderOutputs[targetRole]);
            else
                ControlComponentReflection.CallMethod(targetRole, methodName, this);
        }

        public override void CallMethod<TParam>(string targetRole, string methodName, TParam param)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                // Methods that occupy outputs need to use the FrameControlComponent Name and not the ExternalControlComponent Name to occupy
                if (OccupationMethods.Contains(methodName))
                    ControlComponentReflection.CallMethodGeneric<string, string, string>(targetRole, nameof(CallMethod), targetRole, methodName, ComponentName, this.orderOutputs[targetRole]);
                else
                    ControlComponentReflection.CallMethodGeneric<string, string, TParam>(targetRole, nameof(CallMethod), targetRole, methodName, param, this.orderOutputs[targetRole]);
            else
                ControlComponentReflection.CallMethod<TParam>(targetRole, methodName, param, this);
        }

        public override TReturn CallMethod<TReturn>(string targetRole, string methodName)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
                return ControlComponentReflection.CallMethodGeneric<string, string, TReturn>(targetRole, nameof(CallMethod), targetRole, methodName, this.orderOutputs[targetRole]);
            else
                return ControlComponentReflection.CallMethod<TReturn>(targetRole, methodName, this);
        }

        public override TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param)
        {
            if (this.orderOutputs.ContainsKey(targetRole))
            {
                // Outputs of FrameControlComponent are occupied by FrameComponent not ExternalControlComponent => Change to correct OCCUPIER name
                if (methodName == nameof(IsUsableBy))
                    return ControlComponentReflection.CallMethodGeneric<string, string, string, TReturn>(targetRole, nameof(CallMethod), targetRole, methodName, ComponentName, this.orderOutputs[targetRole]);
                else
                    return ControlComponentReflection.CallMethodGeneric<string, string, TParam, TReturn>(targetRole, nameof(CallMethod), targetRole, methodName, param, this.orderOutputs[targetRole]);
            }
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