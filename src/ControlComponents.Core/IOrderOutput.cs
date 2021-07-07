namespace ControlComponents.Core
{
    // TODO throw errors instead
    public enum OrderOutputError { OK, Completed, Stopped, NotExisting, NullRequested, NotExecuting, NotAccepted, Occupied };

    public interface IOrderOutput : IControlComponent
    {
        OrderOutputError Error { get; }
        string Role { get; }
        string Id { get; }
        bool IsSet { get; }

        bool ChangeComponent(string id);
    }
}