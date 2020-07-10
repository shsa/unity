using Game.Logic.World;
using System;
using System.Linq;
using UnityEngine;

namespace Game.View.World
{
    public abstract class Model
    {
        static Model[] REGISTER;

        public Model()
        {
        }

        public abstract void Register(Block block);

        public abstract BlockPart GetBlockPart(Facing facing, Block block, BlockState state);

        public abstract int GetTextureCount(Block block);

        public static Texture2D GetTexture(string name)
        {
            return Resources.Load<Texture2D>(name);
        }

        public static void Add(ModelType modelType, Model model)
        {
            REGISTER[(int)modelType] = model;
        }

        static int TextureCount(ModelType modelType)
        {
            var model = REGISTER[(int)modelType];
            var count = 0;
            if (model != null)
            {
                for (int i = 0; i < Block.REGISTER.Length; i++)
                {
                    var block = Block.REGISTER[i];
                    if (block != null)
                    {
                        if (block.model == modelType)
                        {
                            count += model.GetTextureCount(block);
                        }
                    }
                }
            }
            return count;
        }

        public static void Register(ModelType modelType)
        {
            var model = REGISTER[(int)modelType];
            if (model != null)
            {
                for (int i = 0; i < Block.REGISTER.Length; i++)
                {
                    var block = Block.REGISTER[i];
                    if (block != null)
                    {
                        if (block.model == modelType)
                        {
                            model.Register(block);
                        }
                    }
                }
            }
        }

        public static Model GetModel(ModelType modelType)
        {
            return REGISTER[(int)modelType];
        }

        public static void Create()
        {
            var count = (int)Enum.GetValues(typeof(ModelType)).Cast<ModelType>().Max() + 1;
            REGISTER = new Model[count];

            Add(ModelType.Simple, new ModelSimple());
            Add(ModelType.RPGMakerTileSet, new ModelRPGMakerTileSet());

            count = 0;
            for (int i = 0; i < REGISTER.Length; i++)
            {
                count += TextureCount((ModelType)i);
            }
            MaterialProvider.Create(64, count);
            for (int i = 0; i < REGISTER.Length; i++)
            {
                Register((ModelType)i);
            }
        }
    }
}
