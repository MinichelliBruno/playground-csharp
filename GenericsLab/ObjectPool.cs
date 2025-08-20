using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace GenericsLab;

/// <summary>
/// Generic pol with simple limit, thread safe.
/// </summary>
/// <typeparam name="T">Pooling object to be instantiated</typeparam>
public sealed class ObjectPool<T> : IPool<T>
where T : class, IResettable, new()
{
    private readonly ConcurrentBag<T> _items = [];
    private readonly int _maxSize;
    private readonly Func<T>? _factory;
    private readonly Action<T>? _onReturn;

    public ObjectPool(int maxSize = 100, Func<T>? factory = null, Action<T>? onReturn = null)
    {
        if (maxSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSize));
        }

        _maxSize = maxSize;
        
        _factory = factory;
        // is of type IResettable
        _onReturn = onReturn ?? (item => item.Reset());
    }

    public int Count => _items.Count;

    public T Rent() => _items.TryTake(out var item) ? item : _factory?.Invoke() ?? new T();

    public void Return(T item)
    {
        _onReturn?.Invoke(item);
        // If full, discard it silently.
        if (_items.Count < _maxSize)
        {
            _items.Add(item);
        }
    }
}