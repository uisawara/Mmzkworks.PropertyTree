namespace works.mmzk.PropertyTree;

public interface IProperty
{
    string Name { get; }
    PropertyGroup Parent { get; set; }

    event Action<IProperty> Updated;
    event Action<IProperty> ChildUpdated;
}