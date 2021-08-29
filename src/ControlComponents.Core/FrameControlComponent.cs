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
    }
}