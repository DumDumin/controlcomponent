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
                    return controlComponent.EXST;
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }
        public ExecutionMode EXMODE
        {
            get
            {
                if (IsSet)
                    return controlComponent.EXMODE;
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public string OpModeName 
        {
            get
            {
                if (IsSet)
                    return controlComponent.OpModeName;
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public ICollection<string> OpModes
        {
            get
            {
                if (IsSet)
                    return controlComponent.OpModes;
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }
        public ICollection<string> Roles
        {
            get
            {
                if (IsSet)
                    return controlComponent.Roles;
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
                    return controlComponent.OCCUPIER;
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public string ComponentName
        {
            get
            {
                if (IsSet)
                    return controlComponent.ComponentName;
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public string WORKST
        {
            get
            {
                if (IsSet)
                    return controlComponent.WORKST;
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public void Occupy(string sender) => RunIfSet(() => controlComponent.Occupy(sender));
        public void Prio(string sender) => RunIfSet(() => controlComponent.Prio(sender));
        public void Free(string sender) => RunIfSet(() => controlComponent.Free(sender));
        public bool IsOccupied() => controlComponent.IsOccupied();
        public bool IsFree() => controlComponent.IsFree();
        public bool IsUsableBy(string id) => IsSet && controlComponent.IsUsableBy(id);

        protected void OnExecutionStateChanged(object sender, ExecutionStateEventArgs e) => ExecutionStateChanged?.Invoke(this.Role, e);
        protected void OnExecutionModeChanged(object sender, ExecutionModeEventArgs e) => ExecutionModeChanged?.Invoke(this.Role, e);
        protected void OnOccupierChanged(object sender, OccupationEventArgs e) => OccupierChanged?.Invoke(this.Role, e);
        protected void OnOperationModeChanged(object sender, OperationModeEventArgs e) => OperationModeChanged?.Invoke(this.Role, e);

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
                return controlComponent.SelectOperationMode(operationMode);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public Task DeselectOperationMode()
        {
            if (IsSet)
                return controlComponent.DeselectOperationMode();
            else
                return Task.CompletedTask;
        }

        protected void RunIfSet(Action action)
        {
            if (IsSet)
                action.Invoke();
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public void Reset(string sender)
        {
            RunIfSet(() => controlComponent.Reset(sender));
        }

        public void Start(string sender)
        {
            RunIfSet(() => controlComponent.Start(sender));
        }
        public void Stop(string sender)
        {
            RunIfSet(() => controlComponent.Stop(sender));
        }

        public void Suspend(string sender)
        {
            RunIfSet(() => controlComponent.Suspend(sender));
        }

        public void Unsuspend(string sender)
        {
            RunIfSet(() => controlComponent.Unsuspend(sender));
        }

        public void Hold(string sender)
        {
            RunIfSet(() => controlComponent.Hold(sender));
        }
        public void Unhold(string sender)
        {
            RunIfSet(() => controlComponent.Unhold(sender));
        }
        public void Abort(string sender)
        {
            RunIfSet(() => controlComponent.Abort(sender));
        }
        public void Clear(string sender)
        {
            RunIfSet(() => controlComponent.Clear(sender));
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

        protected virtual void SubscribeToEvents()
        {
            controlComponent.ExecutionStateChanged += OnExecutionStateChanged;
            controlComponent.ExecutionModeChanged += OnExecutionModeChanged;
            controlComponent.OccupierChanged += OnOccupierChanged;
            controlComponent.OperationModeChanged += OnOperationModeChanged;
        }
        protected virtual void UnsubscribeFromEvents()
        {
            controlComponent.ExecutionStateChanged += OnExecutionStateChanged;
            controlComponent.ExecutionModeChanged += OnExecutionModeChanged;
            controlComponent.OccupierChanged += OnOccupierChanged;
            controlComponent.OperationModeChanged += OnOperationModeChanged;
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
                return controlComponent.ChangeOutput(role, id);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public void ClearOutput(string role)
        {
            if(IsSet)
                controlComponent.ClearOutput(role);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public void Auto(string sender) => RunIfSet(() => controlComponent.Auto(sender));

        public void SemiAuto(string sender) => RunIfSet(() => controlComponent.SemiAuto(sender));


        public override int GetHashCode()
        {
            return (Role + OwnerId).GetHashCode();
        }
    }
}
