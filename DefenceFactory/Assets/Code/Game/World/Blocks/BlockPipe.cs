using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World.Blocks
{
    public class BlockPipe : Block
    {
        public BlockPipe(ModelEnum model) : base(model.ToString(), new Model(ModelTypeEnum.Pipe, model))
        {
        }

        DirectionSet GetDir(in IWorldReader world, int x, int y, int z, DirectionEnum dir)
        {
            ref var v = ref dir.GetVector2();
            if (this == world.GetBlock(x + v.X, y + v.Y, z))
            {
                return dir.Set();
            }
            return DirectionSet.None;
        }

        public override Meta GetMeta(in IWorldReader world, int x, int y, int z)
        {
            var meta = GetDir(world, x, y, z, DirectionEnum.N)
                | GetDir(world, x, y, z, DirectionEnum.E)
                | GetDir(world, x, y, z, DirectionEnum.S)
                | GetDir(world, x, y, z, DirectionEnum.W);
            return (Meta)meta;
        }
    }
}
