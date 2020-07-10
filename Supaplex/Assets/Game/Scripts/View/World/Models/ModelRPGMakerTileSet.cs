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

        public ModelRPGMakerTileSet() : base()
        {
            var count = (int)Enum.GetValues(typeof(BlockType)).Cast<BlockType>().Max() + 1;
            parts = new BlockParts[count];
            PrepareSamples();
        }

        public override void Register(Block block)
        {
            parts[(int)block.id] = RegisterBlock("TileSetDemo2");
        }

        #region Prepare

        class TileInfo
        {
            public int index;
            public CornerEnum corner;
            public byte sides;
            public byte ignoreSides;

            public override string ToString()
            {
                return $"{index} " + CompasEnumExtension.ToText(sides);
            }
        }
        class SampleInfo
        {
            public int mask;
            public Vec2i[] corners = new Vec2i[4];
            public Rect uv;

            Vec2i FromIndex(int index)
            {
                return new Vec2i(index % 4, index / 4);
            }

            public SampleInfo(int mask, int tl, int tr, int bl, int br)
            {
                this.mask = mask;
                this.corners[(int)CornerEnum.TL] = FromIndex(tl);
                this.corners[(int)CornerEnum.TR] = FromIndex(tr);
                this.corners[(int)CornerEnum.BL] = FromIndex(bl);
                this.corners[(int)CornerEnum.BR] = FromIndex(br);
            }
        }
        Vec2i tileGridSize = new Vec2i(4, 6);
        Dictionary<int, SampleInfo> samples = new Dictionary<int, SampleInfo>();
        SampleInfo[] indexes = new SampleInfo[256];

        void PrepareSamples()
        {
            var templates = new List<TileInfo>();
            var corners = new byte[4];
            corners[(int)CornerEnum.TL] = CompasEnumExtension.ToCompasSet("WwN");
            corners[(int)CornerEnum.TR] = CompasEnumExtension.ToCompasSet("NnE");
            corners[(int)CornerEnum.BL] = CompasEnumExtension.ToCompasSet("SsW");
            corners[(int)CornerEnum.BR] = CompasEnumExtension.ToCompasSet("EeS");

            void update(TileInfo info, int index, CornerEnum corner, byte sides, byte ignoreSides)
            {
                info.index = index;
                info.corner = corner;
                var cornerSet = corners[(int)corner];
                info.sides = (byte)(sides & cornerSet);
                info.ignoreSides = (byte)(ignoreSides & cornerSet);
                templates.Add(info);
            }

            void add(int tl, int tr, int bl, int br, string sides, string ignoreSides)
            {
                var _sides = CompasEnumExtension.ToCompasSet(sides);
                var _ignoreSides = CompasEnumExtension.ToCompasSet(ignoreSides);

                update(new TileInfo(), tl, CornerEnum.TL, _sides, _ignoreSides);
                update(new TileInfo(), tr, CornerEnum.TR, _sides, _ignoreSides);
                update(new TileInfo(), bl, CornerEnum.BL, _sides, _ignoreSides);
                update(new TileInfo(), br, CornerEnum.BR, _sides, _ignoreSides);
            }
            add(12, 13, 8, 9, "EeS", "wns");
            add(14, 15, 10, 11, "SsW", "new");
            add(4, 5, 0, 1, "NnE", "esw");
            add(6, 7, 2, 3, "NWw", "nes");

            add(22, 23, 18, 19, "NESW", "");

            TileInfo find(byte mask, CornerEnum corner)
            {
                var cornerSet = (byte)(corners[(int)corner] & mask);
                for (int i = 0; i < templates.Count; i++)
                {
                    var info = templates[i];
                    if (info.corner == corner)
                    {
                        var b = cornerSet & ~info.ignoreSides;
                        if (b == info.sides)
                        {
                            return info;
                        }
                    }
                }
                throw new NotImplementedException();
            }

            samples.Add(0, new SampleInfo(0, 20, 21, 16, 17));
            samples.Add(255, new SampleInfo(255, 9, 10, 5, 6));
            for (int n = 0; n <= 0xFF; n++)
            {
                var text = CompasEnumExtension.ToText((byte)n);
                var tl = find((byte)n, CornerEnum.TL);
                var tr = find((byte)n, CornerEnum.TR);
                var bl = find((byte)n, CornerEnum.BL);
                var br = find((byte)n, CornerEnum.BR);
                var index = tl.sides | tr.sides | bl.sides | br.sides;
                if (!samples.TryGetValue(index, out var sample))
                {
                    sample = new SampleInfo(index, tl.index, tr.index, bl.index, br.index);
                    samples.Add(index, sample);
                }
                indexes[n] = sample;
            }
        }

        BlockParts RegisterBlock(string textureName)
        {
            var render = RenderTexture.active;

            var texture = GetTexture(textureName);
            var src = MaterialProvider.CreateTexture2D(texture.width, texture.height);
            Graphics.ConvertTexture(texture, src);
            var tileSize = new Vector2Int(src.width / tileGridSize.x, src.height / tileGridSize.y);
            var tmpTxt = MaterialProvider.CreateTexture2D(tileSize.x * 2, tileSize.y * 2);

            var corners = new Vec2i[4];
            corners[(int)CornerEnum.TL] = new Vec2i(0, tileSize.x);
            corners[(int)CornerEnum.TR] = new Vec2i(tileSize.x, tileSize.y);
            corners[(int)CornerEnum.BL] = new Vec2i(0, 0);
            corners[(int)CornerEnum.BR] = new Vec2i(tileSize.x, 0);
            foreach (var sample in samples.Values)
            {
                for (int i = 0; i < 4; i++)
                {
                    var x = sample.corners[i].x * tileSize.x;
                    var y = sample.corners[i].y * tileSize.y;
                    Graphics.CopyTexture(src, 0, 0, x, y, tileSize.x, tileSize.y, tmpTxt, 0, 0, corners[i].x, corners[i].y);
                }
                var index = MaterialProvider.AllocateBlock();
                sample.uv = MaterialProvider.Replace(index, tmpTxt, TextureType.Main);
            }
            RenderTexture.active = render;
            UnityEngine.Object.Destroy(tmpTxt);

            void addPart(BlockPart[] pp, Facing facing, Rect uvRect)
            {
                pp[(int)facing] = Geometry.GetBlockPart(facing, uvRect);
            }

            var ss = new BlockParts();
            ss.parts = new BlockPart[256][];
            for (int sides = 0; sides <= 0xFF; sides++)
            {
                var sample = indexes[sides];

                var pp = new BlockPart[6];
                addPart(pp, Facing.North, sample.uv);
                addPart(pp, Facing.East, sample.uv);
                addPart(pp, Facing.South, sample.uv);
                addPart(pp, Facing.West, sample.uv);
                addPart(pp, Facing.Up, sample.uv);
                addPart(pp, Facing.Down, sample.uv);
                ss.parts[sides] = pp;
            }

            return ss;
        }

        #endregion

        public override int GetTextureCount(Block block)
        {
            return samples.Count;
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
    }
}
