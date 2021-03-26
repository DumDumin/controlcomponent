using System.Collections;

namespace ControlComponent
{
    public class OrderOutput
    {
        //TODO throw errors instead
        // public enum OrderOutputError { OK, Completed, Stopped, NotExisting, NullRequested, NotExecuting, NotAccepted, Occupied };

        public string Role { get; }

        public OrderOutput(string role)
        {
            Role = role;
        }

        // public ControlComponentUnity Cc;

        // public OrderOutputError Error;
        // // Used to manage coroutines in ExecuteOpMode macro
        // public IEnumerator Coroutine;

        // TOBI create Reset method to auto assign cc
    }
}
