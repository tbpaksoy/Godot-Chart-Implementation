using Godot;
using System.Collections.Generic;
using System;
using System.Linq;
public partial class GraphChart : Chart
{
    private float[] intervals = null;
    private string[] labels = null;
    [Export]
    private Vector2 offset;
    public override void _Draw()
    {
        float min = float.MaxValue, max = float.MinValue;
        foreach (float f in data)
        {
            if (f < min) min = f;
            if (f > max) max = f;
        }
        List<Vector2> points = new List<Vector2>();
        Vector2 size = GetRect().Size - offset;
        if (min <= 0)
        {
            float zeroPoint = -min / (min - max) * size.Y + size.Y + offset.Y / 2f;
            DrawLine(new Vector2(offset.X / 2f, zeroPoint), new Vector2(size.X + offset.X / 2f, zeroPoint), new Color(1f, 0f, 0f), 2f);
        }
        for (int i = 0; i < data.Count; i++)
        {
            float y = (data[i] - min) / (min - max);
            points.Add(new Vector2(size.X / (data.Count - 1) * i, y * size.Y + size.Y) + offset / 2f);
        }
        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(points[i], points[i + 1], new Color(0f, 0f, 0f), 5f, true);
        }
        for (int i = 0; i < points.Count; i++)
        {
            DrawCircle(points[i], 5f, new Color(1f, 0f, 0.5f));
            DrawString(ThemeDB.FallbackFont, points[i] + new Vector2(5f, 10f), data[i].ToString());
        }
    }
    public void GetLabels(params object[] what)
    {
        if (what.Length == data.Count)
        {
            labels = new string[what.Length];
            for (int i = 0; i < labels.Length; i++) labels[i] = what[i].ToString();
        }
    }
    public void SetIntervals(params float[] intervals) => this.intervals = intervals.Clone() as float[];
    public void GetIntrevals(params DateOnly[] periods)
    {
        periods = periods.OrderBy(d => d.Year * 365 + d.Day).ToArray();
        intervals = new float[periods.Length - 1];
        for (int i = 0; i < intervals.Length; i++)
            intervals[i] = periods[i + 1].DayNumber - periods[i].DayNumber;

    }
}