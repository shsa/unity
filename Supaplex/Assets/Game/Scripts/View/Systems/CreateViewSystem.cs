using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace View
{
    public class CreateViewSystem : ReactiveSystem<GameEntity>
    {
        Contexts contexts;
        public CreateViewSystem(Contexts contexts) : base(contexts.game)
        {
            this.contexts = contexts;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.ObjectType.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var gameEntity in entities)
            {
                GameObject view = null;
                switch (gameEntity.objectType.value)
                {
                    case ObjectType.Stone:
                        {
                            view = GameObject.Instantiate(View.StonePrefab, View.stones.transform);
                        }
                        break;
                    case ObjectType.Wall:
                        {
                            view = GameObject.Instantiate(View.WallPrefab, View.walls.transform);
                        }
                        break;
                }
                if (view != null)
                {
                    view.transform.localScale = Vector3.one;
                    view.transform.localPosition = gameEntity.position.value.ToVector3();
                    gameEntity.AddView(view);
                }
            }
        }
    }
}
