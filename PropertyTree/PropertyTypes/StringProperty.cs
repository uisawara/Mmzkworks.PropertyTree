namespace works.mmzk.PropertyTree;

public class StringProperty : BaseValueProperty<string>
{
    public StringProperty(string name) : base(name)
    {
        base.Set(string.Empty);
    }

    public StringProperty(string name, string initialValue) : base(name)
    {
        base.Set(initialValue);
    }
}