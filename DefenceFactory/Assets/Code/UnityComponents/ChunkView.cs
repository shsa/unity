using DefenceFactory.Game.World;
using DG.Tweening;
using UnityEngine;

namespace DefenceFactory
{
    sealed class ChunkView : MonoBehaviour, IChunkView
    {
        public ViewService ViewService { get; set; }

        Pool<string, BlockView> blockPool = new Pool<string, BlockView>();

        public void UpdatePosition(float x, float y)
        {
            transform.localPosition = new Vector3(x, y, 0);
        }

        BlockView CreateBlock(BlockView blockPrefab, Transform transform)
        {
            var newBlock = blockPool.Pop(blockPrefab.name);
            if (newBlock == default)
            {
                newBlock = Instantiate(blockPrefab, transform);
                newBlock.name = blockPrefab.name;
            }
            else
            {
                //newBlock.transform.SetParent(transform);
                //newBlock.gameObject.SetActive(true);
            }

            return newBlock;
        }

        public void PoolBlock(BlockView block)
        {
            blockPool.Push(block.name, block);
            //block.gameObject.SetActive(false);
            block.transform.position = new Vector3(0, 0, -1000);
        }

        public void CreateBlocks(Chunk chunk)
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
                    var blockPrefab = ViewService.GetBlockPrefab(blockData);
                    var block = CreateBlock(blockPrefab, transform);
                    block.transform.localPosition = new Vector3(x - minPos.x, y - minPos.y, 0);
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
                PoolBlock(c.GetComponent<BlockView>());
            }
        }

        void IView.Destroy()
        {
            ClearBlocks();
            ViewService.PoolChunk(this);
        }
    }
}
