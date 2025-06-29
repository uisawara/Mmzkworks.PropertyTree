namespace works.mmzk.PropertyTree;

public class EnumProperty<T> : BaseValueProperty<T> where T : Enum
{
    public EnumProperty(string name) : base(name)
    {
        base.Set(default);
    }

    public EnumProperty(string name, T initialValue) : base(name)
    {
        base.Set(initialValue);
    }
}