using DG.Tweening;
using UnityEngine;

namespace DefenceFactory
{
    sealed class ChunkView : MonoBehaviour, IView
    {
        public void UpdatePosition(float x, float y)
        {
            transform.localPosition = new Vector3(x, y, 0);
        }

        void IView.Destroy()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var c = transform.GetChild(i);
                Destroy(c.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
