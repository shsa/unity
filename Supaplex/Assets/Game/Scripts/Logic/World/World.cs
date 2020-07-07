using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Logic
{
    public class World
    {
        public static int depth = 64;

        NoiseS3D noiseCore;
        Dictionary<ChunkPos, Chunk> chunks;
        Chunk[] chunkCash;

        public World(int seed)
        {
            noiseCore = new NoiseS3D();
            noiseCore.seed = seed;
            chunks = new Dictionary<ChunkPos, Chunk>();
            chunkCash = new Chunk[16 * 16 * 16];
        }

        public Chunk GetChunk(ChunkPos pos)
        {
            var index = ((pos.x & 0xF) << 8) | ((pos.y & 0xF) << 4) | (pos.z & 0xF);
            var chunk = chunkCash[index];
            if (chunk == null || chunk.position != pos)
            {
                if (!chunks.TryGetValue(pos, out chunk))
                {
                    chunk = new Chunk(this, pos);
                    chunks[chunk.position] = chunk;
                    Generate(chunk);
                }
                chunkCash[index] = chunk;
            }
            return chunk;
        }

        /// <summary>
        /// return value x = {0, 1}
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        double GetNoise(double x, double y, double z)
        {
            return (noiseCore.Noise(x, y, z) + 1) * 0.5; // {0-1}
        }

        float stoneScale = 0.05f;
        bool CalcStone(int x, int y, int z, out float k)
        {
            k = (float)GetNoise(x * stoneScale, y * stoneScale, z * stoneScale);
            var f = 0.7f;
            var n = k - z * 2f / depth;
            if (n < f)
            {
                return true;
            }
            return false;
        }

        ObjectType CalcObjectType(BlockPos pos)
        {
            if (CalcStone(pos.x, pos.y, pos.z, out var k))
            {
                if (k > 0.8f)
                {
                    //return ObjectType.Stone4x4;
                }
                return ObjectType.Stone;
            }
            return ObjectType.Empty;
        }

        public bool IsStone(BlockPos pos)
        {
            return CalcObjectType(pos) == ObjectType.Stone;
        }

        void Generate(Chunk chunk)
        {
            var min = chunk.position.min;
            var max = chunk.position.max;
            var pos = new BlockPos();
            for (int z = min.z; z <= max.z; z++)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        pos.Set(x, y, z);
                        chunk.SetObjectType(pos, CalcObjectType(pos));
                    }
                }
            }
        }

        ChunkPos chunkPos = new ChunkPos(0, 0, 0);
        public int GetMetadata(BlockPos pos)
        {
            chunkPos.Set(pos);
            var chunk = GetChunk(chunkPos);
            return chunk.GetMetadata(pos);
        }

        public void SetMetadata(BlockPos pos, int value)
        {
            var chunk = GetChunk(ChunkPos.From(pos));
            chunk.SetMetadata(pos, value);
        }

        public ObjectType GetObjectType(BlockPos pos)
        {
            return ChunkBlock.GetObjectType(GetMetadata(pos));
        }

        public void SetObjectType(BlockPos pos, ObjectType value)
        {
            var chunk = GetChunk(ChunkPos.From(pos));
            chunk.SetObjectType(pos, value);
        }

        public bool IsEmpty(BlockPos pos)
        {
            UnityEngine.Profiling.Profiler.BeginSample("IsEmpty");
            try
            {
                switch (GetObjectType(pos))
                {
                    case ObjectType.Empty:
                    case ObjectType.None:
                        return true;
                    default:
                        return false;
                }
            }
            finally
            {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }
    }
}
