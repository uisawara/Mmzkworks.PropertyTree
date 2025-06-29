namespace works.mmzk.PropertyTree;

public class FloatProperty : BaseValueProperty<float>, IHasValueRange<float>
{
    public FloatProperty(string name) : base(name)
    {
        base.Set(0f);
        Min = float.MinValue;
        Max = float.MaxValue;
    }

    public FloatProperty(string name, float initialValue) : base(name)
    {
        base.Set(initialValue);
        Min = float.MinValue;
        Max = float.MaxValue;
    }

    public FloatProperty(string name, float initialValue, float min, float max) : base(name)
    {
        Min = min;
        Max = max;
        base.Set(ClampValue(initialValue));
    }

    public float Min { get; }
    public float Max { get; }

    protected override float Get()
    {
        return base.Get();
    }

    protected override void Set(float value)
    {
        base.Set(ClampValue(value));
    }

    private float ClampValue(float value)
    {
        if (value < Min) return Min;
        if (value > Max) return Max;
        return value;
    }
}