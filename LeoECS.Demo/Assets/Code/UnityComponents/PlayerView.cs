using LeoECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace LeoECS
{
    sealed class PlayerView : MonoBehaviour, IView
    {
        public void UpdatePosition(int x, int y)
        {
            transform.DOKill();
            transform.DOLocalMove(new Vector2(x, y), 5f)
                .SetSpeedBased(true)
                .SetEase(Ease.OutCubic);
        }
    }
}
