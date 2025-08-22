using System.Globalization;using GenericsLab;
using System.Text;
using ReflectionLab;

#region Object Pool

Console.WriteLine("Demo ObjectPool<T>");

var pool = new ObjectPool<PooledStringBuilder>(maxSize: 32, factory: InitializePooledStringBuilder, onReturn: OnPooledBuilderReturnCallback);
pool.WarmUp(4);

if (pool.TryRent(out var test))
{
    test!.Sb.Append("No alloc rent");
    Console.WriteLine($"{test.Sb}");
    pool.Return(test);
     
}  
else
{
    Console.WriteLine("Unable to rent, no allocated items.");
}

// covariant
IPool<IResettable> poolAsInterface = pool;
var rentViaInterface = (PooledStringBuilder)poolAsInterface.Rent();
pool.Return(rentViaInterface);

// Rent
var a = pool.Rent();
Console.WriteLine($"Pooled builder capacity: {a.Sb.Capacity}");
a.Sb.Append('x', 5000);
Console.WriteLine($"Pooled builder capacity: {a.Sb.Capacity}");
pool.Return(a);

// Rent once more, check it was reset.
var b = pool.Rent();
Console.WriteLine($"Pooled builder capacity: {b.Sb.Capacity}");
pool.Return(b);


using var lease = pool.RentLease();
lease.Item.Sb.Append("Duh");
Console.WriteLine($"{lease.Item.Sb}");
#endregion Object Pool

#region Reflection, attributes

Console.WriteLine("---- SavableSerializer demo ----");

var serializableData = new SerializableData
{
    Name = "Bruno",
    Surname = "Minichelli",
    Age = 32
};

var dict = SavableSerializer.Serialize(serializableData);

foreach (var keyValuePair in dict)
{
    Console.WriteLine($"{keyValuePair.Key} + {keyValuePair.Value}");
}

#endregion

return;


PooledStringBuilder InitializePooledStringBuilder()
{
    var  sb = new PooledStringBuilder();
    sb.Sb.EnsureCapacity(256);
    return sb;
}

void OnPooledBuilderReturnCallback(PooledStringBuilder builder)
{
    builder.Reset();
    if (builder.Sb.Capacity > 256)
    {
        builder.Sb.Capacity = 256;
    }
}
internal class PooledStringBuilder : IResettable
{
    public StringBuilder Sb { get; } = new();
    public void Reset() => Sb.Clear();
}

internal class SerializableData
{
    [Savable] public string Name { get; set; } = "a";
    [Savable] public string Surname { get; set; } = "b";
    [Savable("Vueltas al sol")] public int Age { get; set; } = 32;
}