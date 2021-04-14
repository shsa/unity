using DefenceFactory.World;
using Leopotam.Ecs.Types;
using UnityEngine;

namespace DefenceFactory
{
    class ViewService : MonoBehaviour, IViewService
    {
        [SerializeField] private Transform _root;
        [SerializeField] private PlayerView _playerViewPrefab;
        [SerializeField] private Transform _camera;

        public IView CreateBlock(BlockPos pos, BlockData blockData)
        {
            var position = new Vector2(pos.x, pos.y);
            var rotation = Quaternion.identity;

            var view = Instantiate(_playerViewPrefab, position, rotation, _root);
            var sr = view.GetComponent<SpriteRenderer>();
            sr.color = Color.gray;
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
            throw new System.NotImplementedException();
        }
    }
}
