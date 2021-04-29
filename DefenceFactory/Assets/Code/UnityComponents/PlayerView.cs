using DG.Tweening;
using UnityEngine;

namespace DefenceFactory
{
    sealed class PlayerView : MonoBehaviour, IView
    {
        public void UpdatePosition(float x, float y)
        {
            transform.position = new Vector3(x, y, transform.position.z);
        }

        void IView.Destroy()
        {
        }
    }
}
