using ControlComponents.Core;
namespace ControlComponents.ML
{
    public interface IMLControlComponent : IControlComponent
    {
        ExecutionState MLSC { get; set; }
        string MLMODEL { get; }
        float[] MLOBSERVE { get; set; }
        bool[][] MLENACT { get; set; }

        // MLDECIDE contains the probabilities of all possible actions per action branch
        float[][] MLDECIDE { get; set; }
        float MLREWARD { get; set; }
        string MLSTATS { get; set; }

        MLProperties MLProperties { get; }
    }
}