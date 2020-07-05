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

    public class ChunkCube : IDisposable
    {
        static Stack<ChunkCube> pool = new Stack<ChunkCube>();

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

        public static ChunkCube Get(Chunk chunk, Vector3Int position, int metadata)
        {
            lock (pool)
            {
                ChunkCube cube = null;
                if (pool.Count() > 0)
                {
                    cube = pool.Pop();
                }
                else
                {
                    cube = new ChunkCube();
                }
                cube.chunk = chunk;
                cube.position = position;
                cube._metadata = metadata;
                return cube;
            }
        }

        public static int GetIndex(Vector3Int pos)
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

    public struct ChunkPosition
    {
        public int x;
        public int y;
        public int z;

        public ChunkPosition(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3Int ToVector3Int()
        {
            return new Vector3Int(x << 4, y << 4, z << 4);
        }

        public Vector3Int min
        {
            get {
                return ToVector3Int();
            }
        }

        public Vector3Int max
        {
            get {
                return ToVector3Int() + new Vector3Int(15, 15, 15);
            }
        }

        public Bounds bounds {
            get {
                return new Bounds((Vector3)(min + max) * 0.5f, Vector3.one * 16);
            }
        }

        bool Equals(ChunkPosition obj)
        {
            return x == obj.x && y == obj.y && z == obj.z;
        }

        public override bool Equals(object obj)
        {
            return Equals((ChunkPosition)obj);
        }

        public override int GetHashCode()
        {
            // from unity Vector3.GetHashCode()
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);

            //unchecked // Overflow is fine, just wrap
            //{
            //    int hash = 17;
            //    // Suitable nullity checks etc, of course :)
            //    hash = hash * 23 + x.GetHashCode();
            //    hash = hash * 23 + y.GetHashCode();
            //    hash = hash * 23 + z.GetHashCode();
            //    return hash;
            //}
        }

        public static bool operator ==(ChunkPosition a, ChunkPosition b)
        {
            return Equals(a, b);
        }

        public static bool operator != (ChunkPosition a, ChunkPosition b)
        {
            return !Equals(a, b);
        }

        public static ChunkPosition operator + (ChunkPosition a, Vector3Int offset)
        {
            return new ChunkPosition(a.x + offset.x, a.y + offset.y, a.z + offset.z);
        }

        public static ChunkPosition From(Vector3Int pos)
        {
            return new ChunkPosition(pos.x >> 4, pos.y >> 4, pos.z >> 4);
        }
    }

    public class ChunkChangeEvent
    {
        public Vector3Int position { get; private set; }
        public FacingSet sides { get; private set; }
        public int metadata { get; private set; }

        public ChunkChangeEvent(Vector3Int position, FacingSet sides, int metadata)
        {
            this.position = position;
            this.sides = sides;
            this.metadata = metadata;
        }

        public void Set(Vector3Int position, FacingSet sides, int metadata)
        {
            this.position = position;
            this.sides = sides;
            this.metadata = metadata;
        }
    }

    public class Chunk
    {
        public World world;
        public ChunkPosition position;
        int[] data;

        public event EventHandler<ChunkChangeEvent> cubeChanged;

        public Chunk(World world)
        {
            this.world = world;
            data = new int[16 * 16 * 16];
            Array.Clear(data, 0, data.Length);
        }

        public int this[Vector3Int pos] {
            get {
                return data[ChunkCube.GetIndex(pos)];
            }
            set {
                data[ChunkCube.GetIndex(pos)] = value;
            }
        }

        public int GetMetadata(Vector3Int pos)
        {
            return data[ChunkCube.GetIndex(pos)];
        }

        public void SetMetadata(Vector3Int pos, int value)
        {
            var index = ChunkCube.GetIndex(pos);
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
                        e.Set(pos.Offset(side), side.Opposite().GetSet(), -1);
                        cubeChanged(this, e);
                    }
                }
            }
        }

        public ChunkCube GetCube(Vector3Int pos)
        {
            return ChunkCube.Get(this, pos, data[ChunkCube.GetIndex(pos)]);
        }

        public ObjectType GetObjectType(Vector3Int pos)
        {
            return ChunkCube.GetObjectType(GetMetadata(pos));
        }

        public void SetObjectType(Vector3Int pos, ObjectType value)
        {
            var index = ChunkCube.GetIndex(pos);
            data[index] = ChunkCube.SetObjectType(data[index], value);
        }

        public static Vector3Int GetIndex(Vector3Int pos)
        {
            return new Vector3Int(pos.x & ~0xF, pos.y & ~0xF, pos.z & ~0xF);
        }
    }

    public class World 
    {
        public static int depth = 32;

        NoiseS3D noiseCore;
        Dictionary<ChunkPosition, Chunk> chunks;
        Chunk[] chunkCash;

        public World(int seed)
        {
            noiseCore = new NoiseS3D();
            noiseCore.seed = seed;
            chunks = new Dictionary<ChunkPosition, Chunk>();
            chunkCash = new Chunk[16 * 16 * 16];
        }

        public Chunk GetChunk(ChunkPosition pos)
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

        public int GetMetadata(Vector3Int pos)
        {
            var chunk = GetChunk(ChunkPosition.From(pos));
            return chunk.GetMetadata(pos);
        }

        public void SetMetadata(Vector3Int pos, int value)
        {
            var chunk = GetChunk(ChunkPosition.From(pos));
            chunk.SetMetadata(pos, value);
        }

        public ObjectType GetObjectType(Vector3Int pos)
        {
            return ChunkCube.GetObjectType(GetMetadata(pos));
        }

        public void SetObjectType(Vector3Int pos, ObjectType value)
        {
            var chunk = GetChunk(ChunkPosition.From(pos));
            chunk.SetObjectType(pos, value);
        }

        public bool IsEmpty(Vector3Int pos)
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
    }
}
