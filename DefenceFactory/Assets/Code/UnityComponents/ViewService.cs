using UnityEngine;

namespace DefenceFactory
{
    class ViewService : MonoBehaviour, IViewService
    {
        [SerializeField] private Transform _root;
        [SerializeField] private PlayerView _playerViewPrefab;

        public IView CreatePlayerView(int x, int y)
        {
            var position = new Vector2(x, y);
            var rotation = Quaternion.identity;

            var pieceView = Instantiate(_playerViewPrefab, position, rotation, _root);

            return pieceView;
        }
    }
}
