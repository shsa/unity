using Game.Logic.World;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.View.World
{
    public class ModelRPGMakerTileSet : Model
    {
        BlockPart[][] parts;
        Texture2D tmpTxt;        

        public ModelRPGMakerTileSet() : base()
        {
            var count = (int)Enum.GetValues(typeof(BlockType)).Cast<BlockType>().Max() + 1;
            parts = new BlockPart[count][];
        }

        class BlockInfo
        {
            public int x;
            public int y;
            public int i;
            public int j;
            public byte sides = 0;

            public BlockInfo(int x, int y, int i, int j)
            {
                this.x = x;
                this.y = y;
                this.i = i;
                this.j = j;
            }

            public void Add(Compas dir)
            {
                sides |= (byte)(1 << (int)dir);
            }
        }
        List<BlockInfo> list = new List<BlockInfo>();

        void BuildTileSet()
        {
            var info = new BlockInfo(0, 2, 0, 0);
            info.Add(Compas.E);
            info.Add(Compas.SE);
            info.Add(Compas.S);
            list.Add(info);

            info = new BlockInfo(0, 3, 0, 0);
            info.Add(Compas.N);
            info.Add(Compas.NE);
            info.Add(Compas.E);
            info.Add(Compas.SE);
            info.Add(Compas.S);
            list.Add(info);

            info = new BlockInfo(0, 4, 0, 0);
            info.Add(Compas.N);
            info.Add(Compas.NE);
            info.Add(Compas.E);
            list.Add(info);

            info = new BlockInfo(2, 0, 0, 0);
            info.Add(Compas.N);
            info.Add(Compas.E);
            info.Add(Compas.S);
            info.Add(Compas.W);
            list.Add(info);

            info = new BlockInfo(1, 2, 0, 0);
            info.Add(Compas.E);
            info.Add(Compas.S);
            info.Add(Compas.W);
            info.Add(Compas.SE);
            info.Add(Compas.SW);
            list.Add(info);

            info = new BlockInfo(2, 2, 0, 0);
            info.Add(Compas.S);
            info.Add(Compas.W);
            info.Add(Compas.SW);
            list.Add(info);

            info = new BlockInfo(1, 3, 0, 0);
            info.Add(Compas.N);
            info.Add(Compas.S);
            info.Add(Compas.E);
            info.Add(Compas.W);
            info.Add(Compas.SW);
            info.Add(Compas.SE);
            info.Add(Compas.NE);
            info.Add(Compas.NW);
            list.Add(info);

            info = new BlockInfo(1, 4, 0, 0);
            info.Add(Compas.N);
            info.Add(Compas.E);
            info.Add(Compas.W);
            info.Add(Compas.NE);
            info.Add(Compas.NW);
            list.Add(info);

            info = new BlockInfo(2, 4, 0, 0);
            info.Add(Compas.N);
            info.Add(Compas.W);
            info.Add(Compas.NW);
            list.Add(info);

            info = new BlockInfo(2, 3, 0, 0);
            info.Add(Compas.N);
            info.Add(Compas.S);
            info.Add(Compas.W);
            info.Add(Compas.SW);
            info.Add(Compas.NW);
            list.Add(info);

        }

        public override void Register(Block block)
        {
            var textureName = "Outside_A4";
            var texture = GetTexture(textureName);
            var src = MaterialProvider.CreateTexture2D(texture.width, texture.height);
            Graphics.ConvertTexture(texture, src);
            var gridSize = new Vector2Int(8, 3);
            var tilesetSize = new Vector2Int(src.width / gridSize.x, src.height / gridSize.y);
            var i = 0;
            var j = 0;
            var tilesetPartSize = new Vector2Int(tilesetSize.x / 4, tilesetSize.y / 10);
            tmpTxt = MaterialProvider.CreateTexture2D(tilesetPartSize.x * 2, tilesetPartSize.y * 2);
            Graphics.CopyTexture(src, 0, 0, i, src.height - tilesetPartSize.y * 2, tilesetPartSize.x * 2, tilesetPartSize.y * 2, tmpTxt, 0, 0, 0, 0);

            var index = MaterialProvider.AllocateBlock();
            var uv = MaterialProvider.Replace(index, tmpTxt, TextureType.Main);

            var pp = new BlockPart[6];
            void addPart(Facing facing, Quaternion rotation)
            {
                var part = new BlockPart();
                part.vertices = BlockSideVertexes(rotation);
                part.triangles = BlockSideTriangles();
                part.uv = BlockSideUVs(uv);
                pp[(int)facing] = part;
            }
            addPart(Facing.North, Quaternion.Euler(0, 180, 0));
            addPart(Facing.East, Quaternion.Euler(0, -90, 0));
            addPart(Facing.South, Quaternion.identity);
            addPart(Facing.West, Quaternion.Euler(0, 90, 0));
            addPart(Facing.Up, Quaternion.Euler(90, 0, 0));
            addPart(Facing.Down, Quaternion.Euler(-90, 0, 0));

            parts[(int)block.id] = pp;
        }

        public override BlockPart GetBlockPart(Facing facing, Block block, BlockState state)
        {
            var pp = parts[(int)block.id];
            return pp[(int)facing];
        }

        public static Vector3[] BlockSideVertexes(Quaternion rotation)
        {
            return new Vector3[] {
                rotation * new Vector3(-0.5f, -0.5f, -0.5f),
                rotation * new Vector3(-0.5f, 0.5f, -0.5f),
                rotation * new Vector3(0.5f, 0.5f, -0.5f),
                rotation * new Vector3(0.5f, -0.5f, -0.5f)
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
