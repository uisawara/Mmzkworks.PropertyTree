namespace works.mmzk.PropertyTree;

public class IntPropertyAdapter : BaseValueProperty<int>, IHasValueRange<int>
{
    private readonly Func<int> _getter;
    private readonly Action<int> _setter;

    public IntPropertyAdapter(string name, int min, int max, Func<int> getter, Action<int> setter)
        : base(name)
    {
        Min = min;
        Max = max;
        _getter = getter;
        _setter = setter;
    }

    public IntPropertyAdapter(IntProperty property)
        : base(property.Name)
    {
        Min = property.Min;
        Max = property.Max;
        _getter = () => property.Value;
        _setter = f => property.Value = f;
    }

    public int Min { get; }
    public int Max { get; }

    protected override int Get()
    {
        return _getter();
    }

    protected override void Set(int value)
    {
        base.Set(value);
        _setter(value);
    }
}