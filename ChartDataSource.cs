using System;
using Godot;
using Godot.Collections;
using Date = System.DateOnly;
[GlobalClass]
[Tool]
public partial class ChartDataSource : Node
{
    private Chart target;
    [Export]
    public string name;
    [Export]
    public float value;
    public Date? date = null;
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
        chart.Remove(this);
        chart.QueueRedraw();
    }
    public override void _Process(double delta) => target.UpdateProp(this);
}