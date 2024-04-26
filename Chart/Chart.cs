using Godot;
using Godot.Collections;
using System.Collections.Generic;
using Date = System.DateOnly;
[GlobalClass]
public abstract partial class Chart : Control
{
    protected Array<float> data = new Array<float>();
    protected Array<string> names = new Array<string>();
    protected Array<ChartDataSource> sources = new Array<ChartDataSource>();
    protected float min = float.MaxValue, max = float.MinValue;
    public virtual void Add(ChartDataSource source)
    {
        sources.Add(source);
        data.Add(source.value);
        names.Add(source.name);
        if (source.value < min) min = source.value;
        if (source.value > max) max = source.value;
    }
    public virtual void Remove(ChartDataSource source)
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
    }
    public virtual void UpdateProp(ChartDataSource source)
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
    public virtual void Update()
    {
        data.Clear();
        names.Clear();
        sources.Clear();
        max = float.MinValue;
        min = float.MaxValue;
        foreach (Node node in GetChildren())
            if (node is ChartDataSource cds) Add(cds);
        QueueRedraw();
    }
    protected virtual bool AbleToDrawByDate()
    {
        foreach (Node node in GetChildren())
        {
            if (node is ChartDataSource cds && cds is not ChartDataSourceWithDate) return false;
        }
        return true;
    }
    public override void _Ready()
    {
        ChildOrderChanged += Update;
    }
    protected int[] DateOrder(bool descending = false)
    {
        List<(Date, int)> dates = new List<(Date, int)>();
        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i] is ChartDataSourceWithDate cds)
            {
                dates.Add((cds.date, i));
            }
        }
        dates.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        if (descending) dates.Reverse();
        int[] order = new int[dates.Count];
        for (int i = 0; i < dates.Count; i++)
        {
            order[i] = dates[i].Item2;
        }
        return order;
    }
}