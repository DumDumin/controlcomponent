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

        public bool IsSet { get; private set; }
        public string Role { get; }
        public string OwnerId { get; }
        public ExecutionState EXST
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<ExecutionState>(Role, nameof(EXST));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }
        public ExecutionMode EXMODE
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<ExecutionMode>(Role, nameof(EXMODE));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public string OpModeName 
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<string>(Role, nameof(OpModeName));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public ICollection<string> OpModes
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<ICollection<string>>(Role, nameof(OpModes));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }
        public ICollection<string> Roles
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<ICollection<string>>(Role, nameof(Roles));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public event ExecutionStateEventHandler ExecutionStateChanged;
        public event OccupationEventHandler OccupierChanged;
        public event OperationModeEventHandler OperationModeChanged;
        public event ExecutionModeEventHandler ExecutionModeChanged;

        public string OCCUPIER
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<string>(Role, nameof(OCCUPIER));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public string ComponentName
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<string>(Role, nameof(ComponentName));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public string WORKST
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<string>(Role, nameof(WORKST));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public void Occupy(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Occupy), sender));
        public void Prio(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Prio), sender));
        public void Free(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Free), sender));
        public bool IsOccupied() => controlComponent.CallMethod<bool>(Role, nameof(IsOccupied));
        public bool IsFree() => controlComponent.CallMethod<bool>(Role, nameof(IsFree));
        public bool IsUsableBy(string id) => IsSet && controlComponent.CallMethod<string, bool>(Role, nameof(IsUsableBy), id);

        private void OnExecutionStateChanged(object sender, ExecutionStateEventArgs e) => ExecutionStateChanged?.Invoke(this.Role, e);
        private void OnExecutionModeChanged(object sender, ExecutionModeEventArgs e) => ExecutionModeChanged?.Invoke(this.Role, e);
        private void OnOccupierChanged(object sender, OccupationEventArgs e) => OccupierChanged?.Invoke(this.Role, e);
        private void OnOperationModeChanged(object sender, OperationModeEventArgs e) => OperationModeChanged?.Invoke(this.Role, e);

        public OrderOutputTemplate(string role, string id, IControlComponentProvider provider)
        {
            Role = role;
            OwnerId = id;
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
            if (IsSet)
                return controlComponent.CallMethod<string, Task>(Role, nameof(SelectOperationMode), operationMode);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public Task DeselectOperationMode()
        {
            if (IsSet)
                return controlComponent.CallMethod<Task>(Role, nameof(DeselectOperationMode));
            else
                return Task.CompletedTask;
        }

        private void RunIfSet(Action action)
        {
            if (IsSet)
                action.Invoke();
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public void Reset(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Reset), sender));
        }

        public void Start(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Start), sender));
        }
        public void Stop(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Stop), sender));
        }

        public void Suspend(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Suspend), sender));
        }

        public void Unsuspend(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Unsuspend), sender));
        }

        public void Hold(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Hold), sender));
        }
        public void Unhold(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Unhold), sender));
        }
        public void Abort(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Abort), sender));
        }
        public void Clear(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Clear), sender));
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
                controlComponent = default(T);
            }
        }

        // TODO seperate IControlComponent and IOrderOutput for this manner ?
        public bool ChangeOutput(string role, string id)
        {
            if(IsSet)
                return controlComponent.CallMethod<string, string, bool>(Role, nameof(ChangeOutput), role, id);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public void ClearOutput(string role)
        {
            if(IsSet)
                controlComponent.CallMethod<string>(Role, nameof(ClearOutput), role);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public void Auto(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Auto), sender));

        public void SemiAuto(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(SemiAuto), sender));

        public TReturn ReadProperty<TReturn>(string targetRole, string propertyName)
        {
            if(IsSet)
                return controlComponent.ReadProperty<TReturn>(targetRole, propertyName);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public void CallMethod(string targetRole, string methodName)
        {
            if(IsSet)
                controlComponent.CallMethod(targetRole, methodName);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public void CallMethod<TParam>(string targetRole, string methodName, TParam param)
        {
            if(IsSet)
                controlComponent.CallMethod<TParam>(targetRole, methodName, param);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public TReturn CallMethod<TReturn>(string targetRole, string methodName)
        {
            if(IsSet)
                return controlComponent.CallMethod<TReturn>(targetRole, methodName);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param)
        {
            if(IsSet)
                return controlComponent.CallMethod<TParam, TReturn>(targetRole, methodName, param);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public TReturn CallMethod<TParam1, TParam2, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2)
        {
            if(IsSet)
                return controlComponent.CallMethod<TParam1, TParam2, TReturn>(targetRole, methodName, param1, param2);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
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
            return (Role + OwnerId).GetHashCode();
        }
    }
}
