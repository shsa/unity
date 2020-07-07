using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Logic;
using Game.View;

namespace Game.Logic.World
{
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
        public WorldProvider world { get; private set; }
        public ChunkPos position { get; private set; }
        int[] data;

        public event EventHandler<ChunkChangeEvent> cubeChanged;

        public Chunk(WorldProvider world, ChunkPos pos)
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
            return data[GetBlockIndex(pos)];
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

        public static ObjectType GetObjectType(int metadata)
        {
            return (ObjectType)((metadata >> 8) & 0xFF);
        }

        public static int SetObjectType(int metadata, ObjectType value)
        {
            return (metadata & ~(0xFF << 8)) | ((int)value << 8);
        }

        public ObjectType GetObjectType(BlockPos pos)
        {
            return GetObjectType(GetMetadata(pos));
        }

        public void SetObjectType(BlockPos pos, ObjectType value)
        {
            var index = GetBlockIndex(pos);
            data[index] = SetObjectType(data[index], value);
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
