using System.Threading.Tasks;

namespace ControlComponent
{
    public interface IControlComponent : IExecutionState
    {
        string OpModeName { get; }

        string OCCUPIER { get; }
        void Occupy(string sender);
        void Free(string sender);
        void Prio(string sender);
        bool IsOccupied();
        bool IsFree();

        void Reset(string sender);
        void Start(string sender);
        void Suspend(string sender);
        void Unsuspend(string sender);
        void Stop(string sender);
        void Hold(string sender);
        void Unhold(string sender);
        void Abort(string sender);
        void Clear(string sender);

        Task SelectOperationMode(string operationMode);
        Task DeselectOperationMode();
    }
}