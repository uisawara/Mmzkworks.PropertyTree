namespace works.mmzk.PropertyTree;

public class BoolPropertyAdapter : BaseValueProperty<bool>
{
    private readonly Func<bool> _getter;
    private readonly Action<bool> _setter;

    public BoolPropertyAdapter(string name, Func<bool> getter, Action<bool> setter)
        : base(name)
    {
        _getter = getter;
        _setter = setter;
    }

    public BoolPropertyAdapter(BoolProperty property)
        : base(property.Name)
    {
        _getter = () => property.Value;
        _setter = f => property.Value = f;
    }

    protected override bool Get()
    {
        return _getter();
    }

    protected override void Set(bool value)
    {
        base.Set(value);
        _setter(value);
    }
}