using Godot;
using Godot.Collections;
using SourceAndDate = System.Collections.Generic.Dictionary<ChartDataSource, System.DateOnly>;
using Enumerable = System.Linq.Enumerable;
[GlobalClass]
public abstract partial class Chart : Control
{
    protected Array<float> data = new Array<float>();
    protected Array<string> names = new Array<string>();
    protected Array<ChartDataSource> sources = new Array<ChartDataSource>();
    protected SourceAndDate sourceAndDate = new SourceAndDate();
    protected bool ableToOrderByDate { get; private set; } = false;
    protected float min = float.MaxValue, max = float.MinValue;
    public void Add(ChartDataSource source)
    {
        sources.Add(source);
        data.Add(source.value);
        names.Add(source.name);
        if (source.date != null)
            sourceAndDate.Add(source, (System.DateOnly)source.date);
        else ableToOrderByDate = false;
        if (source.value < min) min = source.value;
        if (source.value > max) max = source.value;
    }
    public void Remove(ChartDataSource source)
    {
        int index = sources.IndexOf(source);
        if (index == -1) return;
        Remove(index);
    }
    public void Remove(int index)
    {
        if (index >= sources.Count) return;
        ChartDataSource cds = sources[index];
        sources.RemoveAt(index);
        data.RemoveAt(index);
        names.RemoveAt(index);
        if (sourceAndDate.ContainsKey(cds)) sourceAndDate.Remove(cds);
        ableToOrderByDate = sources.Count == sourceAndDate.Count && Enumerable.SequenceEqual(sources, sourceAndDate.Keys);
    }
    public void UpdateProp(ChartDataSource source)
    {
        bool redraw = false;
        int index = sources.IndexOf(source);
        if (index == -1 || index >= sources.Count) return;
        redraw |= data[index] == source.value;
        data[index] = source.value;
        redraw |= names[index] == source.name;
        names[index] = source.name;
        if (data[index] > max) max = data[index];
        else if (data[index] < min) min = data[index];
        else
        {
            max = float.MinValue;
            min = float.MaxValue;
            foreach (float f in data)
            {
                if (max < f) max = f;
                if (min > f) min = f;
            }
        }
        if (redraw) QueueRedraw();
    }
    public void Update()
    {
        data.Clear();
        names.Clear();
        sources.Clear();
        sourceAndDate.Clear();
        max = float.MinValue;
        min = float.MaxValue;
        foreach (Node node in GetChildren())
            if (node is ChartDataSource cds) Add(cds);
        QueueRedraw();
    }
    public override void _Ready()
    {
        ChildOrderChanged += Update;
    }
}