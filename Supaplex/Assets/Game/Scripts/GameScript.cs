using Entitas;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public GameObject stone;
    public GameObject wall;
    public GameObject gamePool;

    Systems systems = null;
    Game.Logic.Level level;

    void Start()
    {
        Debug.Log("Start");
        Game.View.View.root = gameObject;
        Game.View.View.StonePrefab = stone;
        Game.View.View.WallPrefab = wall;
        Game.View.View.pool = gamePool;

        var contexts = new Contexts();
        systems = createSystems(contexts);
        systems.Initialize();

        level = Game.Logic.Level.Load("Level1");

        var walls = new GameObject("walls");
        Game.View.View.walls = walls;
        walls.transform.SetParent(transform);
        walls.transform.localPosition = Vector3.zero;
        walls.transform.localScale = Vector3.one;

        var stones = new GameObject("stones");
        Game.View.View.stones = stones;
        stones.transform.SetParent(transform);
        stones.transform.localPosition = Vector3.zero;
        stones.transform.localScale = Vector3.one;
    }

    void Awake()
    {
        Debug.Log("Awake");
    }

    void Update()
    {
        systems.Execute();
        systems.Cleanup();
    }

    Systems createSystems(Contexts contexts)
    {
        return new Feature("Systems")
            .Add(new Game.InputSystem(contexts))
            .Add(new Feature("Logic")
                .Add(new Game.Logic.GameSystem(contexts))
                .Add(new Game.Logic.PlayerPositionSystem(contexts))
                //.Add(new Logic.StoneFallSystem(contexts))
            )
            .Add(new Feature("View")
                .Add(new Game.View.CreateViewSystem(contexts))
                .Add(new Game.View.PositionViewSystem(contexts))
                .Add(new Game.View.PlayerPositionViewSystem(contexts))

                .Add(new Game.View.DestroyViewSystem(contexts))
            )
            .Add(new Game.Logic.CleanupSystem(contexts))
            ;
    }
}
