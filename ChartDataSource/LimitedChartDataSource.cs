using Godot;
[GlobalClass, Tool]
public partial class LimitedChartDataSource : ChartDataSource
{
    #region data
    [Export]
    public float maxLimit, minLimit;
    private float _value;
    #endregion
    #region Godot methods
    public override void _Process(double delta)
    {
        value = Mathf.Clamp(value, minLimit, maxLimit);
        base._Process(delta);
    }
    #endregion
}
