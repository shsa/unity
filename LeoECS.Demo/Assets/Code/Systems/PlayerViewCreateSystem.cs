using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LeoECS.Ecs
{
    sealed class PlayerViewCreateSystem : IEcsRunSystem
    {
        private readonly IViewService _viewService = default;

        private readonly EcsFilter<Position>.Exclude<View> _views = default;

        void IEcsRunSystem.Run()
        {
            if (_views.IsEmpty())
                return;

            foreach (var i in _views)
            {
                var position = _views.Get1(i).Value;

                var view = _viewService.CreatePlayerView(position.X, position.Y);
                view.UpdatePosition(position.X, position.Y);

                _views.GetEntity(i).Get<View>().Value = view;
            }
        }
    }
}
