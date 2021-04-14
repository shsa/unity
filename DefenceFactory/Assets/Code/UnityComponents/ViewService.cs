using Leopotam.Ecs.Types;
using UnityEngine;

namespace DefenceFactory
{
    class ViewService : MonoBehaviour, IViewService
    {
        [SerializeField] private Transform _root;
        [SerializeField] private PlayerView _playerViewPrefab;

        public IView CreatePlayerView(int x, int y, Float3 color)
        {
            var position = new Vector2(x, y);
            var rotation = Quaternion.identity;

            var view = Instantiate(_playerViewPrefab, position, rotation, _root);
            var sr = view.GetComponent<SpriteRenderer>();
            sr.color = new Color(color.X, color.Y, color.Z);
            return view;
        }
    }
}
