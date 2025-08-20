using GenericsLab;
using System.Text;

Console.WriteLine("Demo ObjectPool<T>");

var pool = new ObjectPool<PooledStringBuilder>(maxSize: 32, factory: InitializePooledStringBuilder, onReturn: OnPooledBuilderReturnCallback);

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