using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World.Blocks
{
    using Meta = Int32;

    public class BlockTileSet : Block
    {
        public BlockTileSet(ModelEnum model) : base(model.ToString(), new TileSetModel(model))
        {
        }

        public override Meta GetMeta(in IWorldReader world, int x, int y, int z)
        {
            var meta = DirectionSet.None;
            for (var d = DirectionEnum.First; d <= DirectionEnum.Last; d++)
            {
                ref var dir = ref d.GetVector2();
                if (this == world.GetBlock(x + dir.X, y + dir.Y, z))
                {
                    meta |= d.Set();
                }
            }
            return (Meta)meta;
        }
    }
}
