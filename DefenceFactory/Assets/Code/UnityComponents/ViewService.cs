using DefenceFactory.Game.World;
using DG.Tweening;
using Leopotam.Ecs.Types;
using UnityEngine;

namespace DefenceFactory
{
    sealed class ViewService : MonoBehaviour, IViewService
    {
        [SerializeField] private Transform _root;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private Transform _camera;
        [SerializeField] private BlockView _emptyBlock;
        [SerializeField] private BlockView _stoneBlock;
        [SerializeField] private Transform _pool;

        Pool<ChunkView> chunkPool = new Pool<ChunkView>();

        public IChunkView CreateChunk(Chunk chunk)
        {
            var chunkView = chunkPool.Pop();
            if (chunkView == null)
            {
                var gameObject = new GameObject();
                chunkView = gameObject.AddComponent<ChunkView>();
                chunkView.ViewService = this;
            }
            else
            {
                //chunkView.gameObject.SetActive(true);
            }
            chunkView.transform.localPosition = Vector3.zero;
            chunkView.name = $"{chunk.Position.x}, {chunk.Position.y}";
            chunkView.CreateBlocks(chunk);
            return chunkView;
        }

        public void PoolChunk(ChunkView chunkView)
        {
            chunkPool.Push(chunkView);
            //chunkView.gameObject.SetActive(false);
            chunkView.transform.position = new Vector3(0, 0, -1000);
        }

        public BlockView GetBlockPrefab(BlockData blockData)
        {
            switch (blockData.GetBlockId())
            {
                case BlockType.Stone: return _stoneBlock;
                default: return _emptyBlock;
            }
        }

        public IView CreatePlayerView(float x, float y)
        {
            _playerView.transform.localPosition = new Vector3(x, y, _playerView.transform.localPosition.z);
            return _playerView.GetComponent<IView>();
        }
    }
}
