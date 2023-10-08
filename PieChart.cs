using Godot;
using System.Collections.Generic;
[GlobalClass]
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
        /*while (angles.Count > 0)
        {
            (float, float) temp = angles.Pop();
            DrawArcPoly(center, size, temp.Item1 * 360f, temp.Item2 * 360f, colors[index % colors.Length]);
            index = (index + 1) % colors.Length;
        }*/
        for (int i = 0; i < data.Count; i++)
        {
            DrawArcPoly(center, size, angles[i].Item1 * 360f, angles[i].Item2 * 360f, colors[i % colors.Length]);
        }
        for (int i = 0; i < data.Count; i++)
        {
            DrawValue(center, data[i].ToString(), angles[i].Item1 * 360f, angles[i].Item2 * 360, size / 2f);
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
    private void DrawValue(Vector2 center, string text, float fromAngle, float toAngle, float distance)
    {
        float middleAngle = (toAngle + fromAngle) / 2f;
        GD.Print(middleAngle);
        DrawString(GetThemeDefaultFont(), center + new Vector2(Mathf.Cos(Mathf.DegToRad(middleAngle - 90f)), Mathf.Sin(Mathf.DegToRad(middleAngle - 90f))) * distance, text, alignment: HorizontalAlignment.Center);
    }
}