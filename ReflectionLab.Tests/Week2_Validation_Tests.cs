namespace ReflectionLab.Tests;

public class Week2_Validation_Tests
{
    [Fact]
    private void Validation_ReturnsError_WhenValueOutOfRange()
    {
        var obj = new Validate
        {
            Age = 11,
            Siblings = 2,
            Name = "Bruce"
        };
        var errors = RangeIntValidation.ValidateRangeInt(obj);

        Assert.Single(errors);
        Assert.Contains(nameof(obj.Age), errors.First());
        Assert.Contains("1 and 10", errors.First());
    }

    [Fact]
    public void Validation_Ok_WhenValuesInRange()
    {
        var obj = new Validate
        {
            Age = 10,
            Siblings = 2,
            Name = "Bruce"
        };
        var errors = RangeIntValidation.ValidateRangeInt(obj);
        Assert.Empty(errors);
    }
}

internal class Validate
{
    [RangeInt(1, 10)] public int Age  { get; set; }
    [RangeInt(1, 10)] public int Siblings { get; set; }
    [RangeInt(1, 10)] public string Name { get; set; }
}