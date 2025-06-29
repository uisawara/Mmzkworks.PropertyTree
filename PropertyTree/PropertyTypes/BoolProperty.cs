namespace works.mmzk.PropertyTree;

public class BoolProperty : BaseValueProperty<bool>
{
    public BoolProperty(string name) : base(name)
    {
        base.Set(false);
    }

    public BoolProperty(string name, bool initialValue) : base(name)
    {
        base.Set(initialValue);
    }
}