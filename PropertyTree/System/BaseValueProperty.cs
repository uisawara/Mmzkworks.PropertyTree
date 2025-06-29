namespace works.mmzk.PropertyTree;

public abstract class BaseValueProperty<T> : BaseProperty
{
    private bool _isInitialized;
    private T _value;

    protected BaseValueProperty(string name) : base(name)
    {
    }

    public T Value
    {
        get => Get();
        set => Set(value);
    }

    protected virtual T Get()
    {
        return _value;
    }

    protected virtual void Set(T value)
    {
        var oldValue = _value;
        _value = value;

        // Fire Updated event if value changed or on initial setting
        if (!_isInitialized || !EqualityComparer<T>.Default.Equals(oldValue, value))
        {
            _isInitialized = true;
            OnUpdated();
        }
    }
}