using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine;

namespace Game.Logic
{
    public class Chunk
    {
        public Chunks owner;
        public Vector2Int position;
        public float time;

        Dictionary<Vector2Int, ObjectType> data;

        public Chunk(Chunks owner)
        {
            data = new Dictionary<Vector2Int, ObjectType>(Game.chunkSize * Game.chunkSize);
            this.owner = owner;
        }

        public IEnumerable<Vector2Int> GetKeys()
        {
            return data.Keys;
        }

        public ObjectType this[Vector2Int position] {
            get {
                if (data.TryGetValue(position, out var obj))
                {
                    return obj;
                }
                return ObjectType.Empty;
            }
            set {
                data[position] = value;
            }
        }

        public void Generate(int seed)
        {
            NoiseS3D.seed = seed;
            for (int y = 0; y < Game.chunkSize; y++)
            {
                for (int x = 0; x < Game.chunkSize; x++)
                {
                    var p = new Vector2Int(position.x * Game.chunkSize + x, position.y * Game.chunkSize + y);
                    var z = NoiseS3D.Noise(p.x, p.y);
                    if (z > -0.3)
                    {
                        data[p] = ObjectType.Wall;
                    }
                }
            }

            NoiseS3D.seed = seed + 1;
            for (int y = 0; y < Game.chunkSize; y++)
            {
                for (int x = 0; x < Game.chunkSize; x++)
                {
                    var p = new Vector2Int(position.x * Game.chunkSize + x, position.y * Game.chunkSize + y);
                    var z = NoiseS3D.Noise(p.x, p.y);
                    if (z > 0)
                    {
                        data[p] = ObjectType.Stone;
                    }
                }
            }
        }

        public void Destroy()
        {
            //owner.Remove(this);
        }
    }

    public class Chunks : IEnumerable<Chunk>
    {
        public int seed = 0;

        Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

        public Chunks(int seed)
        {
            this.seed = seed;
        }

        public Chunk this[Vector2Int position] {
            get {
                if (chunks.TryGetValue(position, out var chunk))
                {
                    return chunk;
                }
                return null;
            }
        }

        public Chunk CreateChunk(Vector2Int position)
        {
            if (chunks.TryGetValue(position, out var chunk))
            {
                return chunk;
            }
            chunk = new Chunk(this);
            chunk.position = position;
            chunk.Generate(seed);
            chunks.Add(chunk.position, chunk);
            return chunk;
        }

        public void Add(Chunk chunk)
        {
            chunks.Add(chunk.position, chunk);
        }

        public void Remove(Chunk chunk)
        {
            chunks.Remove(chunk.position);
        }

        public IEnumerator<Chunk> GetEnumerator()
        {
            return chunks.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return chunks.Values.GetEnumerator();
        }

        public IEnumerable<Chunk> GetUpdateEnumerator()
        {
            var old = chunks;
            chunks = new Dictionary<Vector2Int, Chunk>();
            return old.Values;
        }
    }
}
