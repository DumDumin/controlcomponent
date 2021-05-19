namespace ControlComponents.Conveyor
{
    public interface IMotor
    {
        float Speed { get; set; }
        int Direction { get; set; }
    }
}