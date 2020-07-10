using System.Collections.Generic;

namespace Game.Logic.World
{
    public abstract class EventProvider
    {
        protected Stack<EventChunk> pool = new Stack<EventChunk>();
        protected Queue<EventChunk> queue = new Queue<EventChunk>();

        public void Enqueue(EventChunk chunk)
        {
            lock (queue)
            {
                queue.Enqueue(chunk);
            }
        }

        public void Pool(EventChunk chunk)
        {
            lock (pool)
            {
                pool.Push(chunk);
            }
        }

        public EventChunk<T> Create<T>() where T : Event
        {
            lock (pool)
            {
                if (pool.Count == 0)
                {
                    return new EventChunk<T>(this);
                }
                else
                {
                    return pool.Pop() as EventChunk<T>;
                }
            }
        }
    }

    public sealed class EventProvider<T> : EventProvider where T : Event
    {
        public EventChunk<T> Create()
        {
            lock (pool)
            {
                if (pool.Count == 0)
                {
                    return new EventChunk<T>(this);
                }
                else
                {
                    return pool.Pop() as EventChunk<T>;
                }
            }
        }

        public EventChunk<T> Dequeue()
        {
            if (queue.Count > 0)
            {
                return queue.Dequeue() as EventChunk<T>;
            }
            return null;
        }
    }
}
