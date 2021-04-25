using DG.Tweening;
using UnityEngine;

namespace DefenceFactory
{
    sealed class PlayerView : MonoBehaviour, IView
    {
        public void UpdatePosition(int x, int y)
        {
            //transform.DOKill();
            //transform.DOLocalMove(new Vector2(x, y), 0.1f)
            //    //.SetSpeedBased(true)
            //    .SetEase(Ease.Linear);
            transform.position = new Vector3(x, y);
        }

        void IView.Destroy()
        {
            throw new System.NotImplementedException();
        }
    }
}
