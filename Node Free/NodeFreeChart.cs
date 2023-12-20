using Godot;
using Godot.Collections;
[GlobalClass]
public partial class NodeFreeChart : Control
{
    protected Array<float> data = new Array<float>();
    protected Array<string> names = new Array<string>();
    [Export]
    public Array<float> Data
    {
        get => data;
        set
        {
            data = value;
            QueueRedraw();
        }
    }
    [Export]
    public Array<string> Names
    {
        get => names;
        set
        {
            names = value;
            QueueRedraw();
        }
    }
    public float min
    {
        get
        {
            float min = float.MaxValue;
            foreach (float f in data)
            {
                if (f < min) min = f;
            }
            return min;
        }
    }
    public float max
    {
        get
        {
            float max = float.MinValue;
            foreach (float f in data)
            {
                if (f > max) max = f;
            }
            return max;
        }
    }

}
