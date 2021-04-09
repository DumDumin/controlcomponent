namespace ControlComponent
{
    public interface IControlComponent : IExecutionState
    {
        string OpModeName { get; }

        string OCCUPIER { get; }
        void Occupy(string sender);
        void Free(string sender);
        bool IsOccupied();
        bool IsFree();
    }
}