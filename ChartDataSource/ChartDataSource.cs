using Godot;
using Date = System.DateOnly;
[GlobalClass, Icon("res://Icons/ChartDataSource.svg"), Tool]
public partial class ChartDataSource : Node
{
    #region data
    protected Chart target;
    [Export]
    public string name;
    [Export]
    public float value;
    #endregion
    #region Godot methods
    public override void _EnterTree()
    {
        if (GetParent() is Chart chart)
        {
            chart.Add(this);
            chart.QueueRedraw();
            target = chart;
        }
        else QueueFree();
    }
    public override void _ExitTree()
    {
        Chart chart = GetParent() as Chart;
        try
        {
            chart.Remove(this);
        }
        finally
        {
            chart.QueueRedraw();
        }
    }
    public override void _Process(double delta)
    {
        if (target != null) target.UpdateProp(this);
    }
    #endregion
}