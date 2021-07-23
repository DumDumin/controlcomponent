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
        public ExecutionState EXST => controlComponent.EXST;

        public string OpModeName => controlComponent.OpModeName;
        public ICollection<string> OpModes => controlComponent.OpModes;
        public ICollection<string> Roles => controlComponent.Roles;

        public event ExecutionStateEventHandler ExecutionStateChanged;
        public event OccupationEventHandler OccupierChanged;

        public string OCCUPIER => controlComponent.OCCUPIER;

        public string ComponentName => controlComponent.ComponentName;

        public bool IsSet { get; private set; }

        public void Occupy(string sender) => controlComponent.Occupy(sender);
        public void Prio(string sender) => controlComponent.Prio(sender);
        public void Free(string sender) => controlComponent.Free(sender);
        public bool IsOccupied() => controlComponent.IsOccupied();
        public bool IsFree() => controlComponent.IsFree();

        private void OnExecutionStateChanged(object sender, ExecutionStateEventArgs e) => ExecutionStateChanged?.Invoke(this.Role, e);
        private void OnOccupierChanged(object sender, OccupationEventArgs e) => OccupierChanged?.Invoke(this.Role, e);

        public OrderOutputTemplate(string role, string id, IControlComponentProvider provider)
        {
            Role = role;
            Id = id;
            _provider = provider;
        }

        public OrderOutputTemplate(string role, string id, IControlComponentProvider provider, T cc) : this(role, id, provider)
        {
            controlComponent = cc;
            controlComponent.ExecutionStateChanged += OnExecutionStateChanged;
            controlComponent.OccupierChanged += OnOccupierChanged;
            IsSet = true;
        }


        public async Task SelectOperationMode(string operationMode)
        {
            await controlComponent.SelectOperationMode(operationMode);
        }

        public async Task DeselectOperationMode()
        {
            await controlComponent.DeselectOperationMode();
        }

        public void Reset(string sender)
        {
            controlComponent.Reset(sender);
        }

        public void Start(string sender)
        {
            controlComponent.Start(sender);
        }
        public void Stop(string sender)
        {
            controlComponent.Stop(sender);
        }

        public void Suspend(string sender)
        {
            controlComponent.Suspend(sender);
        }

        public void Unsuspend(string sender)
        {
            controlComponent.Unsuspend(sender);
        }

        public void Hold(string sender)
        {
            controlComponent.Hold(sender);
        }
        public void Unhold(string sender)
        {
            controlComponent.Unhold(sender);
        }
        public void Abort(string sender)
        {
            controlComponent.Abort(sender);
        }
        public void Clear(string sender)
        {
            controlComponent.Clear(sender);
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

        public override int GetHashCode()
        {
            return ComponentName.GetHashCode();
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
                controlComponent.ExecutionStateChanged += OnExecutionStateChanged;
                controlComponent.OccupierChanged += OnOccupierChanged;
                IsSet = true;
                return true;
            }
            else if (controlComponent.EXST == ExecutionState.STOPPED)
            {
                controlComponent.ExecutionStateChanged -= OnExecutionStateChanged;
                controlComponent.OccupierChanged -= OnOccupierChanged;
                controlComponent = cc;
                controlComponent.OccupierChanged += OnOccupierChanged;
                controlComponent.ExecutionStateChanged += OnExecutionStateChanged;
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
                controlComponent.ExecutionStateChanged -= OnExecutionStateChanged;
                controlComponent.OccupierChanged -= OnOccupierChanged;
                // controlComponent = null;
            }
        }

        // TODO seperate IControlComponent and IOrderOutput for this manner ?
        public bool ChangeOutput(string role, string id)
        {
            return controlComponent.ChangeOutput(role, id);
        }
    }
}
