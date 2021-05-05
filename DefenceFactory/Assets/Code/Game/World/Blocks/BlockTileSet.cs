using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World.Blocks
{
    public class BlockTileSet : Block
    {
        public BlockTileSet(ModelEnum model) : base(model.ToString(), new TileSetModel(model))
        {
        }

        DirectionSet CheckDir(in IWorldReader world, int x, int y, int z, DirectionEnum dir)
        {
            ref var v = ref dir.GetVector2();
            if (this == world.GetBlock(x + v.X, y + v.Y, z))
            {
                return dir.Set();
            }
            return DirectionSet.None;
        }

        public override long GetMeta(in IWorldReader world, int x, int y, int z)
        {
            var meta = CheckDir(world, x, y, z, DirectionEnum.N)
                | CheckDir(world, x, y, z, DirectionEnum.E)
                | CheckDir(world, x, y, z, DirectionEnum.S)
                | CheckDir(world, x, y, z, DirectionEnum.W);
            return (long)meta;
        }
    }
}
