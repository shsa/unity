using DefenceFactory.Game.World;
using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DefenceFactory.Models
{
    public class TileSetModel : Model
    {
        class SampleInfo
        {
            public DirectionSet Mask;
            public Int2 NW;
            public Int2 NE;
            public Int2 SW;
            public Int2 SE;

            /// <summary>
            /// index like TileSetDemo.png
            /// 21 => {2, 1}
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            Int2 FromIndex(int index)
            {
                return new Int2(index / 10, index % 10);
            }

            public SampleInfo(DirectionSet mask, int nw, int ne, int sw, int se)
            {
                Mask = mask;
                NW = FromIndex(nw);
                NE = FromIndex(ne);
                SW = FromIndex(sw);
                SE = FromIndex(se);
            }
        }

        Texture2D texture;
        int x;
        int y;
        int tileWidth;
        int tileHeight;
        SampleInfo[] samples = new SampleInfo[256];

        class TileHelper
        {
            public DirectionEnum direction;
            public DirectionSet neighbors;
            public DirectionSet spaces;
            public DirectionSet ignores;
            public int column;
            public int row;

            public int Index {
                get => column * 10 + row;
            }

            public void Update(char c, DirectionSet dir)
            {
                if (c == '+')
                {
                    neighbors |= dir;
                }
                else
                if (c == '.')
                {
                    spaces |= dir;
                }
                else
                if (c == '*')
                {
                    ignores &= ~dir;
                }
            }
        }

        public TileSetModel(TileSetConfig config)
        {
            texture = Resources.Load<Texture2D>(config.texture);
            x = config.x;
            y = config.y;
            tileWidth = 64;
            tileHeight = 64;

            var corners = new Dictionary<DirectionEnum, List<TileHelper>>();

            void add(DirectionEnum dir, string mask, int column, int row)
            {
                if (!corners.TryGetValue(dir, out var list))
                {
                    list = new List<TileHelper>();
                    corners.Add(dir, list);
                }
                var tileItem = new TileHelper
                {
                    neighbors = 0,
                    spaces = 0,
                    ignores = (DirectionSet)~0
                };
                tileItem.Update(mask[0], dir.Prev().Set());
                tileItem.Update(mask[1], dir.Set());
                tileItem.Update(mask[2], dir.Next().Set());
            }

            add(DirectionEnum.NW, "+++", 2, 1);
            add(DirectionEnum.NW, "+.+", 2, 5);
            add(DirectionEnum.NW, "..+", 0, 1);
            add(DirectionEnum.NW, "+..", 2, 3);
            add(DirectionEnum.NW, ".*.", 0, 3);

            add(DirectionEnum.NE, "+++", 1, 1);
            add(DirectionEnum.NE, "+.+", 3, 5);
            add(DirectionEnum.NE, "..+", 1, 3);
            add(DirectionEnum.NE, "+..", 3, 1);
            add(DirectionEnum.NE, ".*.", 3, 3);

            add(DirectionEnum.SW, "+++", 2, 2);
            add(DirectionEnum.SW, "+.+", 2, 4);
            add(DirectionEnum.SW, "..+", 2, 0);
            add(DirectionEnum.SW, "+..", 0, 2);
            add(DirectionEnum.SW, ".*.", 0, 0);

            add(DirectionEnum.SE, "+++", 2, 2);
            add(DirectionEnum.SE, "+.+", 2, 4);
            add(DirectionEnum.SE, "..+", 2, 0);
            add(DirectionEnum.SE, "+..", 0, 2);
            add(DirectionEnum.SE, ".*.", 0, 0);

            TileHelper find(DirectionEnum dir, DirectionSet sets)
            {
                var list = corners[dir];
                foreach (var item in list)
                {
                    var t = item.ignores & sets;
                    var b = ((t & item.spaces) == item.spaces)
                        && ((t & item.neighbors) == item.neighbors);
                    if (b)
                    {
                        return item;
                    }
                }
                throw new NotImplementedException();
            }
            var NW = DirectionSet.W | DirectionSet.NW | DirectionSet.N;
            var NE = DirectionSet.N | DirectionSet.NE | DirectionSet.E;
            var SW = DirectionSet.W | DirectionSet.SW | DirectionSet.S;
            var SE = DirectionSet.S | DirectionSet.SE | DirectionSet.E;
            for (int i = 0; i <= 0xFF; i++)
            {
                var nw = find(DirectionEnum.NW, (DirectionSet)i & NW);
                var ne = find(DirectionEnum.NE, (DirectionSet)i & NE);
                var sw = find(DirectionEnum.NW, (DirectionSet)i & SW);
                var se = find(DirectionEnum.NW, (DirectionSet)i & SE);

                samples[i] = new SampleInfo((DirectionSet)i, nw.Index, ne.Index, sw.Index, se.Index);
            }
        }

        public override string GetKey(long meta)
        {
            var data = (int)(meta & 0xFF);
            var sample = samples[data];
            return "1111";
        }

        public override GameObject GetPrefab(long meta)
        {
            var data = (int)(meta & 0xFF);
            var sample = samples[data];
            throw new NotImplementedException();
            return null;
        }
    }
}
