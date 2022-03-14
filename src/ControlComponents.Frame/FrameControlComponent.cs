using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ControlComponents.Core;

namespace ControlComponents.Frame
{
    // TODO: remove Reflection methods from IControlComponent and override/new relevant methods in FrameControlComponent
    public class FrameControlComponent<T> : ControlComponent, IFrameControlComponent where T : IControlComponent
    {
        // This component includes the external deployed operation mode
        protected readonly T externalControlComponent;
        protected readonly IEnumerable<string> OccupationMethods;

        private IDictionary<string, FrameOrderOutput> frameOrderOutputs;

        protected FrameControlComponent(string name, T cc, ICollection<IOperationMode> opModes, ICollection<IOrderOutput> orderOutputs, ICollection<FrameOrderOutput> frameOrderOutputs, ICollection<string> neededRoles)
        : base(name, opModes, orderOutputs.Concat<IOrderOutput>(frameOrderOutputs).ToList(), neededRoles)
        {
            this.externalControlComponent = cc;
            // The orderoutputs for the wrapped/external cc are used internally to forward reflection methods
            this.frameOrderOutputs = frameOrderOutputs.ToDictionary(o => o.Role);
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
            Collection<FrameOrderOutput> frameOrderOutputs = CreateOrderOutputs(name, cc, provider);

            return new FrameControlComponent<T>(name, cc, opmodes, new Collection<IOrderOutput>() { output }, frameOrderOutputs, new Collection<string>());
        }

        protected static Collection<FrameOrderOutput> CreateOrderOutputs(string name, T cc, IControlComponentProvider provider)
        {
            var outputs = new Collection<FrameOrderOutput>();
            foreach (var role in cc.Roles)
            {
                outputs.Add(new FrameOrderOutput(role, name, provider, cc));
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

        public TReturn ReadProperty<TReturn>(string targetRole, string propertyName)
        {
            if (this.frameOrderOutputs.ContainsKey(targetRole))
            {
                return this.frameOrderOutputs[targetRole].ReadProperty<TReturn>(targetRole, propertyName);
            }
            else
                return ControlComponentReflection.ReadProperty<TReturn>(targetRole, propertyName, this);
        }

        public void CallMethod(string targetRole, string methodName)
        {
            if (this.frameOrderOutputs.ContainsKey(targetRole))
                this.frameOrderOutputs[targetRole].CallMethod<string>(targetRole, methodName);
            else
                ControlComponentReflection.CallMethod(targetRole, methodName, this);
        }

        public void CallMethod<TParam>(string targetRole, string methodName, TParam param)
        {
            if (this.frameOrderOutputs.ContainsKey(targetRole))
                // Methods that occupy outputs need to use the FrameControlComponent Name and not the ExternalControlComponent Name to occupy
                if (OccupationMethods.Contains(methodName))
                    this.frameOrderOutputs[targetRole].CallMethod<string>(targetRole, methodName, ComponentName);
                else
                    this.frameOrderOutputs[targetRole].CallMethod<TParam>(targetRole, methodName, param);
            else
                ControlComponentReflection.CallMethod<TParam>(targetRole, methodName, param, this);
        }

        public TReturn CallMethod<TReturn>(string targetRole, string methodName)
        {
            if (this.frameOrderOutputs.ContainsKey(targetRole))
                return this.frameOrderOutputs[targetRole].CallMethod<TReturn>(targetRole, methodName);
            else
                return ControlComponentReflection.CallMethod<TReturn>(targetRole, methodName, this);
        }

        public TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param)
        {
            if (this.frameOrderOutputs.ContainsKey(targetRole))
            {
                // Outputs of FrameControlComponent are occupied by FrameComponent not ExternalControlComponent => Change to correct OCCUPIER name
                if (methodName == nameof(IsUsableBy))
                    return this.frameOrderOutputs[targetRole].CallMethod<string, TReturn>(targetRole, methodName, ComponentName);
                else
                    return this.frameOrderOutputs[targetRole].CallMethod<TParam, TReturn>(targetRole, methodName, param);
            }
            else
                return ControlComponentReflection.CallMethod<TParam, TReturn>(targetRole, methodName, param, this);
        }

        public TReturn CallMethod<TParam1, TParam2, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2)
        {
            if (this.frameOrderOutputs.ContainsKey(targetRole))
            {
                return this.frameOrderOutputs[targetRole].CallMethod<TParam1, TParam2, TReturn>(targetRole, methodName, param1, param2);
            }
            else
                return ControlComponentReflection.CallMethod<TParam1, TParam2, TReturn>(targetRole, methodName, param1, param2, this);
        }

        public void Subscribe<THandler>(string targetRole, string eventName, THandler eventHandler)
        {
            if (this.frameOrderOutputs.ContainsKey(targetRole))
                this.frameOrderOutputs[targetRole].Subscribe<THandler>(targetRole, eventName, eventHandler);
            else
                ControlComponentReflection.Subscribe<THandler>(targetRole, eventName, eventHandler, this);
        }

        public void Unsubscribe<THandler>(string targetRole, string eventName, THandler eventHandler)
        {
            if (this.frameOrderOutputs.ContainsKey(targetRole))
                this.frameOrderOutputs[targetRole].Unsubscribe<THandler>(targetRole, eventName, eventHandler);
            else
                ControlComponentReflection.Unsubscribe<THandler>(targetRole, eventName, eventHandler, this);
        }

    }
}