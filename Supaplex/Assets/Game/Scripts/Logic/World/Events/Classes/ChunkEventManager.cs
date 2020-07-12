using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Logic.World
{
    public abstract class ChunkEventManager
    {
        #region StaticPart

        public static HashSet<ChunkEventManager> logicHashSet = new HashSet<ChunkEventManager>();
        public static HashSet<ChunkEventManager> viewHashSet = new HashSet<ChunkEventManager>();
        public static HashSet<ChunkEventManager> coroutineHashSet = new HashSet<ChunkEventManager>();

        //public static void CreateManager(object obj)
        //{
        //    var type = obj.GetType();
        //    var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic);
        //    foreach (var field in fields)
        //}

        #endregion

        public object monitor = new object();
        Stack<ChunkEventManager> poolManager = new Stack<ChunkEventManager>();
        Stack<ChunkEventBucket> poolBucket = new Stack<ChunkEventBucket>();
        ChunkEventBucket currentBucket;
        protected HashSet<ChunkEventManager> hashSet;
        protected Type managerType;
        short[] bits;
        bool hasBits = false;

        public ChunkEventManager(HashSet<ChunkEventManager> hashSet, Type managerType)
        {
            bits = new short[4096];
            this.hashSet = hashSet;
            this.managerType = managerType;
            currentBucket = new ChunkEventBucket(this);

            lock (hashSet)
            {
                hashSet.Add(this);
            }
        }

        public bool SetBit(BlockPos pos)
        {
            int index = ((pos.x & 0xF) << 4) | (pos.y & 0xF);
            lock (monitor)
            {
                if (((bits[index] >> (pos.z & 0xF)) & 1) == 0)
                {
                    bits[index] |= (short)(1 << (pos.z & 0xF));
                    hasBits = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool GetBit(BlockPos pos)
        {
            int index = ((pos.x & 0xF) << 4) | (pos.y & 0xF);
            return (bits[index] >> (pos.z & 0xF)) == 1;
        }

        public ChunkEventManager Create()
        {
            ChunkEventManager manager;
            lock (monitor)
            {
                if (poolManager.Count == 0)
                {

                    manager = Activator.CreateInstance(managerType) as ChunkEventManager;
                }
                else
                {
                    manager = poolManager.Pop();
                }
            }
            lock (hashSet)
            {
                hashSet.Add(manager);
            }
            return manager;
        }

        public void Publish(ChunkEvent e)
        {
            lock (monitor)
            {
                currentBucket.Enqueue(e);
            }
        }

        public bool GetBucket(out ChunkEventBucket bucket)
        {
            lock (monitor)
            {
                if (currentBucket.Count > 0)
                {
                    bucket = currentBucket;
                    if (poolBucket.Count > 0)
                    {
                        currentBucket = poolBucket.Pop();
                    }
                    else
                    {
                        currentBucket = new ChunkEventBucket(this);
                    }
                    if (hasBits)
                    {
                        hasBits = false;
                        Array.Clear(bits, 0, bits.Length);
                    }
                    return true;
                }
                else
                {
                    bucket = null;
                    return false;
                }
            }
        }

        public void Pool(ChunkEventBucket bucket)
        {
            lock (monitor)
            {
                poolBucket.Push(bucket);
            }
        }

        public void Pool()
        {
            lock (hashSet)
            {
                hashSet.Remove(this);
            }

            lock (poolManager)
            {
                poolManager.Push(this);
            }
        }
    }

    public sealed class ChunkLogicEventManager : ChunkEventManager
    {
        public ChunkLogicEventManager() : base(logicHashSet, typeof(ChunkLogicEventManager))
        {
        }
    }

    public sealed class ChunkViewEventManager : ChunkEventManager
    {
        public ChunkViewEventManager() : base(viewHashSet, typeof(ChunkViewEventManager))
        {
        }
    }

    public sealed class ChunkCoroutineEventManager : ChunkEventManager
    {
        public ChunkCoroutineEventManager() : base(coroutineHashSet, typeof(ChunkCoroutineEventManager))
        {
        }
    }
}
