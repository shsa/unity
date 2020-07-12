using System.Collections.Generic;
using System.Threading;

namespace Game.Logic.World
{
    public sealed class ChunkEventThread
    {
        object monitor = new object();

        Queue<ChunkEventBucket> queue = new Queue<ChunkEventBucket>();
        Thread thread;

        public ChunkEventThread()
        {
            thread = new Thread(Execute);
            thread.Start();
        }

        public int Count()
        {
            lock (monitor)
            {
                return queue.Count;
            }
        }

        public void Enqueue(ChunkEventBucket bucket)
        {
            lock (monitor)
            {
                queue.Enqueue(bucket);
            }
        }

        void Execute()
        {
            ChunkEventBucket bucket;
            while (true)
            {
                lock (monitor)
                {
                    if (queue.Count > 0)
                    {
                        bucket = queue.Dequeue();
                    }
                    else
                    {
                        bucket = null;
                    }
                }

                if (bucket == null)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    Execute(bucket);
                }
            }
        }

        void Execute(ChunkEventBucket bucket)
        {
            while (bucket.Count > 0)
            {
                var e = bucket.Dequeue();
                e.Execute();
                e.Pool();
            }

            bucket.Pool();
        }
    }
}
