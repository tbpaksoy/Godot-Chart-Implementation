using Godot;
using System.Collections.Generic;
using Date = System.DateOnly;

#if Test
using System.Linq;
using Date = System.DateOnly;
using DateList = System.Collections.Generic.List<System.DateOnly>;
using DateAndIndex = System.Collections.Generic.Dictionary<System.DateOnly, int>;
#endif
[GlobalClass]
[Tool]
public partial class GraphChart : Chart
{
    private List<Date> dates = new List<Date>();
    private Vector2 _offset;
    private bool _drawGrid;
    private Vector2 _gridLength;
    private Color _lineColor, _nodeColor, _gridColor;
    private bool _drawValue;
    private Vector2 _valueOffset, _nameOffset;
    private Color _valueColor, _nameColor;
    private float _lineThickness, _nodeRadius;
    [Export]
    public Vector2 offset
    {
        get => _offset;
        set
        {
            _offset = value;
            QueueRedraw();
        }
    }
    [Export]
    public bool drawGrid
    {
        get => _drawGrid;
        set
        {
            _drawGrid = value;
            QueueRedraw();
        }
    }
    [Export]
    public Vector2 gridLength
    {
        get => _gridLength;
        set
        {
            _gridLength = value;
            QueueRedraw();
        }
    }
    [Export]
    public Color lineColor
    {
        get => _lineColor;
        set
        {
            _lineColor = value;
            QueueRedraw();
        }
    }
    [Export]
    public Color nodeColor
    {
        get => _nodeColor;
        set
        {
            _nodeColor = value;
            QueueRedraw();
        }
    }
    [Export]
    public Color gridColor
    {
        get => _gridColor;
        set
        {
            _gridColor = value;
            QueueRedraw();
        }
    }
    [Export]
    public bool drawValue
    {
        get => _drawValue;
        set
        {
            _drawValue = value;
            QueueRedraw();
        }
    }
    [Export]
    public Vector2 valueOffset
    {
        get => _valueOffset;
        set
        {
            _valueOffset = value;
            QueueRedraw();
        }
    }
    [Export]
    public Vector2 nameOffset
    {
        get => _nameOffset;
        set
        {
            _nameOffset = value;
            QueueRedraw();
        }
    }
    [Export]
    public Color valueColor
    {
        get => _valueColor;
        set
        {
            _valueColor = value;
            QueueRedraw();
        }
    }
    [Export]
    public Color nameColor
    {
        get => _nameColor;
        set
        {
            _nameColor = value;
            QueueRedraw();
        }
    }
    [Export]
    public float lineThickness
    {
        get => _lineThickness;
        set
        {
            _lineThickness = value;
            QueueRedraw();
        }
    }
    [Export]
    public float nodeRadius
    {
        get => _nodeRadius;
        set
        {
            _nodeRadius = value;
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        if (sources == null || sources.Count == 0) return;
        List<Vector2> points = new List<Vector2>();
        Vector2 originalSize = GetRect().Size;
        Vector2 size = originalSize - offset;
        if (drawGrid && gridColor.A > 0 && gridLength.X >= 1f && gridLength.Y >= 1f)
        {
            for (float i = size.Y; i >= offset.Y; i -= gridLength.Y) DrawLine(new Vector2(offset.X, i), new Vector2(size.X, i), gridColor);
            for (float j = offset.X; j <= size.X; j += gridLength.X) DrawLine(new Vector2(j, offset.Y), new Vector2(j, size.Y), gridColor);
        }
        if (min < 0)
        {
            float zeroPoint = -min / (min - max) * size.Y + size.Y + offset.Y / 2f;
            DrawLine(new Vector2(offset.X / 2f, zeroPoint), new Vector2(size.X + offset.X / 2f, zeroPoint), new Color(1f, 0f, 0f), 2f);
        }
        for (int i = 0; i < points.Count - 1; i++)
        {

            DrawLine(points[i], points[i + 1], lineColor, lineThickness, false);
            DrawCircle(points[i], nodeRadius, nodeColor);
            if (!string.IsNullOrWhiteSpace(names[i])) DrawString(GetThemeDefaultFont(), points[i] + nameOffset, names[i]);
        }
        try
        {
            DrawCircle(points[^1], nodeRadius, nodeColor);
        }
        catch
        {

        }
        if (drawValue)
        {
            for (int i = 0; i < data.Count; i++)
            {
                DrawString(GetThemeDefaultFont(), points[i] + valueOffset, data[i].ToString(), modulate: valueColor);
            }
        }
    }
}