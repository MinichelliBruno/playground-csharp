namespace ReflectionLab;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class RangeIntAttribute(int min, int max) : Attribute
{
    public int Min { get; } = min;
    public int Max { get; } = max;
}