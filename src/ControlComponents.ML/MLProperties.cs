namespace ControlComponents.ML
{
    public struct MLProperties
    {
        public readonly int ObservationSize;
        public readonly int ActionSize;
        // TODO add Model name? vs override MLModelName

        public MLProperties(int observationSize, int actionSize)
        {
            ObservationSize = observationSize;
            ActionSize = actionSize;
        }
    }
}