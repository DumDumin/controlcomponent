using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ControlComponent;

namespace ControlComponent.Tests
{
    internal class OperationModeCascade : OperationModeBase
    {
        private Dictionary<string, string> roleToOpMode;

        public OperationModeCascade(string name) : base(name, new Collection<string>() { "ROLE_ONE", "ROLE_TWO" })
        {
            roleToOpMode = new Dictionary<string, string>();
            roleToOpMode.Add("ROLE_ONE", "OpModeOne");
            roleToOpMode.Add("ROLE_TWO", "OpModeTwo");
        }

        protected override void Selected()
        {
            foreach (var roleKV in roleToOpMode)
            {
                base.SelectRole(roleKV.Key, roleKV.Value);
            }
        }
    }
}