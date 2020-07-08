﻿using Game.Logic.World;
using System;
using System.Linq;
using UnityEngine;

namespace Game.View.World.Models
{
    public class SimpleModel : Model
    {
        BlockPart[][] parts;

        public SimpleModel() : base()
        {
            var count = (int)Enum.GetValues(typeof(BlockType)).Cast<BlockType>().Max() + 1;
            parts = new BlockPart[count][];
        }

        public override void Register(Block block)
        {
            var index = MaterialProvider.AllocateBlock();
            var textureName = "Stone2";
            var textureNormal = "NormalMap";
            var uv = MaterialProvider.Replace(index, GetTexture(textureName), TextureType.Main);
            MaterialProvider.Replace(index, GetTexture(textureNormal), TextureType.Normal);

            var pp = new BlockPart[6];
            void addPart(Facing facing, Quaternion rotation)
            {
                var part = new BlockPart();
                part.vertices = BlockSideVertexes(rotation);
                part.triangles = BlockSideTriangles();
                part.uv = BlockSideUVs(uv);
                pp[(int)facing] = part;
            }
            addPart(Facing.North, Quaternion.identity);
            addPart(Facing.East, Quaternion.Euler(0, -90, 0));
            addPart(Facing.South, Quaternion.Euler(0, 180, 0));
            addPart(Facing.West, Quaternion.Euler(0, 90, 0));
            addPart(Facing.Up, Quaternion.Euler(-90, 0, 0));
            addPart(Facing.East, Quaternion.Euler(90, 0, 0));

            parts[(int)block.id] = pp;
        }

        public override BlockPart GetBlockPart(Block block, Facing facing)
        {
            var pp = parts[(int)block.id];
            return pp[(int)facing];
        }

        public static Vector3[] BlockSideVertexes(Quaternion rotation)
        {
            return new Vector3[] {
                rotation * new Vector3(-0.5f, -0.5f, 0.5f),
                rotation * new Vector3(-0.5f, 0.5f, 0.5f),
                rotation * new Vector3(0.5f, 0.5f, 0.5f),
                rotation * new Vector3(0.5f, -0.5f, 0.5f)
            };
        }

        public static int[] BlockSideTriangles()
        {
            return new int[]
            {
                0, 1, 2,
                0, 2, 3
            };
        }

        public static Vector2[] BlockSideUVs(Rect uvRect)
        {
            return new Vector2[]
            {
                new Vector2(uvRect.xMin, uvRect.yMin),
                new Vector2(uvRect.xMin, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMin)
            };
        }
    }
}
