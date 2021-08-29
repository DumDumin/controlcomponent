using System.Collections.ObjectModel;

namespace ControlComponents.Core.Tests
{
    internal class OperationMode : OperationModeBase
    {
        public OperationMode(string name) : base(name)
        {

        }
        public OperationMode(string name, Collection<string> neededRoles) : base(name, neededRoles)
        {

        }

        protected override void Selected()
        {
            // No operation modes are selected on outputs
        }
    }
}