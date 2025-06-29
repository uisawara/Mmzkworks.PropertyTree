namespace works.mmzk.PropertyTree;

public class IntProperty : BaseValueProperty<int>, IHasValueRange<int>
{
    public IntProperty(string name) : base(name)
    {
        base.Set(0);
        Min = int.MinValue;
        Max = int.MaxValue;
    }

    public IntProperty(string name, int initialValue) : base(name)
    {
        base.Set(initialValue);
        Min = int.MinValue;
        Max = int.MaxValue;
    }

    public IntProperty(string name, int initialValue, int min, int max) : base(name)
    {
        Min = min;
        Max = max;
        base.Set(ClampValue(initialValue));
    }

    public int Min { get; }
    public int Max { get; }

    protected override int Get()
    {
        return base.Get();
    }

    protected override void Set(int value)
    {
        base.Set(ClampValue(value));
    }

    private int ClampValue(int value)
    {
        if (value < Min) return Min;
        if (value > Max) return Max;
        return value;
    }
}