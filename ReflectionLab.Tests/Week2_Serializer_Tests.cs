namespace ReflectionLab.Tests;
    
public class Week2_Serializer_Tests
{
    [Fact]
    public void Serialize_ReturnsOnlyMarked_MappingAlias_AndReadsPrivates()
    {
        var s = new SaveMe { Level = 7, Nick = "kit" };
        var dict = SavableSerializer.Serialize(s);

        Assert.True(dict.ContainsKey("Level"));
        Assert.True(dict.ContainsKey("nick"));   // alias de Nick
        Assert.True(dict.ContainsKey("_xp"));    // privado marcado
        Assert.False(dict.ContainsKey("Gold"));  // sin atributo

        Assert.Equal(7, dict["Level"]);
        Assert.Equal("kit", dict["nick"]);
        Assert.Equal(350, dict["_xp"]);
    }

    [Fact]
    public void Serialize_CachedPerType_DoesNotChangeKeysSecondTime()
    {
        var first  = SavableSerializer.Serialize(new SaveMe());
        var second = SavableSerializer.Serialize(new SaveMe());
        Assert.True(first.Keys.SequenceEqual(second.Keys));
    }
}

internal class SaveMe
{
    [Savable] public int Level = 3;
    [Savable("nick")] public string Nick { get; set; } = "cat";
    [Savable] private int _xp = 350;
    public int Gold = 100; // no se guarda
}