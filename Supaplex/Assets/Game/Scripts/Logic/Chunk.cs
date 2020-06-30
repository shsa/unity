using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Logic;
using Game.View;

namespace Game.Logic
{
    public enum ChunkBlockState
    {
        Forward = 0x01,
        Back = 0x02,
        Left = 0x04,
        Right = 0x08,
        Up = 0x10,
        Down = 0x20,
        Updated = 0x40,
    }

    public class ChunkBlock : IDisposable
    {
        static Stack<ChunkBlock> pool = new Stack<ChunkBlock>();

        bool _keepChanges = false;
        bool _modified = false;

        public Chunk chunk;
        public Vector3Int position;
        int _metadata;
        public int metadata {
            get {
                return _metadata;
            }
            set {
                if (_metadata != value)
                {
                    _metadata = value;
                    OnChanged();
                }
            }
        }

        public ObjectType objectType {
            get {
                return (ObjectType)((metadata >> 8) & 0xFF);
            }
            set {
                metadata = ((int)value << 8) | (metadata & 0xFF);
            }
        }

        public bool GetState(ChunkBlockState state)
        {
            return (metadata & (int)state) == (int)state;
        }

        public void SetState(ChunkBlockState state, bool value)
        {
            if (value)
            {
                metadata |= (int)state;
            }
            else
            {
                metadata &= ~(int)state;
            }
        }

        public bool IsVisible(CubeSide side)
        {
            if (!GetState(ChunkBlockState.Updated))
            {
                BeginUpdate();
                SetState(ChunkBlockState.Forward, chunk.world.IsEmpty(position + new Vector3Int(0, 0, -1)));
                SetState(ChunkBlockState.Back, chunk.world.IsEmpty(position + new Vector3Int(0, 0, 1)));
                SetState(ChunkBlockState.Left, chunk.world.IsEmpty(position + Vector3Int.left));
                SetState(ChunkBlockState.Right, chunk.world.IsEmpty(position + Vector3Int.right));
                SetState(ChunkBlockState.Up, chunk.world.IsEmpty(position + Vector3Int.up));
                SetState(ChunkBlockState.Down, chunk.world.IsEmpty(position + Vector3Int.down));
                SetState(ChunkBlockState.Updated, true);
                EndUpdate();
            }
            switch (side)
            {
                case CubeSide.Forward: return GetState(ChunkBlockState.Forward);
                case CubeSide.Back: return GetState(ChunkBlockState.Back);
                case CubeSide.Left: return GetState(ChunkBlockState.Left);
                case CubeSide.Right: return GetState(ChunkBlockState.Right);
                case CubeSide.Up: return GetState(ChunkBlockState.Up);
                case CubeSide.Down: return GetState(ChunkBlockState.Down);
                default: return false;
            }
        }

        public void OnChanged()
        {
            if (_keepChanges)
            {
                _modified = true;
            }
            else
            {
                chunk[position] = this;
            }
        }

        public void BeginUpdate()
        {
            _keepChanges = true;
        }

        public void EndUpdate()
        {
            _keepChanges = false;
            if (_modified)
            {
                OnChanged();
                _modified = false;
            }
        }

        public void Dispose()
        {
            pool.Push(this);
        }

        public static ChunkBlock Get(Chunk chunk, Vector3Int position, int metadata)
        {
            lock (pool)
            {
                ChunkBlock block = null;
                if (pool.Count() > 0)
                {
                    block = pool.Pop();
                }
                else
                {
                    block = new ChunkBlock();
                }
                block.chunk = chunk;
                block.position = position;
                block._metadata = metadata;
                return block;
            }
        }
    }

    public class Chunk
    {
        public World world;
        public Vector2Int position;
        int[] data;

        public Chunk(World world)
        {
            this.world = world;
            data = new int[16 * 16 * 256];
            Array.Clear(data, 0, data.Length);
        }

        public ChunkBlock this[Vector3Int pos] {
            get {
                return ChunkBlock.Get(this, pos, data[BlockIndex(pos)]);
            }
            set {
                data[BlockIndex(pos)] = value.metadata;
            }
        }

        public static int BlockIndex(Vector3Int pos)
        {
            return ((int)(pos.y & 0xF) << 12) | ((int)(pos.x & 0xF) << 8) | (pos.z & 0xFF);
        }
    }

    public class World 
    {
        public static int depth = 10;

        NoiseS3D noiseCore;
        Dictionary<Vector2Int, Chunk> chunks;

        public World(int seed)
        {
            noiseCore = new NoiseS3D();
            noiseCore.seed = seed;
            chunks = new Dictionary<Vector2Int, Chunk>();
        }

        public Chunk GetChunk(Vector2Int pos)
        {
            if (!chunks.TryGetValue(pos, out var chunk))
            {
                chunk = new Chunk(this);
                chunk.position = pos;
                chunks[pos] = chunk;
                Generate(chunk);
            }
            return chunk;
        }

        float wallScale = 0.05f;
        public bool IsWall(int x, int y, int z)
        {
            //return (x + y) % 2 == 0;
            var n = noiseCore.Noise(x * wallScale, y * wallScale, z * wallScale);
            return n < 0.3;
        }

        void Generate(Chunk chunk)
        {
            var minX = 0;
            var maxX = 0;
            var minY = 0;
            var maxY = 0;
            minX = chunk.position.x << 4;
            maxX = (chunk.position.x + 1) << 4;
            minY = chunk.position.y << 4;
            maxY = (chunk.position.y + 1) << 4;

            for (int z = 0; z < depth; z++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        var pos = new Vector3Int(x, y, z);
                        using (var obj = chunk[pos])
                        {
                            if (IsWall(x, y, z))
                            {
                                obj.objectType = ObjectType.Wall;
                            }
                            else
                            {
                                obj.objectType = ObjectType.Empty;
                            }
                        }
                    }
                }
            }
        }

        public ChunkBlock this[Vector3Int pos]
        {
            get {
                UnityEngine.Profiling.Profiler.BeginSample("get chunk");
                var chunk = GetChunk(new Vector2Int(pos.x >> 4, pos.y >> 4));
                UnityEngine.Profiling.Profiler.EndSample();
                UnityEngine.Profiling.Profiler.BeginSample("get pos");
                var result = chunk[pos];
                UnityEngine.Profiling.Profiler.EndSample();

                return result;
            }
            set {
                var chunk = GetChunk(new Vector2Int(pos.x >> 4, pos.y >> 4));
                chunk[pos] = value;
            }
        }

        public bool IsEmpty(Vector3Int pos)
        {
            using (var obj = this[pos])
            {
                switch (obj.objectType)
                {
                    case ObjectType.Empty:
                    case ObjectType.None:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}
