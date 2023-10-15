using Godot;
[Tool]
[GlobalClass]
public partial class LimitedChartDataSource : ChartDataSource
{
    [Export]
    public float maxLimit, minLimit;
    public override void _Process(double delta)
    {
        value = Mathf.Clamp(value, minLimit, maxLimit);
        base._Process(delta);
    }
}
