namespace works.mmzk.PropertyTree;

public interface IHasValueRange<T>
{
    public T Min { get; }
    public T Max { get; }
}