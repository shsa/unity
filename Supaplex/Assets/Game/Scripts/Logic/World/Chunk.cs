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
        public World world { get; private set; }
        public ChunkPos position { get; private set; }
        int[] data;

        public event EventHandler<ChunkChangeEvent> cubeChanged;

        public Chunk(World world, ChunkPos pos)
        {
            this.world = world;
            this.position = new ChunkPos(pos);
            data = new int[16 * 16 * 16];
            Array.Clear(data, 0, data.Length);
        }

        public int this[BlockPos pos] {
            get {
                return data[GetBlockIndex(pos)];
            }
            set {
                data[GetBlockIndex(pos)] = value;
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
}
