using System.Threading;
using System.Threading.Tasks;
using ControlComponents.Core;

namespace ControlComponents.ML
{
    public class ConfigOperationMode : OperationModeBase
    {
        public ConfigOperationMode(string name) : base(name)
        {
        }

        protected override void Selected()
        {
            throw new System.NotImplementedException();
        }

        protected override Task Starting(CancellationToken token)
        {
            // outputs["RL"]
            return base.Starting(token);
        }
    }
}