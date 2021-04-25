using DefenceFactory.World;
using DG.Tweening;
using Leopotam.Ecs.Types;
using UnityEngine;

namespace DefenceFactory
{
    class ViewService : MonoBehaviour, IViewService
    {
        [SerializeField] private Transform _root;
        [SerializeField] private PlayerView _playerViewPrefab;
        [SerializeField] private Transform _camera;
        [SerializeField] private BlockView _stoneBlock;

        public IView CreateChunk(Chunk chunk)
        {
            var gameObject = new GameObject($"{chunk.Position.x}, {chunk.Position.y}");
            var chunkView = gameObject.AddComponent<ChunkView>();
            var minPos = chunk.Position.MinBlockPos();
            var maxPos = chunk.Position.MaxBlockPos();
            chunkView.transform.localPosition = new Vector3(minPos.x, minPos.y, 0);

            var rotation = Quaternion.identity;

            var blockPos = new BlockPos();
            for (var x = minPos.x; x <= maxPos.x; x++)
            {
                for (var y = minPos.y; y <= maxPos.y; y++)
                {
                    blockPos.Set(x, y, 0);
                    if (chunk.GetBlockData(blockPos).GetBlockId() == BlockType.Stone)
                    {
                        var block = Instantiate(_stoneBlock, chunkView.transform, true);
                        block.transform.localPosition = new Vector3(x - minPos.x, y - minPos.y, 0);
                    }
                }
            }
            return chunkView;
        }

        public IView CreateBlock(BlockPos pos, BlockData blockData)
        {
            var position = new Vector2(pos.x, pos.y);
            var rotation = Quaternion.identity;

            var view = Instantiate(_stoneBlock, position, rotation, _root);
            return view;
        }

        public IView CreatePlayerView(int x, int y, Float3 color)
        {
            var position = new Vector2(x, y);
            var rotation = Quaternion.identity;

            var view = Instantiate(_playerViewPrefab, position, rotation, _root);
            var sr = view.GetComponent<SpriteRenderer>();
            sr.color = new Color(color.X, color.Y, color.Z);
            return view;
        }

        public void SetPlayerPosition(int x, int y)
        {
            _camera.DOKill();
            _camera.DOLocalMove(new Vector3(x, y, _camera.localPosition.z), 1f)
                //.SetSpeedBased(true)
                .SetEase(Ease.Linear);
        }
    }
}
