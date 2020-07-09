using System;
using System.Collections.Generic;

namespace Game.Logic.World
{
    public abstract class EventChunk
    {
        EventProvider provider;
        protected Stack<Event> pool = new Stack<Event>();
        protected Queue<Event> queue = new Queue<Event>();

        public EventChunk(EventProvider provider)
        {
            this.provider = provider;
        }

        public bool Raise()
        {
            if (queue.Count > 0)
            {
                var e = queue.Dequeue();
                e.Raise();
                pool.Push(e);
                return true;
            }
            return false;
        }

        public void Pool()
        {
            provider.Pool(this);
        }
    }

    public sealed class EventChunk<T> : EventChunk where T : Event
    {
        public EventChunk(EventProvider provider) : base(provider)
        {
        }

        public T Create()
        {
            T e;
            if (pool.Count == 0)
            {
                e = Activator.CreateInstance<T>();
            }
            else
            {
                e = pool.Pop() as T;
            }
            queue.Enqueue(e);
            return e;
        }
    }
}
