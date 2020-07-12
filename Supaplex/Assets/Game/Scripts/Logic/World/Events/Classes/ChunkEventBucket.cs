using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logic.World
{
    public sealed class ChunkEventBucket : Queue<ChunkEvent>
    {
        ChunkEventManager owner;
        public ChunkEventBucket(ChunkEventManager owner)
        {
            this.owner = owner;
        }

        public void Pool()
        {
            owner.Pool(this);
        }
    }
}
