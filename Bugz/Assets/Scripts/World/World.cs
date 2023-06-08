using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.World
{
    public class World
    {
        Dictionary<ChunkPos, Chunk> chunks = new Dictionary<ChunkPos, Chunk>();
        public Chunk GetChunk(int x, int y)
        {
            var chunkPos = new ChunkPos(x >> 4, y >> 4);

            if (!chunks.TryGetValue(chunkPos, out var chunk))
            {
                chunk = new Chunk(this, chunkPos);
                chunks.Add(chunkPos, chunk);
            }
            return chunk;
        }

        public ChunkValue this[int x, int y]
        {
            get
            {
                var chunk = GetChunk(x, y);
                return chunk[(byte)(x & 0xF), (byte)(y & 0xF)];
            }
            set
            {
                var chunk = GetChunk(x, y);
                chunk[(byte)(x & 0xF), (byte)(y & 0xF)] = value;
            }
        }

        public void UpdateItem(int x, int y)
        {
            var chunk = GetChunk(x, y);
            chunk.UpdateItem((byte)(x & 0xF), (byte)(y & 0xF));
        }
    }
}
