using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Logic
{
    public static class GameHelper
    {
        public static IEnumerable<GameEntity> GetObjects(this Contexts contexts, Vector2Int position)
        {
            return contexts.game.GetEntitiesWithPosition(position)
                .Where(e => e.hasObjectType);
        }

        public static bool IsEmpty(this Contexts contexts, Vector2Int position)
        {
            var objects = contexts.GetObjects(position);
            foreach (var obj in objects)
            {
                if (obj.objectType.value != ObjectType.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool HasObject(this Contexts contexts, Vector2Int position, ObjectType type)
        {
            var objects = contexts.GetObjects(position);
            foreach (var obj in objects)
            {
                if (obj.objectType.value == type)
                {
                    return true;
                }
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
                obj.ReplacePosition(to);
                obj.ReplaceObjectState(ObjectState.Move);
            }
            return true;
        }
    }
}