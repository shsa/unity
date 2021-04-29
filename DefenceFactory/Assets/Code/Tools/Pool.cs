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
}

public class Pool<TKey, TValue>
{
    private Dictionary<TKey, Pool<TValue>> _pool = new Dictionary<TKey, Pool<TValue>>();

    public void Push(TKey key, TValue value)
    {
        Pool<TValue> stack;
        if (!_pool.TryGetValue(key, out stack))
        {
            stack = new Pool<TValue>();
            _pool.Add(key, stack);
        }
        stack.Push(value);
    }

    public TValue Pop(TKey key)
    {
        Pool<TValue> stack;
        if (_pool.TryGetValue(key, out stack))
        {
            return stack.Pop();
        }
        return default(TValue);
    }
}