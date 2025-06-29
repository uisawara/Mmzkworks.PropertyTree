using System.Linq;

namespace works.mmzk.PropertyTree;

public sealed class PropertyGroup : BaseProperty
{
    private readonly List<IProperty> _items = new();

    public PropertyGroup(string name, IEnumerable<IProperty> items = null) : base(name)
    {
        if (items != null) AddRange(items);
    }

    public IReadOnlyList<IProperty> Items => _items.AsReadOnly();

    // PropertyGroup-specific events
    public event Action<IProperty> Added;
    public event Action<IProperty> Removed;

    public void Add(IProperty property)
    {
        property.Parent = this;
        _items.Add(property);
        // Fire Added event
        Added?.Invoke(property);
    }

    public void AddRange(IEnumerable<IProperty> items)
    {
        foreach (var item in items)
        {
            if (item is BaseProperty baseProperty) baseProperty.Parent = this;
            _items.Add(item);
            // Fire Added event
            Added?.Invoke(item);
        }
    }

    public void Remove(IProperty property)
    {
        property.Parent = null;
        _items.Remove(property);
        // Fire Removed event
        Removed?.Invoke(property);
    }

    public void ClearAll()
    {
        foreach (var item in _items)
        {
            if (item is IProperty baseProperty) baseProperty.Parent = null;
            // Fire Removed event
            Removed?.Invoke(item);
        }

        _items.Clear();
    }

    /// <summary>
    /// Searches for a property with the specified name
    /// </summary>
    /// <param name="name">Property name</param>
    /// <returns>Found property, or null if not found</returns>
    public IProperty FindByName(string name)
    {
        return _items.FirstOrDefault(p => p.Name == name);
    }

    /// <summary>
    /// Checks if a property with the specified name exists
    /// </summary>
    /// <param name="name">Property name</param>
    /// <returns>True if it exists</returns>
    public bool HasProperty(string name)
    {
        return _items.Any(p => p.Name == name);
    }



    public IProperty At(int index)
    {
        return _items[index];
    }

    public T At<T>(int index)
    {
        return (T)_items[index];
    }
}