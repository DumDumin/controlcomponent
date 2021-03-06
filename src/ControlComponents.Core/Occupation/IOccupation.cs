namespace ControlComponents.Core
{
    public interface IOccupation 
    {
        string OCCUPIER { get; }
        void Occupy(string sender);
        void Free(string sender);
        void Prio(string sender);
        bool IsOccupied();
        bool IsFree();
        
        bool IsUsableBy(string id);

        event OccupationEventHandler OccupierChanged;
    }
}