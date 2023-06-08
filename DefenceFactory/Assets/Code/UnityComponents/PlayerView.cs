using DG.Tweening;
using UnityEngine;

namespace DefenceFactory
{
    sealed class PlayerView : MonoBehaviour, IView
    {
        public void UpdatePosition(int x, int y)
        {
            transform.DOKill();
            transform.DOLocalMove(new Vector2(x, y), 5f)
                //.SetSpeedBased(true)
                .SetEase(Ease.InOutCubic);
        }
    }
}
