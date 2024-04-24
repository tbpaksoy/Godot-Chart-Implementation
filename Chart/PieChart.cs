using Godot;
using System.Collections.Generic;
using Date = System.DateOnly;
[GlobalClass]
[Tool]
public partial class PieChart : Chart
{
    private List<Date> dates = new List<Date>();
    private Date _minDate = Date.MaxValue, _maxDate = Date.MinValue;
    private Color[] _colors;
    [Export]
    public Color[] colors
    {
        get => _colors;
        set
        {
            _colors = value;
            QueueRedraw();
        }
    }
    public override void _Draw()
    {

        if (sources.Count < 2 && min < 0f) return;
        float total = 0;
        foreach (float f in data)
            total += f;
        float t = 0f;
        List<(float, float)> angles = new List<(float, float)>();
        for (int i = 0; i < data.Count; i++)
        {
            float first = t / total;
            t += data[i];
            float second = t / total;
            angles.Add((first, second));
        }
        Vector2 s = GetRect().Size;
        Vector2 center = s / 2f;
        float size = Mathf.Min(s.X, s.Y) / 2;
        for (int i = 0; i < data.Count; i++)
        {
            DrawArcPoly(center, size, angles[i].Item1 * 360f, angles[i].Item2 * 360f, colors[i % colors.Length]);
        }
        for (int i = 0; i < data.Count; i++)
        {
            DrawValue(center, data[i].ToString(), angles[i].Item1 * 360f, angles[i].Item2 * 360, size / 2f);
        }
        for (int i = 0, j = 0; i < names.Count; i++)
        {
            if (!string.IsNullOrEmpty(names[i]))
            {
                DrawLegendElement(new Vector2(0f, j * 16f), Vector2.One * 16f, colors[i % colors.Length], names[i]);
                j++;
            }
        }
    }
    protected override bool AbleToDrawByDate() => false;
    public override void Add(ChartDataSource source)
    {
        base.Add(source);
        if (source is ChartDataSourceWithDate cdsd)
        {
            dates.Add(cdsd.date);
            if (cdsd.date < _minDate) _minDate = cdsd.date;
            if (cdsd.date > _maxDate) _maxDate = cdsd.date;

        }
    }
    public override void Remove(ChartDataSource source)
    {
        int index = sources.IndexOf(source);
        base.Remove(source);
        if (index == -1) return;
        Remove(index);
    }

    private void DrawArcPoly(Vector2 center, float radius, float angleFrom, float angleTo, Color color)
    {
        int nbPoints = 32;
        var pointsArc = new Vector2[nbPoints + 2];
        pointsArc[0] = center;
        var colors = new Color[] { color };
        for (int i = 0; i <= nbPoints; i++)
        {
            float anglePoint = Mathf.DegToRad(angleFrom + i * (angleTo - angleFrom) / nbPoints - 90);
            pointsArc[i + 1] = center + new Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius;
        }
        DrawPolygon(pointsArc, colors);


    }
    private void DrawValue(Vector2 center, string text, float fromAngle, float toAngle, float distance)
    {
        float middleAngle = (toAngle + fromAngle) / 2f;
        DrawString(GetThemeDefaultFont(), center + new Vector2(Mathf.Cos(Mathf.DegToRad(middleAngle - 90f)), Mathf.Sin(Mathf.DegToRad(middleAngle - 90f))) * distance, text, alignment: HorizontalAlignment.Center);
    }
    private void DrawLegendElement(Vector2 begin, Vector2 size, Color color, string name)
    {
        Vector2[] points = new Vector2[] { begin, begin + size with { X = 0f }, begin + size, begin + size with { Y = 0f } };
        Vector2[] uvs = new Vector2[] { Vector2.Zero, Vector2.Down, Vector2.One, Vector2.Right };
        DrawPrimitive(points, new Color[] { color, color, color, color }, uvs);
        DrawString(GetThemeDefaultFont(), points[2], name, alignment: HorizontalAlignment.Center);
    }
}