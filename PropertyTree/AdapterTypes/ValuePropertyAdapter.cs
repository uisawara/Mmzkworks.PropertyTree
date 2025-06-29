namespace works.mmzk.PropertyTree;

public class ValuePropertyAdapter<T> : BaseValueProperty<T>, IHasValueRange<T>
{
    private readonly Func<T> _getter;
    private readonly Action<T> _setter;

    public ValuePropertyAdapter(string name, T min, T max, Func<T> getter, Action<T> setter)
        : base(name)
    {
        Min = min;
        Max = max;
        _getter = getter;
        _setter = setter;
    }

    public T Min { get; }
    public T Max { get; }

    protected override T Get()
    {
        return _getter();
    }

    protected override void Set(T value)
    {
        _setter(value);
        OnUpdated();
    }
}