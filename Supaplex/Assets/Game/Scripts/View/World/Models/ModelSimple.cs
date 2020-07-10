using Game.Logic.World;
using System;
using System.Linq;
using UnityEngine;

namespace Game.View.World
{
    public sealed class ModelSimple : Model
    {
        BlockPart[][] parts;

        public ModelSimple() : base()
        {
            var count = (int)Enum.GetValues(typeof(BlockType)).Cast<BlockType>().Max() + 1;
            parts = new BlockPart[count][];
        }

        public override int GetTextureCount(Block block)
        {
            return 1;
        }

        public override void Register(Block block)
        {
            var index = MaterialProvider.AllocateBlock();
            var textureName = "Stone2";
            var textureNormal = "NormalMap";
            var uv = MaterialProvider.Replace(index, GetTexture(textureName), TextureType.Main);
            MaterialProvider.Replace(index, GetTexture(textureNormal), TextureType.Normal);

            var pp = new BlockPart[6];
            void addPart(Facing facing)
            {
                pp[(int)facing] = Geometry.GetBlockPart(facing, uv);
            }
            addPart(Facing.North);
            addPart(Facing.East);
            addPart(Facing.South);
            addPart(Facing.West);
            addPart(Facing.Up);
            addPart(Facing.Down);

            parts[(int)block.id] = pp;
        }

        public override BlockPart GetBlockPart(Facing facing, Block block, BlockState state)
        {
            var pp = parts[(int)block.id];
            return pp[(int)facing];
        }
    }
}
