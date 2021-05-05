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
    sealed class GenerateChunkSystem : IEcsRunSystem, IEcsDestroySystem
    {
        readonly EcsFilter<Ecs.Chunk> _filter = default;
        readonly GameWorld _gameWorld = default;

        public GenerateChunkSystem()
        {
        }

        public void Destroy()
        {
        }

        Dictionary<Chunk, ChunkGenerateContainer> jobs = new Dictionary<Chunk, ChunkGenerateContainer>();

        void SetBlockUpdate(int x, int y, int z)
        {
            if (_gameWorld.TryGetChunk(x, y, z, out var c))
            {
                c.SetFlag(x, y, z, BlockFlag.Update);
            }
        }

        void IEcsRunSystem.Run()
        {
            foreach (var gc in jobs.Values.ToArray())
            {
                if (gc.handle.IsCompleted)
                {
                    gc.handle.Complete();
                    var chunk = gc.chunk;
                    gc.data.CopyTo(chunk.data);
                    for (int x = chunk.x - 1; x <= (chunk.x + 0x10); x++)
                    {
                        for (int z = (int)WorldLayerEnum.First; z <= (int)WorldLayerEnum.Last; z++)
                        {
                            SetBlockUpdate(x, chunk.y - 1, z);
                            SetBlockUpdate(x, chunk.y + 0x10, z);
                        }
                    }
                    for (int y = chunk.y - 1; y <= (chunk.y + 0x10); y++)
                    {
                        for (int z = (int)WorldLayerEnum.First; z <= (int)WorldLayerEnum.Last; z++)
                        {
                            SetBlockUpdate(chunk.x - 1, y, z);
                            SetBlockUpdate(chunk.x + 0x10, y, z);
                        }
                    }
                    for (int i = 0; i < gc.chunk.data_update.Length; i++)
                    {
                        chunk.data_update[i] = BlockFlag.Update;
                    }
                    jobs.Remove(gc.chunk);
                    chunk.flag |= ChunkFlag.Loaded | ChunkFlag.Update;
                    gc.Dispose();
                    chunk.count = chunk.data.Where(d => d.id == BlockType.Stone).Count();
                }
            }

            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                ref var chunk = ref _filter.Get1(i).Value;
                if ((chunk.flag & ChunkFlag.Generate) == ChunkFlag.Generate)
                {
                    var container = new ChunkGenerateContainer
                    {
                        chunk = chunk,
                        data = new NativeArray<BlockData>(chunk.data, Allocator.TempJob)
                    };

                    var job = new ChunkGenerateJob
                    {
                        x = chunk.x,
                        y = chunk.y,
                        z = chunk.z,
                        data = container.data
                    };

                    container.handle = job.Schedule();
                    jobs.Add(chunk, container);
                    chunk.flag &= ~ChunkFlag.Generate;
                }
            }
        }
    }
}