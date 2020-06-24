using System.Collections.Generic;

public class Pool<TKey, TValue>
{
    private Dictionary<TKey, Stack<TValue>> _pool = new Dictionary<TKey, Stack<TValue>>();

    public void Push(TKey key, TValue value)
    {
        Stack<TValue> stack;
        if (!_pool.TryGetValue(key, out stack))
        {
            stack = new Stack<TValue>();
            _pool.Add(key, stack);
        }
        stack.Push(value);
    }

    public TValue Pop(TKey key)
    {
        Stack<TValue> stack;
        if (_pool.TryGetValue(key, out stack))
        {
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
        }
        return default(TValue);
    }
}