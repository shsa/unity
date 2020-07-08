using UnityEngine;
using Entitas;

namespace Game.View
{
    public static class CreateView
    {
        static Pool<string, GameObject> pool = new Pool<string, GameObject>();

        public static GameObject GetView(this GameEntity entity)
        {
            var name = entity.objectType.value.ToString();
            var view = pool.Pop(name);
            if (view != null)
            {
                view.transform.SetParent(View.setup.gameRoot.transform);
            }
            else
            {
                switch (entity.objectType.value)
                {
                    case BlockType.Wall:
                        view = GameObject.Instantiate(View.setup.wallPrefab, View.setup.gameRoot.transform);
                        break;
                    case BlockType.Stone:
                        view = GameObject.Instantiate(View.setup.stonePrefab, View.setup.gameRoot.transform);
                        break;
                    default: return null;
                }
                view.name = name;
            }

            view.transform.localScale = Vector3.one;
            return view;
        }

        public static void Pool(GameObject view)
        {
            pool.Push(view.name, view);
            view.transform.SetParent(View.setup.gamePool.transform);
        }
    }
}
