using Godot;
using System.Collections.Generic;
using System;
[Tool]
public partial class PieChart : Chart
{
    [Export]
    public Color[] colors;
    public override void _Draw()
    {
        float total = 0;
        foreach (float f in data)
            total += f;
        float t = 0f;
        Stack<(float, float)> angles = new Stack<(float, float)>();
        for (int i = 0; i < data.Count; i++)
        {
            float first = t / total;
            t += data[i];
            float second = t / total;
            angles.Push((first, second));
        }
        Vector2 s = GetRect().Size;
        Vector2 center = s / 2f;
        float size = Mathf.Min(s.X, s.Y) / 2;
        int index = 0;
        while (angles.Count > 0)
        {
            (float, float) temp = angles.Pop();
            index = (index + 1) % colors.Length;
            DrawArcPoly(center, size, temp.Item1 * 360f, temp.Item2 * 360f, colors[index]);
        }
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
}