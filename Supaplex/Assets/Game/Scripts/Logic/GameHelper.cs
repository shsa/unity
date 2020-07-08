using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Logic
{
    public static class GameHelper
    {
        public static IEnumerable<GameEntity> GetObjects(this Contexts contexts, Vector2Int position)
        {
            return contexts.game.GetEntitiesWithPositionInt(position)
                .Where(e => e.hasObjectType);
        }

        public static bool IsEmpty(this Contexts contexts, Vector2Int position)
        {
            var objects = contexts.GetObjects(position);
            foreach (var obj in objects)
            {
                //if (obj.objectType.value != ObjectType.Empty)
                //{
                //    return false;
                //}
                throw new System.NotImplementedException();
            }
            return true;
        }

        public static bool HasObject(this Contexts contexts, Vector2Int position, BlockType type)
        {
            var objects = contexts.GetObjects(position);
            foreach (var obj in objects)
            {
                //if (obj.objectType.value == type)
                //{
                //    return true;
                //}
                throw new System.NotImplementedException();
            }
            return false;
        }

        public static bool Move(this Contexts contexts, Vector2Int from, Vector2Int to)
        {
            if (!contexts.IsEmpty(to))
                return false;

            var objects = contexts.GetObjects(from).ToArray();
            foreach (var obj in objects)
            {
                //obj.ReplacePositionInt(to);
                //obj.ReplaceObjectState(ObjectState.Move);
                throw new System.NotImplementedException();
            }
            return true;
        }

        public static Vector2Int Floor(this PositionComponent position)
        {
            return new Vector2Int(position.value.x.Floor(), position.value.y.Floor());
        }
    }
}