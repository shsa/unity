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

        public IChunkView CreateChunk(Chunk chunk)
        {
            var gameObject = new GameObject($"{chunk.Position.x}, {chunk.Position.y}");
            var chunkView = gameObject.AddComponent<ChunkView>();
            chunkView.ViewService = this;
            chunkView.Create(chunk);
            return chunkView;
        }

        public BlockView CreateBlock(Transform transform, BlockData blockData)
        {
            switch (blockData.GetBlockId())
            {
                case BlockType.Stone: return Instantiate(_stoneBlock, transform);
                default: return Instantiate(_emptyBlock, transform);
            }
        }

        public IView CreatePlayerView(float x, float y)
        {
            _playerView.transform.localPosition = new Vector3(x, y, _playerView.transform.localPosition.z);
            return _playerView.GetComponent<IView>();
        }
    }
}
