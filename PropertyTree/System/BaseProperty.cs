namespace works.mmzk.PropertyTree;

public abstract class BaseProperty : IProperty
{
    // Stack for circular reference detection
    private static readonly HashSet<IProperty> _updateStack = new();

    private PropertyGroup _parent;

    protected BaseProperty(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public PropertyGroup Parent
    {
        get => _parent;
        set
        {
            if (_parent != value)
            {
                _parent = value;
                // Propagate ChildUpdated event to parent when Parent changes
                NotifyParentOfChildUpdate();
            }
        }
    }

    // Event implementation
    public event Action<IProperty> Updated;
    public event Action<IProperty> ChildUpdated;

    // Method to fire Updated event
    protected virtual void OnUpdated()
    {
        Updated?.Invoke(this);
        // Propagate ChildUpdated event to parent
        NotifyParentOfChildUpdate();
    }

    // Method to fire ChildUpdated event
    protected virtual void OnChildUpdated(IProperty child)
    {
        // Detect circular reference
        if (_updateStack.Contains(this)) return; // Stop processing if circular reference detected

        try
        {
            _updateStack.Add(this);
            ChildUpdated?.Invoke(child);
            // Propagate ChildUpdated event to parent
            NotifyParentOfChildUpdate();
        }
        finally
        {
            _updateStack.Remove(this);
        }
    }

    // Method to propagate ChildUpdated event to parent
    private void NotifyParentOfChildUpdate()
    {
        if (Parent != null && !_updateStack.Contains(Parent)) Parent.OnChildUpdated(this);
    }
}