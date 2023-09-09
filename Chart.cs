using Godot;
using Godot.Collections;
using SourceAndDate = System.Collections.Generic.Dictionary<ChartDataSource, System.DateOnly>;
using Enumerable = System.Linq.Enumerable;
public abstract partial class Chart : Control
{
    protected Array<float> data = new Array<float>();
    protected Array<string> names = new Array<string>();
    protected Array<ChartDataSource> sources = new Array<ChartDataSource>();
    protected SourceAndDate sourceAndDate = new SourceAndDate();
    protected bool ableToOrderByDate { get; private set; } = false;
    public void Add(ChartDataSource source)
    {
        sources.Add(source);
        data.Add(source.value);
        names.Add(source.name);
        if (source.date != null)
            sourceAndDate.Add(source, (System.DateOnly)source.date);
        else ableToOrderByDate = false;
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


        if (redraw) QueueRedraw();
    }
}