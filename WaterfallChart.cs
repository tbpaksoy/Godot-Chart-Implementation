using Godot;
using System;

[GlobalClass]
[Tool]
public partial class WaterfallChart : Chart
{
    [Export]
    public Color startAndEndColor, colorOnDecrease, colorOnIncrease;
    [Export]
    public bool writeValues;
    [Export]
    public int fontSize;

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
        float xPerColumn = size.X / (data.Count + 1), yPerValue = size.Y / (max - Mathf.Min(0f, min));
        float positiveToNegativeRatio = max / (max + Mathf.Abs(min));
        float zeroPoint = size.Y * positiveToNegativeRatio;
        if (xPerColumn == 0f || yPerValue == 0f) return;
        if (Mathf.Sign(min) >= 0f && Mathf.Sign(max) >= 0f)
        {
            DrawColumn(0, new Vector2(0f, size.Y), new Vector2(xPerColumn, -Mathf.Abs(yPerValue * data[0])), writeValues ? data[0].ToString() : null);
            float total = data[0] * yPerValue;
            for (int i = 1; i < data.Count; i++)
            {
                float diffrence = data[i - 1] - data[i];
                DrawColumn(i, new Vector2(xPerColumn * i, size.Y - total), new Vector2(xPerColumn, diffrence * yPerValue), diffrence < 0 ? colorOnIncrease : colorOnDecrease, writeValues ? (-diffrence).ToString() : null);
                total -= yPerValue * diffrence;
            }
            DrawColumn(data.Count - 1, size - new Vector2(xPerColumn, 0f), new Vector2(xPerColumn, -Mathf.Abs(yPerValue * data[^1])), writeValues ? data[^1].ToString() : null);
        }
        else if (Mathf.Sign(min) >= 0f ^ Mathf.Sign(max) >= 0f)
        {
            DrawColumn(0, new Vector2(0f, zeroPoint - data[0] * yPerValue), new Vector2(xPerColumn, yPerValue * data[0]), writeValues ? data[0].ToString() : null);
            float total = zeroPoint - data[0] * yPerValue;
            for (int i = 1; i < data.Count; i++)
            {
                float diffrence = data[i - 1] - data[i];
                DrawColumn(i, new Vector2(xPerColumn * i, total), new Vector2(xPerColumn, diffrence * yPerValue), diffrence < 0 ? colorOnIncrease : colorOnDecrease, writeValues ? (-diffrence).ToString() : null);
                total += yPerValue * diffrence;
            }
            DrawColumn(data.Count - 1, new Vector2(size.X - xPerColumn, zeroPoint - yPerValue * data[^1]), new Vector2(xPerColumn, yPerValue * data[^1]), writeValues ? data[^1].ToString() : null);
        }
        if (min < 0f) DrawLine(new Vector2(0f, zeroPoint), new Vector2(size.X, zeroPoint), new Color(1f, 0f, 0f), width: 2f);


    }
    private void DrawColumn(int index, Vector2 begin, Vector2 offset, string text = null)
    {
        float f = data[index];
        Vector2[] points = new Vector2[4];
        points[0] = begin;
        points[1] = begin + offset with { Y = 0 };
        points[2] = begin + offset;
        points[3] = begin + offset with { X = 0 };
        DrawPolygon(points, new Color[] { startAndEndColor, startAndEndColor, startAndEndColor, startAndEndColor });
        if (text == null) return;
        DrawString(GetThemeDefaultFont(), (points[0] + points[3]) / 2f, text, alignment: HorizontalAlignment.Fill, fontSize: fontSize);
    }
    private void DrawColumn(int index, Vector2 begin, Vector2 offset, Color color, string text = null)
    {
        float f = data[index];
        Vector2[] points = new Vector2[4];
        points[0] = begin;
        points[1] = begin + offset with { Y = 0 };
        points[2] = begin + offset;
        points[3] = begin + offset with { X = 0 };
        DrawPolygon(points, new Color[] { color, color, color, color });
        if (text == null) return;
        DrawString(GetThemeDefaultFont(), (points[0] + points[3]) / 2f, text, alignment: HorizontalAlignment.Fill, fontSize: fontSize);
    }
}
