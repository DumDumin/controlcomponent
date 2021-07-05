using ControlComponents.Core;

namespace ControlComponents.ML
{
    public struct MLProperties
    {
        public int ObservationSize;
        public int ActionSize;

        public MLProperties(int observationSize, int actionSize)
        {
            ObservationSize = observationSize;
            ActionSize = actionSize;
        }
    }

    public class MLControlComponent : ControlComponent, IMLControlComponent
    {
        public MLControlComponent(string name, MLProperties properties) : base(name) {
            MLOBSERVE = new float[properties.ObservationSize];
            MLDECIDE = new float[properties.ActionSize];
            MLENACT = new bool[1][];
            MLENACT[0] = new bool[properties.ActionSize];
        }

        public ExecutionState MLSC { get; set; }

        // TODO maybe allow only get and set must be done via indices
        // This would make the size of array fix
        public string MLMODEL { get; }
        public float[] MLOBSERVE { get; set; }
        public bool[][] MLENACT { get; set; }
        public float[] MLDECIDE { get; set; }
        public float MLREWARD { get; set; }
        public string MLSTATS { get; set; }
    }
}