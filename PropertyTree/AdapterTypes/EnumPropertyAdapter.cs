namespace works.mmzk.PropertyTree;

public class EnumPropertyAdapter<T> : BaseValueProperty<T> where T : struct, Enum
{
    private readonly Func<T> _getter;
    private readonly Action<T> _setter;

    public EnumPropertyAdapter(string name, Func<T> getter, Action<T> setter)
        : base(name)
    {
        _getter = getter;
        _setter = setter;
    }

    protected override T Get()
    {
        return _getter();
    }

    protected override void Set(T value)
    {
        base.Set(value);
        _setter(value);
    }
}