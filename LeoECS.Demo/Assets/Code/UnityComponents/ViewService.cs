using LeoECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LeoECS
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
