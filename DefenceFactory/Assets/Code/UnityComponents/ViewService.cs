using DefenceFactory.Game.World;
using DG.Tweening;
using Leopotam.Ecs.Types;
using System.Collections.Generic;
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
        Pool<string, BlockView> blockPool = new Pool<string, BlockView>();
        Dictionary<string, Models.Model> models = new Dictionary<string, Models.Model>();

        public IChunkView CreateChunk(Chunk chunk)
        {
            var chunkView = chunkPool.Pop();
            if (chunkView == null)
            {
                var gameObject = new GameObject();
                chunkView = gameObject.AddComponent<ChunkView>();
                chunkView.ViewService = this;
                chunkView.transform.SetParent(transform);
                chunkView.gameObject.isStatic = true;
            }
            else
            {
                //chunkView.gameObject.SetActive(true);
            }
            chunkView.ChunkName = $"{chunk.Position.x}, {chunk.Position.y}";
            chunkView.name = "chunk";
            chunkView.CreateBlocks(chunk);
            return chunkView;
        }

        public void PoolChunk(ChunkView chunkView)
        {
            chunkPool.Push(chunkView);
            //chunkView.gameObject.SetActive(false);
            chunkView.transform.position = new Vector3(0, 0, -1000);
        }

        public void Pool(BlockView blockView)
        {
            blockPool.Push(blockView.name, blockView);
            blockView.transform.SetParent(_pool, true);
        }

        public BlockView Unpool(BlockView prefab, Transform parent)
        {
            if (!blockPool.TryPop(prefab.name, out var newBlock))
            {
                newBlock = Instantiate(prefab, parent);
                newBlock.name = prefab.name;
                newBlock.gameObject.isStatic = true;
                return newBlock;
            }
            newBlock.transform.SetParent(parent, true);
            return newBlock;
        }

        public BlockView GetBlockPrefab(BlockData blockData)
        {
            switch (blockData.GetBlockId())
            {
                case BlockType.Stone: return _stoneBlock;
                default: return _emptyBlock;
            }
            //var modelName = blockData.GetBlock().Name;
            //if (!models.TryGetValue(modelName, out var model))
            //{
            //    var json = Resources.Load<TextAsset>(modelName);
            //    var config = JsonUtility.FromJson<Models.TileSetConfig>(json.text);
            //    model = new Models.TileSetModel(config);
            //    models.Add(modelName, model);
            //}
            //var key = model.GetKey(blockData.GetMeta());
            //if (blockPool.TryPop(key, out var obj))
            //{
            //    return obj;
            //}
            //var go = model.GetPrefab(blockData.GetMeta());
            //return go.GetComponent<BlockView>();
        }

        public IView CreatePlayerView(float x, float y)
        {
            _playerView.transform.localPosition = new Vector3(x, y, _playerView.transform.localPosition.z);
            return _playerView.GetComponent<IView>();
        }

        public void DrawChunks()
        {
            var blockPos = BlockPos.From(new Float2(_playerView.transform.localPosition.x, _playerView.transform.localPosition.y));
            var chunkPos = blockPos.ChunkPos;
            var min = chunkPos.MinBlockPos();
            var max = chunkPos.MaxBlockPos();
            var start = new Vector3(min.x - 0.5f, min.y - 0.5f, 0);
            var step = new Vector3(max.x - min.x + 1, max.y - min.y + 1, 0);
            var lineSize = 150;
            for (int i = -9; i <= 9; i++)
            {
                Debug.DrawLine(new Vector3(start.x + i * step.x, start.y - lineSize, 0), new Vector3(start.x + i * step.x, start.y + lineSize, 0), Color.red);
                Debug.DrawLine(new Vector3(start.x - lineSize, start.y + i * step.y, 0), new Vector3(start.x + lineSize, start.y + i * step.y, 0), Color.red);
            }
        }

        void Update()
        {
            DrawChunks();
        }

    }
}
