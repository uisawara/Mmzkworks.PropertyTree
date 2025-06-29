namespace works.mmzk.PropertyTree;

public class StringPropertyAdapter : BaseValueProperty<string>
{
    private readonly Func<string> _getter;
    private readonly Action<string> _setter;

    public StringPropertyAdapter(string name, Func<string> getter, Action<string> setter)
        : base(name)
    {
        _getter = getter;
        _setter = setter;
    }

    public StringPropertyAdapter(StringProperty property)
        : base(property.Name)
    {
        _getter = () => property.Value;
        _setter = f => property.Value = f;
    }

    protected override string Get()
    {
        return _getter();
    }

    protected override void Set(string value)
    {
        base.Set(value);
        _setter(value);
    }
}