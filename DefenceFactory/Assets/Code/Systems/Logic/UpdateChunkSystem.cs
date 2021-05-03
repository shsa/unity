using DefenceFactory.Game;
using DefenceFactory.Game.Jobs;
using DefenceFactory.Game.World;
using Leopotam.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace DefenceFactory
{

    using BlockDataArray = NativeArray<BlockData>;

    sealed class UpdateChunkSystem : IEcsRunSystem, IEcsDestroySystem
    {
        readonly EcsFilter<Ecs.Chunk> _filter = default;
        BlockDataArray N;
        BlockDataArray NE;
        BlockDataArray E;
        BlockDataArray SE;
        BlockDataArray S;
        BlockDataArray SW;
        BlockDataArray W;
        BlockDataArray NW;

        public UpdateChunkSystem()
        {
            N = new BlockDataArray(4096, Allocator.Persistent);
            NE = new BlockDataArray(4096, Allocator.Persistent);
            E = new BlockDataArray(4096, Allocator.Persistent);
            SE = new BlockDataArray(4096, Allocator.Persistent);
            S = new BlockDataArray(4096, Allocator.Persistent);
            SW = new BlockDataArray(4096, Allocator.Persistent);
            W = new BlockDataArray(4096, Allocator.Persistent);
            NW = new BlockDataArray(4096, Allocator.Persistent);
        }

        public void Destroy()
        {
            N.Dispose();
            NE.Dispose();
            E.Dispose();
            SE.Dispose();
            S.Dispose();
            SW.Dispose();
            W.Dispose();
            NW.Dispose();
        }

        BlockDataArray Neighbour(Container container, DirectionEnum dir)
        {
            ref var v = ref dir.GetVector2();
            var chunk = container.chunk;
            if (chunk.World.TryGetChunk(chunk.x + v.X * 0x10, chunk.y + v.Y * 0x10, chunk.z, out var c))
            {
                var data = new BlockDataArray(c.data, Allocator.TempJob);
                container.neighbours.Add(data);
                return data;
            }
            switch (dir)
            {
                case DirectionEnum.N: return N;
                case DirectionEnum.NE: return NE;
                case DirectionEnum.E: return E;
                case DirectionEnum.SE: return SE;
                case DirectionEnum.S: return S;
                case DirectionEnum.SW: return SW;
                case DirectionEnum.W: return W;
                case DirectionEnum.NW: return NW;
                default: throw new NotImplementedException();
            }
        }

        class Container : IDisposable
        {
            public Chunk chunk;
            public JobHandle jobHandle;

            public BlockDataArray data;
            public NativeArray<BlockFlag> data_update;
            public List<BlockDataArray> neighbours = new List<BlockDataArray>();

            public override int GetHashCode()
            {
                return chunk.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is Container c)
                {
                    return chunk.Equals(c.chunk);
                }
                return false;
            }

            private bool disposedValue;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    data.Dispose();
                    data_update.Dispose();
                    foreach (var a in neighbours)
                    {
                        a.Dispose();
                    }
                    disposedValue = true;
                }
            }

            // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            ~Container()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: false);
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        HashSet<Container> jobs = new HashSet<Container>();

        void IEcsRunSystem.Run()
        {
            foreach (var c in jobs.ToArray())
            {
                if (c.jobHandle.IsCompleted)
                {
                    c.jobHandle.Complete();
                    c.data.CopyTo(c.chunk.data);
                    jobs.Remove(c);
                    c.chunk.flag
                        .Remove(ChunkFlag.Updating)
                        .Add(ChunkFlag.Redraw);
                    c.Dispose();
                }
            }

            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                ref var chunk = ref _filter.Get1(i).Value;
                if (chunk.count == 0)
                {
                    continue;
                }
                if (chunk.flag.HasFlag(ChunkFlag.Updating))
                {
                    continue;
                }
                if (chunk.flag.HasFlag(ChunkFlag.Update))
                {
                    var container = new Container
                    {
                        chunk = chunk,
                        data = new BlockDataArray(chunk.data, Allocator.TempJob),
                        data_update = new NativeArray<BlockFlag>(chunk.data_update, Allocator.TempJob)
                    };

                    var job = new ChunkUpdateJob
                    {
                        count = chunk.count,
                        x = chunk.x,
                        y = chunk.y,
                        z = chunk.z,
                        data = container.data,
                        data_update = container.data_update,
                        N = Neighbour(container, DirectionEnum.N),
                        NE = Neighbour(container, DirectionEnum.NE),
                        E = Neighbour(container, DirectionEnum.E),
                        SE = Neighbour(container, DirectionEnum.SE),
                        S = Neighbour(container, DirectionEnum.S),
                        SW = Neighbour(container, DirectionEnum.SW),
                        W = Neighbour(container, DirectionEnum.W),
                        NW = Neighbour(container, DirectionEnum.NW),
                    };

                    container.jobHandle = job.Schedule();
                    jobs.Add(container);

                    for (int j = 0; j < chunk.data_update.Length; j++)
                    {
                        chunk.data_update[j] &= ~BlockFlag.Update;
                    }
                    chunk.flag
                        .Add(ChunkFlag.Updating)
                        .Remove(ChunkFlag.Update);
                }
            }
        }
    }
}