using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World.Blocks
{
    public class BlockTileSet : Block
    {
        public BlockTileSet(ModelEnum model) : base(nameof(ModelTypeEnum.TileSet) + "/" + model.ToString())
        {

        }
    }
}
