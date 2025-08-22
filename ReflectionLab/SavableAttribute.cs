namespace ReflectionLab;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SavableAttribute(string? name = null) : Attribute
{
    public string? Name { get; } = name;
}