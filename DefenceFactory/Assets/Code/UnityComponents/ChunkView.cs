using DefenceFactory.Game.World;
using DG.Tweening;
using UnityEngine;

namespace DefenceFactory
{
    sealed class ChunkView : MonoBehaviour, IChunkView
    {
        public ViewService ViewService { get; set; }

        public void UpdatePosition(float x, float y)
        {
            transform.localPosition = new Vector3(x, y, 0);
        }

        public void Create(Chunk chunk)
        {
            var minPos = chunk.Position.MinBlockPos();
            var maxPos = chunk.Position.MaxBlockPos();
            transform.localPosition = new Vector3(minPos.x, minPos.y, 0);

            var blockPos = new BlockPos();
            for (var x = minPos.x; x <= maxPos.x; x++)
            {
                for (var y = minPos.y; y <= maxPos.y; y++)
                {
                    blockPos.Set(x, y, 0);
                    var blockData = chunk.GetBlockData(blockPos);
                    var block = ViewService.CreateBlock(transform, blockData);
                    block.transform.localPosition = new Vector3(x - minPos.x, y - minPos.y, 0);
                }
            }
        }

        public void Update(Chunk chunk)
        {
            Clear();
            Create(chunk);
        }

        void Clear()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var c = transform.GetChild(i);
                Destroy(c.gameObject);
            }
        }

        void IView.Destroy()
        {
            Clear();
            Destroy(gameObject);
        }
    }
}
