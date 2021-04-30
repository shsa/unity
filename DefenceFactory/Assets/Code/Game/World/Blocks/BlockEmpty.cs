using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World.Blocks
{
    public class BlockEmpty : Block
    {
        public BlockEmpty() : base(nameof(ModelTypeEnum.Simple) + "/" + nameof(ModelEnum.Empty))
        {

        }
    }
}
