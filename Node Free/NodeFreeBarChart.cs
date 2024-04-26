using Godot;
[GlobalClass, Tool]
public partial class NodeFreeBarChart : NodeFreeChart
{
    #region display options
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
    #endregion
    #region Godot methods
    public override void _Draw()
    {
        float min = float.MaxValue, max = float.MinValue;
        foreach (float f in data)
        {
            if (f < min) min = f;
            if (f > max) max = f;
        }
        if (data == null || data.Count == 0 || colors == null || colors.Length == 0) return;
        float positiveRatio = (max <= 0 && min >= 0f) ? 1f : max / (max - min);
        positiveRatio = Mathf.Clamp(positiveRatio, 0f, 1f);
        if (min < 0f)
        {
            DrawLine(new Vector2(0f, positiveRatio * Size.Y), new Vector2(Size.X, Size.Y * positiveRatio), new Color(1f, 0f, 0f));
        }
        float xPerColumn = Size.X / data.Count, yPerValue = Size.Y / (Mathf.Max(max, 0f) - Mathf.Min(min, 0f));
        for (int i = 0; i < data.Count; i++)
        {
            string name = null;
            try { name = names[i]; }
            catch { }
            DrawColumn(new Vector2(i * xPerColumn, Size.Y * positiveRatio), new Vector2(xPerColumn, -data[i] * yPerValue), colors[i % colors.Length], name);
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
    #endregion
    #region methods
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
    #endregion
}