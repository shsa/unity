using System.Collections.Generic;

public class Pool<TValue>
{
    Stack<TValue> _pool = new Stack<TValue>();

    public void Push(TValue value)
    {
        _pool.Push(value);
    }

    public TValue Pop()
    {
        if (_pool.Count > 0)
        {
            return _pool.Pop();
        }
        return default(TValue);
    }

    public bool TryPop(out TValue value)
    {
        if (_pool.Count > 0)
        {
            value = _pool.Pop();
            return true;
        }
        value = default(TValue);
        return false;
    }
}

public class Pool<TKey, TValue>
{
    private Dictionary<TKey, Pool<TValue>> _pool = new Dictionary<TKey, Pool<TValue>>();

    public void Push(in TKey key, TValue value)
    {
        Pool<TValue> stack;
        if (!_pool.TryGetValue(key, out stack))
        {
            stack = new Pool<TValue>();
            _pool.Add(key, stack);
        }
        stack.Push(value);
    }

    public TValue Pop(in TKey key)
    {
        Pool<TValue> stack;
        if (_pool.TryGetValue(key, out stack))
        {
            return stack.Pop();
        }
        return default(TValue);
    }

    public bool TryPop(in TKey key, out TValue value)
    {
        Pool<TValue> stack;
        if (_pool.TryGetValue(key, out stack))
        {
            return stack.TryPop(out value);
        }
        value = default(TValue);
        return false;
    }
}