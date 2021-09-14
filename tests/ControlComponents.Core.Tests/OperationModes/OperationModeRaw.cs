using System.Collections.ObjectModel;

namespace ControlComponents.Core.Tests
{
    internal class OperationModeRaw : OperationMode
    {
        public OperationModeRaw(string name) : base(name)
        {

        }
        public OperationModeRaw(string name, Collection<string> neededRoles) : base(name, neededRoles)
        {

        }

        protected override void Selected()
        {
            // No operation modes are selected on outputs
        }
    }
}