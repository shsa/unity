using Entitas;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public GameObject stone;
    public GameObject wall;

    Systems systems;
    Level level;

    void Start()
    {
        View.View.root = gameObject;
        View.View.StonePrefab = stone;
        View.View.WallPrefab = wall;

        var contexts = new Contexts();
        systems = createSystems(contexts);
        systems.Initialize();

        level = Level.Load("Level1");

        var walls = new GameObject("walls");
        View.View.walls = walls;
        walls.transform.SetParent(transform);
        walls.transform.localPosition = Vector3.zero;
        walls.transform.localScale = Vector3.one;

        var stones = new GameObject("stones");
        View.View.stones = stones;
        stones.transform.SetParent(transform);
        stones.transform.localPosition = Vector3.zero;
        stones.transform.localScale = Vector3.one;

        foreach (var obj in level)
        {
            var e = contexts.game.CreateEntity();
            e.AddPosition(obj.position);
            e.AddObjectType(obj.type);
            e.AddObjectState(ObjectState.Init);
        }
    }

    void Update()
    {
        systems.Execute();
        systems.Cleanup();

        foreach (var obj in level.GetEnumerator(ObjectType.Stone))
        {
            if (level.IsEmpty(obj.position + Vector2Int.up))
            {
                if (level.IsEmpty(obj.position + Vector2Int.left) && level.IsEmpty(obj.position + new Vector2Int(-1, -1)))
                {
                    //level.Move(obj.position, obj.position + new Vector2Int(-1, -1));
                }
            }
        }
    }

    Systems createSystems(Contexts contexts)
    {
        return new Feature("Systems")
            .Add(new Feature("Logic")
                .Add(new Logic.GameSystem(contexts))
                .Add(new Logic.StoneFallSystem(contexts))
            )
            .Add(new Feature("View")
                .Add(new View.CreateViewSystem(contexts))
                .Add(new View.PositionViewSystem(contexts))
            )
            ;
    }
}
