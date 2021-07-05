using ControlComponents.Core;

namespace ControlComponents.ML
{
    public class MLControlComponent : ControlComponent, IMLControlComponent
    {
        public MLControlComponent(string name) : base(name) {}

        public ExecutionState MLSC { get; set; }

        public string MLMODEL { get; }
        public float[] MLOBSERVE { get; set; }
        public bool[][] MLENACT { get; set; }
        public float[] MLDECIDE { get; set; }
        public float MLREWARD { get; set; }
        public string MLSTATS { get; set; }
    }
}