﻿using Game.Logic.World;
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

        public static Texture2D GetTexture(string name)
        {
            return Resources.Load<Texture2D>(name);
        }

        public static void Register(ModelType modelType, Model model)
        {
            REGISTER[(int)modelType] = model;
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

        public static Model GetModel(ModelType modelType)
        {
            return REGISTER[(int)modelType];
        }

        public static void Create()
        {
            var count = (int)Enum.GetValues(typeof(ModelType)).Cast<ModelType>().Max() + 1;
            REGISTER = new Model[count];

            Register(ModelType.Simple, new ModelSimple());
            Register(ModelType.RPGMakerTileSet, new ModelRPGMakerTileSet());
        }
    }
}