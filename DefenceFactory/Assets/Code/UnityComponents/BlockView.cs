using DG.Tweening;
using UnityEngine;

namespace DefenceFactory
{
    sealed class BlockView : MonoBehaviour, IView
    {
        void IView.Destroy()
        {
            throw new System.NotImplementedException();
        }

        public void UpdatePosition(int x, int y)
        {
            transform.localPosition = new Vector3(x, y, 0);
        }
    }
}
