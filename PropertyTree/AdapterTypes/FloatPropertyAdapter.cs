namespace works.mmzk.PropertyTree;

public class FloatPropertyAdapter : BaseValueProperty<float>, IHasValueRange<float>
{
    private readonly Func<float> _getter;
    private readonly Action<float> _setter;

    public FloatPropertyAdapter(string name, float min, float max, Func<float> getter, Action<float> setter)
        : base(name)
    {
        Min = min;
        Max = max;
        _getter = getter;
        _setter = setter;
    }

    public FloatPropertyAdapter(FloatProperty property)
        : base(property.Name)
    {
        Min = property.Min;
        Max = property.Max;
        _getter = () => property.Value;
        _setter = f => property.Value = f;
    }

    public float Min { get; }
    public float Max { get; }

    protected override float Get()
    {
        return _getter();
    }

    protected override void Set(float value)
    {
        base.Set(value);
        _setter(value);
    }
}