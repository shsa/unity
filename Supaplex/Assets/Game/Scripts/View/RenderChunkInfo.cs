using System;
using UnityEngine;
using Game.Logic;

namespace Game.View
{
    public class RenderChunkInfo
    {
        public RenderChunk renderChunk;
        public Facing facing;
        public FacingSet setFacing;

        public RenderChunkInfo(RenderChunk renderChunk, Facing facing)
        {
            this.renderChunk = renderChunk;
            this.facing = facing;
        }
    }
}
