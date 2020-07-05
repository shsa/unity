using System;
using UnityEngine;
using Game.Logic;

namespace Game.View
{
    public class RenderChunk : IDisposable
    {
        public FacingSet sides;
        FacingSet[] data;
        public Chunk chunk { get; private set; }
        public Bounds bounds { get; private set; }
        public int frameIndex { get; private set; }

        public RenderChunk(Chunk chunk)
        {
            this.chunk = chunk;
            this.bounds = chunk.position.bounds;
            this.chunk.cubeChanged += Chunk_cubeChanged;
            data = new FacingSet[4096]; // 16 * 16 * 16
        }

        public FacingSet this[Vector3Int pos] {
            get {
                return data[ChunkCube.GetIndex(pos)];
            }
        }

        private void Chunk_cubeChanged(object sender, ChunkChangeEvent e)
        {
            //throw new System.NotImplementedException();
        }

        public bool SetFrameIndex(int value)
        {
            if (frameIndex == value)
            {
                return false;
            }
            else
            {
                frameIndex = value;
                return true;
            }
        }

        public bool IsVisible(Facing facing)
        {
            return sides.HasSide(facing);
        }

        public void CalcVisibility()
        {
            var min = chunk.position.min;
            var max = chunk.position.max;
            var pos = Vector3Int.zero;
            sides = FacingSet.None;
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    for (int z = min.z; z <= max.z; z++)
                    {
                        pos.Set(x, y, z);
                        if (!chunk.world.IsEmpty(pos))
                        {
                            var set = FacingSet.None;
                            for (Facing side = Facing.First; side <= Facing.Last; side++)
                            {
                                if (chunk.world.IsEmpty(pos.Offset(side)))
                                {
                                    set |= side.GetSet();
                                }
                            }
                            data[ChunkCube.GetIndex(pos)] = set;
                        }
                        else
                        {
                            if ((x & 0xF) == 0)
                            {
                                sides |= FacingSet.West;
                            }
                            else
                            if ((x & 0xF) == 0xF)
                            {
                                sides |= FacingSet.East;
                            }

                            if ((y & 0xF) == 0)
                            {
                                sides |= FacingSet.Down;
                            }
                            else
                            if ((y & 0xF) == 0xF)
                            {
                                sides |= FacingSet.Up;
                            }

                            if ((z & 0xF) == 0)
                            {
                                sides |= FacingSet.South;
                            }
                            else
                            if ((z & 0xF) == 0xF)
                            {
                                sides |= FacingSet.North;
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            chunk.cubeChanged -= Chunk_cubeChanged;
        }
    }
}
