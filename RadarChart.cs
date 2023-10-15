using Godot;
using Godot.Collections;
[GlobalClass]
[Tool]
public partial class RadarChart : Chart
{
    private Array<float> limits = new Array<float>();
    [Export]
    public float lineWidth;
    [Export]
    public Color lineColor;
    [Export]
    public Color polygonColor;
    public override void _Draw()
    {
        if (data == null && data.Count < 3) return;
        foreach (float f in limits) if (f <= 0f) return;
        CreateRadar();
        CreatePolygon();
    }
    public override void _Notification(int what)
    {
        if (what == NotificationChildOrderChanged)
        {
            for (int i = 0; i < GetChildCount(); i++)
            {
                Node node = GetChild(i);
                if (node is not LimitedChartDataSource)
                {
                    node.QueueFree();
                    GD.PrintErr("Only limited chart data source is accepted in radar chart");
                }
            }
        }
    }
    public override void UpdateProp(ChartDataSource source)
    {
        base.UpdateProp(source);
        if (source is LimitedChartDataSource lcds)
        {
            int index = sources.IndexOf(lcds);
            bool redraw = limits[index] != lcds.maxLimit;
            limits[index] = lcds.maxLimit;
            if (redraw) QueueRedraw();
        }
    }
    public override void Add(ChartDataSource source)
    {
        base.Add(source);
        if (source is LimitedChartDataSource lcds)
        {
            limits.Add(lcds.maxLimit);
        }
    }
    private void CreateRadar()
    {
        float period = 2f * Mathf.Pi / data.Count;
        Vector2[] points = new Vector2[data.Count + 1];
        float radius = Mathf.Min(Size.X, Size.Y) / 2f;
        for (int i = 0; i < data.Count; i++)
        {
            float temp = period * i;
            points[i] = new Vector2(Mathf.Cos(temp), Mathf.Sin(temp)) * radius + Size / 2f;
        }
        points[^1] = points[0];
        DrawPolyline(points, lineColor, lineWidth);
    }
    private void CreatePolygon()
    {
        float period = 2f * Mathf.Pi / data.Count;
        Vector2[] points = new Vector2[data.Count + 1];
        Color[] colors = new Color[data.Count + 1];
        for (int i = 0; i < data.Count; i++)
        {
            float radius = Mathf.Min(Size.X, Size.Y) / 2f * data[i] / limits[i];
            float temp = period * i;
            colors[i] = polygonColor;
            points[i] = new Vector2(Mathf.Cos(temp), Mathf.Sin(temp)) * radius + Size / 2f;
        }
        points[^1] = points[0];
        colors[^1] = polygonColor;
        DrawPolygon(points, colors);
    }
}