using Godot;
using Godot.Collections;

public abstract partial class Chart : Control
{
    [Export(PropertyHint.ArrayType)]
    protected Array<float> data = new Array<float>();
}
