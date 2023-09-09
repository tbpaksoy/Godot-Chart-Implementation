using Godot;
using System.Collections.Generic;
using System.Linq;
using Date = System.DateOnly;
using DateList = System.Collections.Generic.List<System.DateOnly>;
using DateAndIndex = System.Collections.Generic.Dictionary<System.DateOnly, int>;
[Tool]
public partial class GraphChart : Chart
{
    [Export]
    private Vector2 offset;
    [Export]
    public Color lineColor, nodeColor;
    public override void _Draw()
    {

        if (sources == null || sources.Count == 0) return;
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
        if (ableToOrderByDate)
        {
            float divider = min - max;
            divider = divider == 0 ? 1 : divider;
            for (int i = 0; i < data.Count; i++)
            {
                float y = (data[i] - min) / divider;
                points.Add(new Vector2(size.X / (data.Count - 1) * i, y * size.Y + size.Y) + offset / 2f);
            }
        }
#if Test
        else
        {
            DateList dateList = sourceAndDate.Values.ToList();
            DateAndIndex dateAndIndex = new DateAndIndex();
            for (int i = 0, j = 0; i < dateList.Count; i++)
            {
                dateAndIndex.Add(dateList[i], j);
                j++;
            }
            DateList keys = dateAndIndex.Keys.OrderBy(d => d.DayNumber).ToList();
            float widthPerDay = size.Y / (keys.Max().DayNumber - keys.Min().DayNumber);
            float x = 0f;
            for (int i = 0; i < keys.Count; i++)
            {
                float y = data[dateAndIndex[keys[i]]] / (min - max);
                points.Add(new Vector2(x, y * size.Y + size.Y));
                x += (keys[(i + 1) % i].DayNumber - keys[i].DayNumber) * widthPerDay;
            }
        }
#endif
        for (int i = 0; i < points.Count - 1; i++)
        {

            DrawLine(points[i], points[i + 1], lineColor, 5f, false);
            DrawCircle(points[i], 5f, nodeColor);
        }
        DrawCircle(points[^1], 5f, nodeColor);
    }
    public static float DampingFunction(float t) => Mathf.Pow(Mathf.E, -t) * Mathf.Cos(2 * Mathf.Pi * t);
}