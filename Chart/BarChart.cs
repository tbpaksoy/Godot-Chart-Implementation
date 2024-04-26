using Godot;
using Date = System.DateOnly;
using System.Collections.Generic;
[GlobalClass, Tool]
public partial class BarChart : Chart
{
    #region data
    private List<Date> dates = new List<Date>();
    private Date _minDate = Date.MaxValue, _maxDate = Date.MinValue;
    #endregion
    #region display options
    private Color[] colors;
    private bool writeValues, writeDates;
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
    public bool WriteDates
    {
        get => writeDates;
        set
        {
            writeDates = value;
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
    #endregion
    #region Godot methods
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
                Vector2 begin = new Vector2(i * xPerColumn, Size.Y * positiveRatio);
                Vector2 offset = new Vector2(xPerColumn, -data[index] * yPerValue);
                DrawColumn(begin, offset, colors[i % colors.Length], out Vector2 top, out Vector2 middle, out Vector2 down);
                string[] texts = new string[] { null, null, null };
                if (writeValues)
                    switch (data[index])
                    {
                        case < 0:
                            texts[2] = data[index].ToString();
                            break;
                        case > 0:
                            texts[0] = data[index].ToString();
                            break;
                    }
                if (writeDates)
                    texts[1] = dates[index].ToString();
                if (data[index] < 0) texts[0] = names[index].ToString();
                else if (data[index] > 0) texts[2] = names[index].ToString();
                DrawTexts(texts[0], texts[1], texts[2], top, middle, down);
            }
        }
        else
        {
            for (int i = 0; i < data.Count; i++)
            {
                Vector2 begin = new Vector2(i * xPerColumn, Size.Y * positiveRatio);
                Vector2 offset = new Vector2(xPerColumn, -data[i] * yPerValue);
                DrawColumn(begin, offset, colors[i % colors.Length], out Vector2 top, out Vector2 middle, out Vector2 down);
                string[] texts = new string[] { null, null, null };
                if (writeValues)
                    switch (data[i])
                    {
                        case < 0:
                            texts[2] = data[i].ToString();
                            break;
                        case > 0:
                            texts[0] = data[i].ToString();
                            break;
                    }
                if (data[i] < 0) texts[0] = names[i].ToString();
                else if (data[i] > 0) texts[2] = names[i].ToString();
                DrawTexts(texts[0], texts[1], texts[2], top, middle, down);
            }
        }
    }
    #endregion
    #region methods
    public override void Add(ChartDataSource source)
    {
        base.Add(source);
        if (source is ChartDataSourceWithDate cdsd)
        {
            dates.Add(cdsd.date);
        }
    }
    public override void Remove(ChartDataSource source)
    {
        int index = sources.IndexOf(source);
        base.Remove(source);
        if (index == -1) return;
        Remove(index);
    }
    private void DrawColumn(Vector2 begin, Vector2 offset, Color color, out Vector2 top, out Vector2 middle, out Vector2 down, string text = null)
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
        middle = (points[0] + points[2]) / 2f;
        top = middle with { Y = Mathf.Min(begin.Y, begin.Y + offset.Y) };
        down = middle with { Y = Mathf.Max(begin.Y, begin.Y + offset.Y) };
    }
    private void DrawTexts(string topText, string middleText, string downText, Vector2 top, Vector2 middle, Vector2 down, Color? color = null)
    {
        if (topText != null) DrawString(GetThemeDefaultFont(), top, topText, HorizontalAlignment.Center, modulate: color);
        if (middleText != null) DrawString(GetThemeDefaultFont(), middle, middleText, HorizontalAlignment.Center, modulate: color);
        if (downText != null) DrawString(GetThemeDefaultFont(), down, downText, HorizontalAlignment.Center, modulate: color);
    }
    public override void Update()
    {
        dates.Clear();
        base.Update();
    }
    #endregion
}