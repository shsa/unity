using DefenceFactory.Game.World;
using DG.Tweening;
using UnityEngine;

namespace DefenceFactory
{
    sealed class ChunkView : MonoBehaviour, IChunkView
    {
        public ViewService ViewService { get; set; }

        public string ChunkName;

        Pool<string, BlockView> blockPool = new Pool<string, BlockView>();

        public void UpdatePosition(float x, float y)
        {
            transform.localPosition = new Vector3(x, y, 0);
        }

        BlockView CreateBlock(BlockView blockPrefab)
        {
            if (!blockPool.TryPop(blockPrefab.name, out var newBlock))
            {
                newBlock = Instantiate(blockPrefab, transform);
                newBlock.BlockName = blockPrefab.name;
                newBlock.name = blockPrefab.name;
                newBlock.gameObject.isStatic = true;
            }
            else
            {
                newBlock.IsDesroyed = false;
            }

            return newBlock;
        }

        public void PoolBlock(BlockView block)
        {
            blockPool.Push(block.BlockName, block);
            block.transform.position = new Vector3(0, 0, -1000);
            block.IsDesroyed = true;
        }

        public void CreateBlocks(Chunk chunk)
        {
            transform.localPosition = new Vector3(chunk.min.x, chunk.min.y, 0);

            var blockPos = new BlockPos();
            for (var x = chunk.min.x; x <= chunk.max.x; x++)
            {
                for (var y = chunk.min.y; y <= chunk.max.y; y++)
                {
                    blockPos.Set(x, y, 0);
                    var blockData = chunk.GetBlockData(blockPos);
                    var blockPrefab = ViewService.GetBlockPrefab(blockData);
                    var block = CreateBlock(blockPrefab);
                    block.x = x;
                    block.y = y;
                    block.transform.localPosition = new Vector3(x - chunk.min.x, y - chunk.min.y, 0);
                }
            }
        }

        public void UpdateBlocks(Chunk chunk)
        {
            ClearBlocks();
            CreateBlocks(chunk);
        }

        void ClearBlocks()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var c = transform.GetChild(i);
                var blockView = c.GetComponent<BlockView>();
                if (!blockView.IsDesroyed)
                {
                    PoolBlock(blockView);
                }
            }
        }

        void IView.Destroy()
        {
            ClearBlocks();
            ViewService.PoolChunk(this);
        }
    }
}
