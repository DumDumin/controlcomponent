using System;

namespace ControlComponents.Core
{
    public class FrameControlComponent : ControlComponent
    {
        // This component includes the external deployed operation mode
        private readonly IControlComponent externalControlComponent;

        public FrameControlComponent(IControlComponent cc, IControlComponentProvider provider, string name = "FrameControlComponent") : base(name)
        {
            this.externalControlComponent = cc;

            // (TODO) create ExternalOpmodeOutput to inject it into opmode
            var output = new OrderOutput("ExternalOperationMode", ComponentName, provider, cc);
            AddOrderOutput(output);

            foreach (var operationModeName in cc.OpModes)
            {
                AddOperationMode(new ConfigOperationMode(operationModeName, output));
            }
        }

        public override TReturn ReadProperty<TReturn>(string targetRole, string propertyName)
        {
            if(this.orderOutputs.ContainsKey(targetRole))
                return ReadPropertyy<TReturn>(targetRole, propertyName, this.orderOutputs[targetRole]);
            else
                return ReadPropertyy<TReturn>(targetRole, propertyName, this);
        }
    }
}