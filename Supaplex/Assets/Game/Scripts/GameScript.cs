using Entitas;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public GameObject stonePrefab;
    public GameObject wallPrefab;
    public GameObject gamePool;
    public GameObject player;
    public Camera _camera;
    public GameObject gameRoot;
    public Material wallMaterial;

    public int counts;
    public Vector3Int viewSize;

    Systems systems = null;

    void Awake()
    {
        Game.View.View.setup = this;

        var contexts = new Contexts();
        systems = createSystems(contexts);
        systems.Initialize();
    }

    void Start()
    {
    }

    void Update()
    {
        systems.Execute();
        systems.Cleanup();

        counts = gameRoot.transform.childCount;
    }

    Systems createSystems(Contexts contexts)
    {
        return new Feature("Systems")
            .Add(new Game.InputSystem(contexts))
            .Add(new Feature("Logic")
                .Add(new Game.Logic.GameSystem(contexts, this))
                .Add(new Game.Logic.PlayerPositionSystem(contexts))
                //.Add(new Logic.StoneFallSystem(contexts))
            )
            .Add(new Feature("View")
                .Add(new Game.View.CreateViewSystem(contexts))
                .Add(new Game.View.PositionViewSystem(contexts))
                .Add(new Game.View.PlayerPositionViewSystem(contexts))

                .Add(new Game.View.DestroyViewSystem(contexts))

                .Add(new Game.View.RenderViewSystem(contexts))
            )
            .Add(new Game.Logic.CleanupSystem(contexts))
            ;
    }
}
