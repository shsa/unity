using Game.Logic.World;

namespace Game.View.World
{
    public sealed class RenderCalcVisibilityEvent : ChunkEvent
    {
        public RenderChunk render;
        public IChunkReader chunk;
        BlockPos pos = new BlockPos();

        public override void Execute()
        {
            var version = chunk.version;
            var min = chunk.position.min;
            var max = chunk.position.max;

            for (int z = min.z; z <= max.z; z++)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        pos.Set(x, y, z);
                        if (chunk.GetBlockData(pos).IsEmpty())
                        {
                            render.data[pos.GetIndex()] = false;
                            if ((x & 0xF) == 0)
                            {
                                render.visibleFacing |= 1 << (int)Facing.West;
                            }
                            else
                            if ((x & 0xF) == 0xF)
                            {
                                render.visibleFacing |= 1 << (int)Facing.East;
                            }

                            if ((y & 0xF) == 0)
                            {
                                render.visibleFacing |= 1 << (int)Facing.Down;
                            }
                            else
                            if ((y & 0xF) == 0xF)
                            {
                                render.visibleFacing |= 1 << (int)Facing.Up;
                            }

                            if ((z & 0xF) == 0)
                            {
                                render.visibleFacing |= 1 << (int)Facing.South;
                            }
                            else
                            if ((z & 0xF) == 0xF)
                            {
                                render.visibleFacing |= 1 << (int)Facing.North;
                            }
                        }
                        else
                        {
                            render.data[pos.GetIndex()] = true;
                            render.empty--;
                        }
                    }
                }
            }

            render.visibilityVersion = version;
            render.calculating = false;
        }
    }
}
