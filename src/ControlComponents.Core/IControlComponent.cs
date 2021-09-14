using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public interface IControlComponent : IExecutionState, IOccupation
    {
        string OpModeName { get; }
        string WORKST { get; }
        ICollection<string> OpModes { get; }
        ICollection<string> Roles { get; }

        void Reset(string sender);
        void Start(string sender);
        void Suspend(string sender);
        void Unsuspend(string sender);
        void Stop(string sender);
        void Hold(string sender);
        void Unhold(string sender);
        void Abort(string sender);
        void Clear(string sender);

        void Auto(string sender);
        void SemiAuto(string sender);

        event OperationModeEventHandler OperationModeChanged;
        Task SelectOperationMode(string operationMode);
        Task DeselectOperationMode();

        bool ChangeOutput(string role, string id);
        void ClearOutput(string role);

        TReturn ReadProperty<TReturn>(string targetRole, string propertyName);
        void CallMethod(string targetRole, string methodName);
        void CallMethod<TParam>(string targetRole, string methodName, TParam param);
        TReturn CallMethod<TReturn>(string targetRole, string methodName);
        TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param);
        TReturn CallMethod<TParam1, TParam2, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2);
        void Subscribe<T>(string targetRole, string eventName, T eventHandler);
        void Unsubscribe<T>(string targetRole, string eventName, T eventHandler);
    }
}