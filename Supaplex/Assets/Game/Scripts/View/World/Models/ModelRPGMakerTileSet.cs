using Game.Logic.World;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.View.World
{
    public class ModelRPGMakerTileSet : Model
    {
        class BlockParts
        {
            public BlockPart[][] parts;
        }

        BlockParts[] parts;
        Texture2D tmpTxt;        

        public ModelRPGMakerTileSet() : base()
        {
            var count = (int)Enum.GetValues(typeof(BlockType)).Cast<BlockType>().Max() + 1;
            parts = new BlockParts[count];
        }

        enum CornerEnum
        {
            TL,
            TR,
            BL,
            BR
        }
        byte[] cornerSides = new byte[4];

        class BlockInfo
        {
            public int x;
            public int y;
            public CornerEnum corner;
            public byte sides = 0;

            public BlockInfo(int x, int y, CornerEnum corner)
            {
                this.x = x;
                this.y = y;
                this.corner = corner;
            }

            public void Add(CompasEnum dir)
            {
                sides |= (byte)(1 << (int)dir);
            }
        }
        List<BlockInfo> list;

        void BuildTileSet()
        {
            var list0 = new List<BlockInfo>();
            var info0 = new BlockInfo(0, 2, CornerEnum.TL);
            info0.Add(CompasEnum.E);
            info0.Add(CompasEnum.SE);
            info0.Add(CompasEnum.S);
            list0.Add(info0);

            info0 = new BlockInfo(0, 3, CornerEnum.TL);
            info0.Add(CompasEnum.N);
            info0.Add(CompasEnum.NE);
            info0.Add(CompasEnum.E);
            info0.Add(CompasEnum.SE);
            info0.Add(CompasEnum.S);
            list0.Add(info0);

            info0 = new BlockInfo(0, 4, CornerEnum.TL);
            info0.Add(CompasEnum.N);
            info0.Add(CompasEnum.NE);
            info0.Add(CompasEnum.E);
            list0.Add(info0);

            info0 = new BlockInfo(2, 0, CornerEnum.TL);
            info0.Add(CompasEnum.N);
            info0.Add(CompasEnum.E);
            info0.Add(CompasEnum.S);
            info0.Add(CompasEnum.W);
            list0.Add(info0);

            info0 = new BlockInfo(1, 2, CornerEnum.TL);
            info0.Add(CompasEnum.E);
            info0.Add(CompasEnum.S);
            info0.Add(CompasEnum.W);
            info0.Add(CompasEnum.SE);
            info0.Add(CompasEnum.SW);
            list0.Add(info0);

            info0 = new BlockInfo(2, 2, CornerEnum.TL);
            info0.Add(CompasEnum.S);
            info0.Add(CompasEnum.W);
            info0.Add(CompasEnum.SW);
            list0.Add(info0);

            info0 = new BlockInfo(1, 3, CornerEnum.TL);
            info0.Add(CompasEnum.N);
            info0.Add(CompasEnum.S);
            info0.Add(CompasEnum.E);
            info0.Add(CompasEnum.W);
            info0.Add(CompasEnum.SW);
            info0.Add(CompasEnum.SE);
            info0.Add(CompasEnum.NE);
            info0.Add(CompasEnum.NW);
            list0.Add(info0);

            info0 = new BlockInfo(1, 4, CornerEnum.TL);
            info0.Add(CompasEnum.N);
            info0.Add(CompasEnum.E);
            info0.Add(CompasEnum.W);
            info0.Add(CompasEnum.NE);
            info0.Add(CompasEnum.NW);
            list0.Add(info0);

            info0 = new BlockInfo(2, 4, CornerEnum.TL);
            info0.Add(CompasEnum.N);
            info0.Add(CompasEnum.W);
            info0.Add(CompasEnum.NW);
            list0.Add(info0);

            info0 = new BlockInfo(2, 3, CornerEnum.TL);
            info0.Add(CompasEnum.N);
            info0.Add(CompasEnum.S);
            info0.Add(CompasEnum.W);
            info0.Add(CompasEnum.SW);
            info0.Add(CompasEnum.NW);
            list0.Add(info0);

            byte union(CompasEnum dir1, CompasEnum dir2, CompasEnum dir3)
            {
                return (byte)(((1 << (int)dir1) | (1 << (int)dir2) | (1 << (int)dir3)));
            }

            cornerSides[(int)CornerEnum.TL] = union(CompasEnum.W, CompasEnum.NW, CompasEnum.N);
            cornerSides[(int)CornerEnum.TR] = union(CompasEnum.N, CompasEnum.NE, CompasEnum.E);
            cornerSides[(int)CornerEnum.BR] = union(CompasEnum.E, CompasEnum.SE, CompasEnum.S);
            cornerSides[(int)CornerEnum.BL] = union(CompasEnum.S, CompasEnum.SW, CompasEnum.W);

            byte N = 1 << (int)CompasEnum.N;
            byte S = 1 << (int)CompasEnum.S;
            byte E = 1 << (int)CompasEnum.E;
            byte W = 1 << (int)CompasEnum.W;

            void cross(BlockInfo info, byte sides)
            {
                var corners = cornerSides[(int)info.corner];
                info.sides = (byte)(sides & corners);
                list.Add(info);
                if (info.sides == 0 || info.sides == N || info.sides == S || info.sides == E || info.sides == W)
                {
                    var info2 = new BlockInfo(info.x, info.y, info.corner);
                    info2.sides = info.sides;
                    switch (info.corner)
                    {
                        case CornerEnum.TL:
                            info2.sides |= 1 << (int)CompasEnum.NW;
                            break;
                        case CornerEnum.TR:
                            info2.sides |= 1 << (int)CompasEnum.NE;
                            break;
                        case CornerEnum.BL:
                            info2.sides |= 1 << (int)CompasEnum.SW;
                            break;
                        case CornerEnum.BR:
                            info2.sides |= 1 << (int)CompasEnum.SE;
                            break;
                    }
                    list.Add(info2);
                }
            }

            list = new List<BlockInfo>();
            foreach (var info in list0)
            {
                var info2 = new BlockInfo(info.x, info.y, CornerEnum.TL);
                cross(info2, info.sides);

                info2 = new BlockInfo(info.x + 1, info.y, CornerEnum.TR);
                cross(info2, info.sides);

                info2 = new BlockInfo(info.x + 1, info.y + 1, CornerEnum.BR);
                cross(info2, info.sides);

                info2 = new BlockInfo(info.x, info.y + 1, CornerEnum.BL);
                cross(info2, info.sides);
            }
        }

        public override void Register(Block block)
        {
            var textureName = "Outside_A4";
            var texture = GetTexture(textureName);
            var src = MaterialProvider.CreateTexture2D(texture.width, texture.height);
            Graphics.ConvertTexture(texture, src);
            var gridSize = new Vector2Int(8, 3);
            var tilesetGridSize = new Vector2Int(4, 10);
            var tilesetSize = new Vector2Int(src.width / gridSize.x, src.height / gridSize.y);
            var i = 0;
            var j = 1;
            var tilesetPartSize = new Vector2Int(tilesetSize.x / 4, tilesetSize.y / 10);
            tmpTxt = MaterialProvider.CreateTexture2D(tilesetPartSize.x * 2, tilesetPartSize.y * 2);

            BuildTileSet();

            void copy(BlockInfo info)
            {
                var pos = new Vector2Int((i * tilesetGridSize.x) + info.x * tilesetPartSize.x, 
                    ((gridSize.y - j - 1) * tilesetGridSize.y + tilesetGridSize.y - info.y - 1) * tilesetPartSize.y);
                Vector2Int target;
                switch (info.corner)
                {
                    case CornerEnum.TL:
                        target = new Vector2Int(0, tilesetPartSize.y);
                        break;
                    case CornerEnum.TR:
                        target = new Vector2Int(tilesetPartSize.y, tilesetPartSize.y);
                        break;
                    case CornerEnum.BL:
                        target = new Vector2Int(0, 0);
                        break;
                    case CornerEnum.BR:
                        target = new Vector2Int(tilesetPartSize.y, 0);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                Graphics.CopyTexture(src, 0, 0, pos.x, pos.y, tilesetPartSize.x, tilesetPartSize.y, tmpTxt, 0, 0, target.x, target.y);
            }

            void find(CornerEnum corner, byte sidesIn)
            {
                var corners = cornerSides[(int)corner];
                sidesIn = (byte)(sidesIn & corners);
                for (int ii = 0; ii < list.Count; ii++)
                {
                    var info = list[ii];
                    if (info.corner == corner)
                    {
                        if ((info.sides & corners) == sidesIn)
                        {
                            copy(info);
                            return;
                        }
                    }
                }
                throw new NotImplementedException();
            }

            void addPart(BlockPart[] pp, Facing facing, Quaternion rotation, ref Rect uv)
            {
                var part = new BlockPart();
                part.vertices = BlockSideVertexes(rotation);
                part.triangles = BlockSideTriangles();
                part.uv = BlockSideUVs(uv);
                pp[(int)facing] = part;
            }

            var ss = new BlockParts();
            ss.parts = new BlockPart[256][];
            for (int sides = 0; sides <= 0xFF; sides++)
            {
                find(CornerEnum.TL, (byte)sides);
                find(CornerEnum.TR, (byte)sides);
                find(CornerEnum.BL, (byte)sides);
                find(CornerEnum.BR, (byte)sides);

                var index = MaterialProvider.AllocateBlock();
                var uv = MaterialProvider.Replace(index, tmpTxt, TextureType.Main);

                var pp = new BlockPart[6];
                addPart(pp, Facing.North, Quaternion.Euler(0, 180, 0), ref uv);
                addPart(pp, Facing.East, Quaternion.Euler(0, -90, 0), ref uv);
                addPart(pp, Facing.South, Quaternion.identity, ref uv);
                addPart(pp, Facing.West, Quaternion.Euler(0, 90, 0), ref uv);
                addPart(pp, Facing.Up, Quaternion.Euler(90, 0, 0), ref uv);
                addPart(pp, Facing.Down, Quaternion.Euler(-90, 0, 0), ref uv);
                ss.parts[sides] = pp;
            }

            parts[(int)block.id] = ss;
        }

        class TileInfo
        {
            public int index;
            public CornerEnum corner;
            public byte freeSides;
            public byte ignoreSides;
        }

        public void Prepare2()
        {
            // clockwise: NE = n, ES = e, SW = s, WN = w
            CompasEnum toCompas(char c)
            {
                switch (c)
                {
                    case 'N': return CompasEnum.N;
                    case 'n': return CompasEnum.NE;
                    case 'E': return CompasEnum.E;
                    case 'e': return CompasEnum.SE;
                    case 'S': return CompasEnum.S;
                    case 's': return CompasEnum.SW;
                    case 'W': return CompasEnum.W;
                    case 'w': return CompasEnum.NW;
                    default: throw new NotImplementedException();
                }
            }

            byte toCompasSet(string text)
            {
                byte set = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    set |= (byte)(1 << (int)toCompas(text[i]));
                }
                return set;
            }

            var templates = new List<TileInfo>();
            var corners = new byte[4];
            corners[(int)CornerEnum.TL] = toCompasSet("sWwNn");
            corners[(int)CornerEnum.TR] = toCompasSet("wNnEe");
            corners[(int)CornerEnum.BL] = toCompasSet("eSsWw");
            corners[(int)CornerEnum.BR] = toCompasSet("nEeSs");

            void update(TileInfo info, int index, CornerEnum corner, byte freeSides, byte ignoreSides)
            {
                info.index = index;
                info.corner = corner;
                info.freeSides = (byte)(freeSides & corners[(int)corner]);
                info.ignoreSides = (byte)(ignoreSides & corners[(int)corner]);
                templates.Add(info);
            }

            void add(int tl, int tr, int bl, int br, string freeSides, string ignoreSides)
            {
                var _freeSides = toCompasSet(freeSides);
                var _ignoreSides = toCompasSet(ignoreSides);

                update(new TileInfo(), tl, CornerEnum.TL, _freeSides, _ignoreSides);
                update(new TileInfo(), tr, CornerEnum.TR, _freeSides, _ignoreSides);
                update(new TileInfo(), bl, CornerEnum.BL, _freeSides, _ignoreSides);
                update(new TileInfo(), br, CornerEnum.BR, _freeSides, _ignoreSides);
            }
            add(12, 13, 8, 9, "WN", "swn");
            add(14, 15, 10, 11, "NE", "wne");
            add(4, 5, 0, 1, "SW", "esw");
            add(6, 7, 2, 3, "ES", "nes");

            add(14, 13, 10, 9, "N", "wn");
            add(6, 7, 10, 11, "E", "ne");
            add(6, 5, 2, 1, "S", "es");
            add(4, 5, 8, 9, "W", "sw");

            add(9, 10, 5, 6, "", "");

            add(22, 23, 18, 19, "nesw", "");
        }

        public override BlockPart GetBlockPart(Facing facing, Block block, BlockState state)
        {
            var ss = parts[(int)block.id];
            var b = block as BlockRPGMakerTileSet;
            if (b == null)
            {
                return ss.parts[0][(int)facing];
            }
            var index = b.CalcSide(facing, state);
            return ss.parts[index][(int)facing];
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
