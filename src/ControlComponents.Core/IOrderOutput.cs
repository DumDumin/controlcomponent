namespace ControlComponents.Core
{
    public enum OrderOutputError { OK, Completed, Stopped, NotExisting, NullRequested, NotExecuting, NotAccepted, Occupied };

    public interface IOrderOutput : IControlComponent
    {
        OrderOutputError Error { get; }
        string Role { get; }
        string OwnerId { get; }
        bool IsSet { get; }

        bool ChangeComponent(string id);
        void ClearComponent();
    }
}