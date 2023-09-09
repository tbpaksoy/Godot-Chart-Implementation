using System;
using Godot;
using Godot.Collections;
using Date = System.DateOnly;
[GlobalClass]
[Tool]
public partial class ChartDataSource : Node
{
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
        }
        else QueueFree();
    }
    public override void _ExitTree()
    {
        Chart chart = GetParent() as Chart;
        chart.Remove(this);
        chart.QueueRedraw();
    }
    public override void _Notification(int what)
    {
        base._Notification(what);
        (GetParent() as Chart)?.UpdateProp(this);
    }
}