namespace works.mmzk.PropertyTree;

public class ActionProperty : BaseProperty
{
    private readonly Action _action;

    public ActionProperty(string name, Action action = default) : base(name)
    {
        _action = action;
    }

    /// <summary>
    /// Gets the Action held by this ActionProperty
    /// </summary>
    public Action Action => _action;

    public void Execute()
    {
        _action?.Invoke();
        // Fire Updated event when action is executed
        OnUpdated();
    }
}