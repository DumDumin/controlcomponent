using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public class OrderOutput : OrderOutputTemplate<IControlComponent>
    {
        public OrderOutput(string role, string id, IControlComponentProvider provider) : base(role, id, provider) { }

        public OrderOutput(string role, string id, IControlComponentProvider provider, IControlComponent cc) : base(role, id, provider, cc) { }
    }

    public class OrderOutputTemplate<T> : IOrderOutput where T : IControlComponent
    {
        private readonly IControlComponentProvider _provider;

        public OrderOutputError Error { get; }

        // TOBI create Reset method to auto assign cc

        protected T controlComponent { get; private set; }

        public string Role { get; }
        public string Id { get; }
        public ExecutionState EXST => controlComponent.ReadProperty<ExecutionState>(Role, nameof(EXST));
        public ExecutionMode EXMODE => controlComponent.ReadProperty<ExecutionMode>(Role, nameof(EXMODE));

        public string OpModeName => controlComponent.ReadProperty<string>(Role, nameof(OpModeName));
        public ICollection<string> OpModes => controlComponent.ReadProperty<ICollection<string>>(Role, nameof(OpModes));
        public ICollection<string> Roles => controlComponent.ReadProperty<ICollection<string>>(Role, nameof(Roles));

        public event ExecutionStateEventHandler ExecutionStateChanged;
        public event OccupationEventHandler OccupierChanged;
        public event OperationModeEventHandler OperationModeChanged;
        public event ExecutionModeEventHandler ExecutionModeChanged;

        public string OCCUPIER => controlComponent.ReadProperty<string>(Role, nameof(OCCUPIER));

        public string ComponentName => controlComponent.ReadProperty<string>(Role, nameof(ComponentName));

        public bool IsSet { get; private set; }

        public string WORKST => controlComponent.ReadProperty<string>(Role, nameof(WORKST));

        public void Occupy(string sender) => controlComponent.CallMethod<string>(Role, nameof(Occupy), sender);
        public void Prio(string sender) => controlComponent.CallMethod<string>(Role, nameof(Prio), sender);
        public void Free(string sender) => controlComponent.CallMethod<string>(Role, nameof(Free), sender);
        public bool IsOccupied() => controlComponent.CallMethod<bool>(Role, nameof(IsOccupied));
        public bool IsFree() => controlComponent.CallMethod<bool>(Role, nameof(IsFree));
        // TODO use IsSet as well
        public bool IsUsableBy(string id) => controlComponent.CallMethod<string, bool>(Role, nameof(IsUsableBy), id);

        private void OnExecutionStateChanged(object sender, ExecutionStateEventArgs e) => ExecutionStateChanged?.Invoke(this.Role, e);
        private void OnExecutionModeChanged(object sender, ExecutionModeEventArgs e) => ExecutionModeChanged?.Invoke(this.Role, e);
        private void OnOccupierChanged(object sender, OccupationEventArgs e) => OccupierChanged?.Invoke(this.Role, e);
        private void OnOperationModeChanged(object sender, OperationModeEventArgs e) => OperationModeChanged?.Invoke(this.Role, e);

        public OrderOutputTemplate(string role, string id, IControlComponentProvider provider)
        {
            Role = role;
            Id = id;
            _provider = provider;
        }

        public OrderOutputTemplate(string role, string id, IControlComponentProvider provider, T cc) : this(role, id, provider)
        {
            controlComponent = cc;
            SubscribeToEvents();
            IsSet = true;
        }


        public Task SelectOperationMode(string operationMode)
        {
            return controlComponent.CallMethod<string, Task>(Role, nameof(SelectOperationMode) ,operationMode);
        }

        public Task DeselectOperationMode()
        {
            return controlComponent.CallMethod<Task>(Role, nameof(DeselectOperationMode));
        }

        public void Reset(string sender)
        {
            controlComponent.CallMethod<string>(Role, nameof(Reset), sender);
        }

        public void Start(string sender)
        {
            controlComponent.CallMethod<string>(Role, nameof(Start), sender);
        }
        public void Stop(string sender)
        {
            controlComponent.CallMethod<string>(Role, nameof(Stop), sender);
        }

        public void Suspend(string sender)
        {
            controlComponent.CallMethod<string>(Role, nameof(Suspend), sender);
        }

        public void Unsuspend(string sender)
        {
            controlComponent.CallMethod<string>(Role, nameof(Unsuspend), sender);
        }

        public void Hold(string sender)
        {
            controlComponent.CallMethod<string>(Role, nameof(Hold), sender);
        }
        public void Unhold(string sender)
        {
            controlComponent.CallMethod<string>(Role, nameof(Unhold), sender);
        }
        public void Abort(string sender)
        {
            controlComponent.CallMethod<string>(Role, nameof(Abort), sender);
        }
        public void Clear(string sender)
        {
            controlComponent.CallMethod<string>(Role, nameof(Clear), sender);
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                OrderOutput p = (OrderOutput)obj;
                return (ComponentName == p.ComponentName);
            }
        }

        private void SubscribeToEvents()
        {
            controlComponent.Subscribe<ExecutionStateEventHandler>(Role, nameof(ExecutionStateChanged), OnExecutionStateChanged);
            controlComponent.Subscribe<ExecutionModeEventHandler>(Role, nameof(ExecutionModeChanged), OnExecutionModeChanged);
            controlComponent.Subscribe<OccupationEventHandler>(Role, nameof(OccupierChanged), OnOccupierChanged);
            controlComponent.Subscribe<OperationModeEventHandler>(Role, nameof(OperationModeChanged), OnOperationModeChanged);
        }
        private void UnsubscribeFromEvents()
        {
            controlComponent.Unsubscribe<ExecutionStateEventHandler>(Role, nameof(ExecutionStateChanged), OnExecutionStateChanged);
            controlComponent.Unsubscribe<ExecutionModeEventHandler>(Role, nameof(ExecutionModeChanged), OnExecutionModeChanged);
            controlComponent.Unsubscribe<OccupationEventHandler>(Role, nameof(OccupierChanged), OnOccupierChanged);
            controlComponent.Unsubscribe<OperationModeEventHandler>(Role, nameof(OperationModeChanged), OnOperationModeChanged);
        }
        
        public bool ChangeComponent(string id)
        {
            T c = _provider.GetComponent<T>(id);
            return this.ChangeComponent(c);
        }

        public bool ChangeComponent(T cc)
        {
            if (!IsSet)
            {
                controlComponent = cc;
                SubscribeToEvents();
                IsSet = true;
                return true;
            }
            else if (EXST == ExecutionState.STOPPED)
            {
                UnsubscribeFromEvents();
                controlComponent = cc;
                SubscribeToEvents();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClearComponent()
        {
            if (IsSet)
            {
                IsSet = false;
                UnsubscribeFromEvents();
                // controlComponent = null;
            }
        }

        // TODO seperate IControlComponent and IOrderOutput for this manner ?
        public bool ChangeOutput(string role, string id)
        {
            return controlComponent.CallMethod<string, string, bool>(Role, nameof(ChangeOutput), role, id);
        }

        public void Auto(string sender) => controlComponent.CallMethod<string>(Role, nameof(Auto), sender);

        public void SemiAuto(string sender) => controlComponent.CallMethod<string>(Role, nameof(SemiAuto), sender);

        public TReturn ReadProperty<TReturn>(string targetRole, string propertyName)
        {
            return controlComponent.ReadProperty<TReturn>(targetRole, propertyName);
        }

        public void CallMethod(string targetRole, string methodName)
        {
            controlComponent.CallMethod(targetRole, methodName);
        }

        public void CallMethod<TParam>(string targetRole, string methodName, TParam param)
        {
            controlComponent.CallMethod<TParam>(targetRole, methodName, param);
        }

        public TReturn CallMethod<TReturn>(string targetRole, string methodName)
        {
            return controlComponent.CallMethod<TReturn>(targetRole, methodName);
        }

        public TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param)
        {
            return controlComponent.CallMethod<TParam, TReturn>(targetRole, methodName, param);
        }

        public TReturn CallMethod<TParam1, TParam2, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2)
        {
            return controlComponent.CallMethod<TParam1, TParam2, TReturn>(targetRole, methodName, param1, param2);
        }

        public void Subscribe<T1>(string targetRole, string eventName, T1 eventHandler)
        {
            // IMPORTANT: subscribe to OrderOutput events and not controlcomponent, because controlcomponent might be changed
            ControlComponentReflection.Subscribe<T1>(targetRole, eventName, eventHandler, this);
        }

        public void Unsubscribe<T1>(string targetRole, string eventName, T1 eventHandler)
        {
            ControlComponentReflection.Unsubscribe<T1>(targetRole, eventName, eventHandler, this);
        }

        public override int GetHashCode()
        {
            return (Role + Id).GetHashCode();
        }
    }
}
