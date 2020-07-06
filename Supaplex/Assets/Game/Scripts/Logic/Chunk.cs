using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Logic;
using Game.View;

namespace Game.Logic
{
    public enum ChunkCubeState
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
        public BlockPos position;
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

        public bool GetState(ChunkCubeState state)
        {
            return (metadata & (int)state) == (int)state;
        }

        public void SetState(ChunkCubeState state, bool value)
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

        public bool IsVisible(Facing side)
        {
            if (!GetState(ChunkCubeState.Updated))
            {
                BeginUpdate();
                SetState(ChunkCubeState.Forward, chunk.world.IsEmpty(position + new Vector3Int(0, 0, -1)));
                SetState(ChunkCubeState.Back, chunk.world.IsEmpty(position + new Vector3Int(0, 0, 1)));
                SetState(ChunkCubeState.Left, chunk.world.IsEmpty(position + Vector3Int.left));
                SetState(ChunkCubeState.Right, chunk.world.IsEmpty(position + Vector3Int.right));
                SetState(ChunkCubeState.Up, chunk.world.IsEmpty(position + Vector3Int.up));
                SetState(ChunkCubeState.Down, chunk.world.IsEmpty(position + Vector3Int.down));
                SetState(ChunkCubeState.Updated, true);
                EndUpdate();
            }
            switch (side)
            {
                case Facing.South: return GetState(ChunkCubeState.Forward);
                case Facing.North: return GetState(ChunkCubeState.Back);
                case Facing.West: return GetState(ChunkCubeState.Left);
                case Facing.East: return GetState(ChunkCubeState.Right);
                case Facing.Up: return GetState(ChunkCubeState.Up);
                case Facing.Down: return GetState(ChunkCubeState.Down);
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
                chunk.SetMetadata(this.position, this.metadata);
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

        public static ChunkBlock Get(Chunk chunk, BlockPos position, int metadata)
        {
            lock (pool)
            {
                ChunkBlock cube = null;
                if (pool.Count() > 0)
                {
                    cube = pool.Pop();
                }
                else
                {
                    cube = new ChunkBlock();
                }
                cube.chunk = chunk;
                cube.position = position;
                cube._metadata = metadata;
                return cube;
            }
        }

        public static int GetIndex(BlockPos pos)
        {
            return ((pos.y & 0xF) << 8) | ((pos.x & 0xF) << 4) | (pos.z & 0xF);
        }

        public static ObjectType GetObjectType(int metadata)
        {
            return (ObjectType)((metadata >> 8) & 0xFF);
        }

        public static int SetObjectType(int metadata, ObjectType value)
        {
            return (metadata & ~(0xFF << 8)) | ((int)value << 8);
        }
    }

    public class ChunkChangeEvent
    {
        public BlockPos position { get; private set; }
        public FacingSet sides { get; private set; }
        public int metadata { get; private set; }

        public ChunkChangeEvent(BlockPos position, FacingSet sets, int metadata)
        {
            this.position = position;
            this.sides = sets;
            this.metadata = metadata;
        }

        public void Set(BlockPos position, FacingSet sides, int metadata)
        {
            this.position = position;
            this.sides = sides;
            this.metadata = metadata;
        }
    }

    public class Chunk
    {
        public World world;
        public ChunkPos position;
        int[] data;

        public event EventHandler<ChunkChangeEvent> cubeChanged;

        public Chunk(World world)
        {
            this.world = world;
            data = new int[16 * 16 * 16];
            Array.Clear(data, 0, data.Length);
        }

        public int this[BlockPos pos] {
            get {
                return data[ChunkBlock.GetIndex(pos)];
            }
            set {
                data[ChunkBlock.GetIndex(pos)] = value;
            }
        }

        public int GetMetadata(BlockPos pos)
        {
            return data[ChunkBlock.GetIndex(pos)];
        }

        public void SetMetadata(BlockPos pos, int value)
        {
            var index = GetBlockIndex(pos);
            var metadata = data[index];
            if (metadata != value)
            {
                data[index] = value;
                if (cubeChanged != null)
                {
                    var e = new ChunkChangeEvent(pos, FacingSet.All, metadata);
                    cubeChanged(this, e);
                    for (var side = Facing.First; side <= Facing.Last; side++)
                    {
                        e.Set(pos.Offset(side), side.Opposite().ToSet(), -1);
                        cubeChanged(this, e);
                    }
                }
            }
        }

        public ChunkBlock GetBlock(BlockPos pos)
        {
            return ChunkBlock.Get(this, pos, data[GetBlockIndex(pos)]);
        }

        public ObjectType GetObjectType(BlockPos pos)
        {
            return ChunkBlock.GetObjectType(GetMetadata(pos));
        }

        public void SetObjectType(BlockPos pos, ObjectType value)
        {
            var index = ChunkBlock.GetIndex(pos);
            data[index] = ChunkBlock.SetObjectType(data[index], value);
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

        public static int GetBlockIndex(BlockPos pos)
        {
            return ((pos.y & 0xF) << 8) | ((pos.x & 0xF) << 4) | (pos.z & 0xF);
        }
    }

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
                    chunk = new Chunk(this);
                    chunk.position = pos;
                    chunks[pos] = chunk;
                    Generate(chunk);
                }
                chunkCash[index] = chunk;
            }
            return chunk;
        }

        float wallScale = 0.05f;
        public bool IsWall(int x, int y, int z)
        {
            //return (x + y) % 2 == 0;
            var n = noiseCore.Noise(x * wallScale, y * wallScale, z * wallScale) - z * 2f / World.depth;
            return n < 0.3;
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
                        if (IsWall(x, y, z))
                        {
                            chunk.SetObjectType(pos, ObjectType.Wall);
                        }
                        else
                        {
                            chunk.SetObjectType(pos, ObjectType.Empty);
                        }
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
