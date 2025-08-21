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

    #region API
    
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

    public void WarmUp(int count)
    {
        if (count <= 0)
        {
            return;
        }

        for (var i = 0; i < count && _items.Count < _maxSize; i++)
        {
            var item = _factory?.Invoke() ?? new T();
            _items.Add(item);
        }
    }

    public bool TryRent(out T? item)
    {
        if (_items.TryTake(out var gotItem))
        {
            item = gotItem;
            return true;
        }

        item = null;
        return false;
    }

    #endregion

   

    #region Lease

    public Lease RentLease() => new(this, Rent());
    public readonly struct Lease : IDisposable
    {
        private readonly ObjectPool<T> _pool;
        public readonly T Item;

        internal Lease(ObjectPool<T> pool, T item)
        {
            _pool = pool;
            Item = item;
        }

        public void Dispose() => _pool.Return(Item);
    }

    #endregion
}