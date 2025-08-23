namespace ReflectionLab.Tests;

public class Week2_Serializer_Indexer_Tests
{
    [Fact]
    public void Serialize_SkipsIndexedProperties()
    {
        var x = new WithIndexer();
        var dict = SavableSerializer.Serialize(x);
        Assert.True(dict.ContainsKey("Plain"));
        Assert.DoesNotContain("Item", dict.Keys); // nombre interno del indexer
    }
}

internal class WithIndexer
{
    [Savable] public int Plain { get; set; } = 1;

    // Aun si le ponés [Savable], el serializer la ignora por ser indexada
    [Savable] public int this[int i] { get => i; set { } }
}