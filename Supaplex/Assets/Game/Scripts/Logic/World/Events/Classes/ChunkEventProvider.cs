using System;
using System.Collections.Generic;

namespace Game.Logic.World
{
    public class ChunkEventProviderBase
    {
        public ChunkEventManager manager;
        protected Stack<ChunkEvent> pool = new Stack<ChunkEvent>();

        public ChunkEventProviderBase(ChunkEventManager manager)
        {
            this.manager = manager;
        }

        public void Pool(ChunkEvent e)
        {
            lock (pool)
            {
                pool.Push(e);
            }
        }
    }

    public sealed class ChunkEventProvider<T> : ChunkEventProviderBase where T : ChunkEvent
    {
        object[] _params;
        public ChunkEventProvider(ChunkEventManager manager) : base(manager)
        {
            _params = new object[] { this };
        }

        public T Create()
        {
            lock (pool)
            {
                if (pool.Count > 0)
                {
                    return pool.Pop() as T;
                }
                else
                {
                    var obj = Activator.CreateInstance<T>();
                    obj.SetProvider(this);
                    return obj;
                }
            }
        }
    }
}
