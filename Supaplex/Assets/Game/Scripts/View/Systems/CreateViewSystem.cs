using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Game.View
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
                GameObject view = CreateView.GetView(gameEntity);
                if (view != null)
                {
                    view.transform.localScale = Vector3.one;
                    view.transform.localPosition = gameEntity.position.value;
                    gameEntity.AddView(view);
                }
            }
        }
    }
}
