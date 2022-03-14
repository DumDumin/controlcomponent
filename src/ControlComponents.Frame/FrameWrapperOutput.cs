using System.Collections.Generic;
using System.Threading.Tasks;
using ControlComponents.Core;

namespace ControlComponents.Frame
{
    public class FrameWrapperOutput : OrderOutputTemplate<IFrameControlComponent>
    {
        public FrameWrapperOutput(string role, string id, IControlComponentProvider provider) : base(role, id, provider) { }

        public FrameWrapperOutput(string role, string id, IControlComponentProvider provider, IFrameControlComponent cc) : base(role, id, provider, cc) { }

        public new ExecutionState EXST
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<ExecutionState>(Role, nameof(EXST));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }
        public new ExecutionMode EXMODE
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<ExecutionMode>(Role, nameof(EXMODE));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public new string OpModeName 
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<string>(Role, nameof(OpModeName));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public new ICollection<string> OpModes
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<ICollection<string>>(Role, nameof(OpModes));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }
        public new ICollection<string> Roles
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<ICollection<string>>(Role, nameof(Roles));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public new string OCCUPIER
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<string>(Role, nameof(OCCUPIER));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public new string ComponentName
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<string>(Role, nameof(ComponentName));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public new string WORKST
        {
            get
            {
                if (IsSet)
                    return controlComponent.ReadProperty<string>(Role, nameof(WORKST));
                else
                    throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
            }
        }

        public new void Occupy(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Occupy), sender));
        public new void Prio(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Prio), sender));
        public new void Free(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Free), sender));
        public new bool IsOccupied() => controlComponent.CallMethod<bool>(Role, nameof(IsOccupied));
        public new bool IsFree() => controlComponent.CallMethod<bool>(Role, nameof(IsFree));
        public new bool IsUsableBy(string id) => IsSet && controlComponent.CallMethod<string, bool>(Role, nameof(IsUsableBy), id);


        public new Task SelectOperationMode(string operationMode)
        {
            if (IsSet)
                return controlComponent.SelectOperationMode(operationMode);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public new Task DeselectOperationMode()
        {
            if (IsSet)
                return controlComponent.DeselectOperationMode();
            else
                return Task.CompletedTask;
        }

        public new void Reset(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Reset), sender));
        }

        public new void Start(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Start), sender));
        }

        public new void Stop(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Stop), sender));
        }

        public new void Suspend(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Suspend), sender));
        }

        public new void Unsuspend(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Unsuspend), sender));
        }

        public new void Hold(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Hold), sender));
        }

        public new void Unhold(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Unhold), sender));
        }

        public new void Abort(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Abort), sender));
        }
        
        public new void Clear(string sender)
        {
            RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Clear), sender));
        }

        public new bool ChangeOutput(string role, string id)
        {
            if(IsSet)
                return controlComponent.CallMethod<string, string, bool>(Role, nameof(ChangeOutput), role, id);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public new void ClearOutput(string role)
        {
            if(IsSet)
                controlComponent.CallMethod<string>(Role, nameof(ClearOutput), role);
            else
                throw new OrderOutputException($"Output {Role} in {OwnerId} is not set.");
        }

        public new void Auto(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(Auto), sender));

        public new void SemiAuto(string sender) => RunIfSet(() => controlComponent.CallMethod<string>(Role, nameof(SemiAuto), sender));

        protected override void SubscribeToEvents()
        {
            controlComponent.Subscribe<ExecutionStateEventHandler>(Role, nameof(ExecutionStateChanged), OnExecutionStateChanged);
            controlComponent.Subscribe<ExecutionModeEventHandler>(Role, nameof(ExecutionModeChanged), OnExecutionModeChanged);
            controlComponent.Subscribe<OccupationEventHandler>(Role, nameof(OccupierChanged), OnOccupierChanged);
            controlComponent.Subscribe<OperationModeEventHandler>(Role, nameof(OperationModeChanged), OnOperationModeChanged);
        }

        protected override void UnsubscribeFromEvents()
        {
            controlComponent.Unsubscribe<ExecutionStateEventHandler>(Role, nameof(ExecutionStateChanged), OnExecutionStateChanged);
            controlComponent.Unsubscribe<ExecutionModeEventHandler>(Role, nameof(ExecutionModeChanged), OnExecutionModeChanged);
            controlComponent.Unsubscribe<OccupationEventHandler>(Role, nameof(OccupierChanged), OnOccupierChanged);
            controlComponent.Unsubscribe<OperationModeEventHandler>(Role, nameof(OperationModeChanged), OnOperationModeChanged);
        }
    }
}