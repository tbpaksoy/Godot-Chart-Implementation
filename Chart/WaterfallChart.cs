using Godot;
using System.Collections.Generic;
using Date = System.DateOnly;
[GlobalClass, Tool]
public partial class WaterfallChart : Chart
{
    #region data
    private List<Date> dates = new List<Date>();
    private Date _minDate = Date.MaxValue, _maxDate = Date.MinValue;
    #endregion
    #region display options
    private Color _startAndEndColor, _colorOnDecrease, _colorOnIncrease;
    private bool _writeValues;
    private int _fontSize;
    private Vector2 _valueTextOffset;
    [Export]
    public Color startAndEndColor
    {
        get => _startAndEndColor;
        set
        {
            _startAndEndColor = value;
            QueueRedraw();
        }
    }
    [Export]
    public Color colorOnDecrease
    {
        get => _colorOnDecrease;
        set
        {
            _colorOnDecrease = value;
            QueueRedraw();
        }
    }
    [Export]
    public Color colorOnIncrease
    {
        get => _colorOnIncrease;
        set
        {
            _colorOnIncrease = value;
            QueueRedraw();
        }
    }
    [Export]
    public bool writeValues
    {
        get => _writeValues;
        set
        {
            _writeValues = value;
            QueueRedraw();
        }
    }
    [Export]
    public int fontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            QueueRedraw();
        }
    }
    [Export]
    public Vector2 valueTextOffset
    {
        get => _valueTextOffset;
        set
        {
            _valueTextOffset = value;
            QueueRedraw();
        }
    }
    #endregion
    #region Godot methods
    public override void _Draw()
    {

        if (data.Count < 3) return;
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
        DrawString(GetThemeDefaultFont(), (points[0] + points[3]) / 2f + valueTextOffset, text, alignment: HorizontalAlignment.Fill, fontSize: fontSize);
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
        DrawString(GetThemeDefaultFont(), (points[0] + points[3]) / 2f + valueTextOffset, text, alignment: HorizontalAlignment.Fill, fontSize: fontSize);
    }
    #endregion
}
