using System.Text;
using GenericsLab;

namespace Playground.Tests;

// Tipo de prueba para el pool
file sealed class PooledBuilder : IResettable
{
    public StringBuilder SB { get; } = new();
    public void Reset() => SB.Clear();
}

public class ObjectPool_StringBuilder_Tests
{
    [Fact]
    public void Return_ShrinksHugeBuffers_AfterClear()
    {
        var pool = new ObjectPool<PooledBuilder>(
            maxSize: 32,
            factory: () => { var p = new PooledBuilder(); p.SB.EnsureCapacity(256); return p; },
            onReturn: b => { b.Reset(); if (b.SB.Capacity > 1024) b.SB.Capacity = 256; }
        );

        var a = pool.Rent();
        a.SB.Append('x', 5000);               // forzamos que crezca
        Assert.True(a.SB.Capacity > 1024);     // sanity check
        pool.Return(a);

        var b = pool.Rent();
        Assert.Equal(0, b.SB.Length);          // quedó limpio
        Assert.True(b.SB.Capacity <= 256);     // se “capó” el buffer
    }

    [Fact]
    public void Return_ClearsContent_WhenNotHuge()
    {
        var pool = new ObjectPool<PooledBuilder>(32, factory: () => new PooledBuilder());
        var a = pool.Rent();
        a.SB.Append("abc");
        pool.Return(a);

        var b = pool.Rent();
        Assert.Equal(0, b.SB.Length);          // al menos se limpió
        pool.Return(b);
    }
}