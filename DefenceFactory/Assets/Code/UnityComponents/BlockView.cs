using DG.Tweening;
using UnityEngine;

namespace DefenceFactory
{
    public sealed class BlockView : MonoBehaviour, IView
    {
        public string BlockName;
        public int x;
        public int y;
        public int index;
        public bool IsDesroyed;

        void IView.Destroy()
        {
            throw new System.NotImplementedException();
        }

        public void UpdatePosition(float x, float y)
        {
            transform.localPosition = new Vector3(x, y, 0);
        }
    }
}
