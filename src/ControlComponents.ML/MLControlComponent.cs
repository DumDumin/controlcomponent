using ControlComponents.Core;

namespace ControlComponents.ML
{
    public class MLControlComponent : ControlComponent, IMLControlComponent
    {
        public MLControlComponent(string name, MLProperties properties) : base(name) {
            MLOBSERVE = new float[properties.ObservationSize];

            MLDECIDE = new float[properties.ActionSize.Length][];
            for (int i = 0; i < properties.ActionSize.Length; i++)
            {
                MLDECIDE[i] = new float[properties.ActionSize[i]];
            }

            MLENACT = new bool[properties.ActionSize.Length][];
            for (int i = 0; i < properties.ActionSize.Length; i++)
            {
                MLENACT[i] = new bool[properties.ActionSize[i]];
            }
            
            MLProperties = properties;
        }

        public ExecutionState MLSC { get; set; }

        // TODO maybe allow only get and set must be done via indices
        // This would make the size of array fix
        public string MLMODEL { get; }
        public float[] MLOBSERVE { get; set; }
        public bool[][] MLENACT { get; set; }
        public float[][] MLDECIDE { get; set; }
        public float MLREWARD { get; set; }
        public string MLSTATS { get; set; }

        public MLProperties MLProperties { get;  private set; }
    }
}