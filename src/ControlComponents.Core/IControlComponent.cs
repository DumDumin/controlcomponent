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
    }
}