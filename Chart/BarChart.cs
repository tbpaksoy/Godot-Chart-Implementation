using Godot;
using Date = System.DateOnly;
using System.Collections.Generic;
[Tool]
[GlobalClass]
public partial class BarChart : Chart
{
    private List<Date> dates = new List<Date>();
    private Date _minDate = Date.MaxValue, _maxDate = Date.MinValue;
    private Color[] colors;
    private bool writeValues;
    private Vector2 valueTextOffset;
    [Export]
    public Color[] Colors
    {
        get => colors;
        set
        {
            colors = value;
            QueueRedraw();
        }
    }
    [Export]
    public bool WriteValues
    {
        get => writeValues;
        set
        {
            writeValues = value;
            QueueRedraw();
        }
    }
    [Export]
    public Vector2 ValueTextOffset
    {
        get => valueTextOffset;
        set
        {
            valueTextOffset = value;
            QueueRedraw();
        }
    }
    public override void _Draw()
    {
        if (data == null || data.Count == 0 || colors == null || colors.Length == 0) return;

        float positiveRatio = (max <= 0 && min >= 0f) ? 1f : max / (max - min);
        positiveRatio = Mathf.Clamp(positiveRatio, 0f, 1f);
        if (min < 0f)
        {
            DrawLine(new Vector2(0f, positiveRatio * Size.Y), new Vector2(Size.X, Size.Y * positiveRatio), new Color(1f, 0f, 0f));
        }

        float xPerColumn = Size.X / data.Count, yPerValue = Size.Y / (Mathf.Max(max, 0f) - Mathf.Min(min, 0f));
        if (AbleToDrawByDate())
        {
            int[] order = DateOrder();
            for (int i = 0; i < data.Count; i++)
            {
                int index = order[i];
                DrawColumn(new Vector2(i * xPerColumn, Size.Y * positiveRatio), new Vector2(xPerColumn, -data[index] * yPerValue), colors[i % colors.Length], names[index]);
            }
            if (writeValues)
            {
                float height = positiveRatio * Size.Y;
                for (int i = 0; i < data.Count; i++)
                {
                    int index = order[i];
                    DrawString(GetThemeDefaultFont(), new Vector2(i * xPerColumn, height) + valueTextOffset, data[index].ToString());
                }
            }
        }
        else
        {
            for (int i = 0; i < data.Count; i++)
            {
                DrawColumn(new Vector2(i * xPerColumn, Size.Y * positiveRatio), new Vector2(xPerColumn, -data[i] * yPerValue), colors[i % colors.Length], names[i]);
            }
            if (writeValues)
            {
                float height = positiveRatio * Size.Y;
                for (int i = 0; i < data.Count; i++)
                {
                    DrawString(GetThemeDefaultFont(), new Vector2(i * xPerColumn, height) + valueTextOffset, data[i].ToString());
                }
            }
        }
    }
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
    private void DrawColumn(Vector2 begin, Vector2 offset, Color color, string text = null)
    {
        Vector2[] points = new Vector2[]
        {
            begin, begin + offset with {Y = 0f}, begin + offset, begin + offset with {X = 0f}
        };
        Vector2[] uvs = new Vector2[]
        {
            Vector2.Zero, Vector2.Down, Vector2.One, Vector2.Right
        };
        DrawPrimitive(points, new Color[] { color, color, color, color }, uvs);
        if (text != null)
        {
            DrawString(GetThemeDefaultFont(), (points[0] + points[3]) / 2f, text);
        }
    }
}