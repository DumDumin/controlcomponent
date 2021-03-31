using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ControlComponent.Tests
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