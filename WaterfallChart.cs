using Godot;
using System;

public partial class WaterfallChart : Chart
{
    [Export]
    public Color startAndEndColor;
    [Export]
    public bool writeDelta;
    public int deltaFontSize;
    public override void _Draw()
    {
        if (data.Count < 3) return;
        float min = float.MaxValue, max = float.MinValue;
        foreach (float f in data)
        {
            if (min > f) min = f;
            if (max < f) max = f;
        }
        Vector2 size = GetRect().Size;
        float xPerColumn = size.X / data.Count, yPerValue = size.Y / (max - min);
        if (xPerColumn == 0f || yPerValue == 0f) return;
        if (min < 0f)
        {
            float zeroPoint = -min / (min - max) * size.Y + size.Y;
            DrawLine(new Vector2(0f, zeroPoint), new Vector2(size.X, zeroPoint), new Color(1f, 0f, 0f));
        }
        for (int i = 1; i < data.Count - 1; i++)
        {

        }
    }
    private void DrawColumn(int index, Color color, float width, float heigthPerValue, Vector2 rectSize, bool writeDelta = false, int compareWith = -1)
    {
        float f = data[index];
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(index * width, f * heigthPerValue);
    }
}
