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

        // maybe allow only get and set must be done via indices,
        // but this would make the size of array fix
        public string MLMODEL { get; }
        public float[] MLOBSERVE { get; set; }
        public bool[][] MLENACT { get; set; }
        public float[][] MLDECIDE { get; set; }
        public float MLREWARD { get; set; }
        public string MLSTATS { get; set; }

        public MLProperties MLProperties { get;  private set; }

        public static float[][] ConvertToActionMask(bool[][] actions)
        {
            var actionMask = new float[actions.Length][];
            for (int j = 0; j < actions.Length; j++)
            {    
                // returns action mask => 1 indicates masked and will not be used
                actionMask[j] = new float[actions[j].Length];
                for (int i = 0; i < actions[j].Length; i++)
                {
                    if (actions[j][i])
                        actionMask[j][i] = 0;
                    else
                        actionMask[j][i] = 1;
                }
            }
            return actionMask;
        }

        public static bool[][] ConvertToMLENACT(float[][] actionMask)
        {
            var action = new bool[actionMask.Length][];
            for (int j = 0; j < actionMask.Length; j++)
            {
                // returns MLENACT => false is masked and should not be used
                action[j] = new bool[actionMask[j].Length];
                for (int i = 0; i < actionMask[j].Length; i++)
                {
                    // if it is masked => false
                    if (actionMask[j][i] == 1)
                        action[j][i] = false;
                    else
                        action[j][i] = true;
                }
            }

            return action;
        }
    }
}