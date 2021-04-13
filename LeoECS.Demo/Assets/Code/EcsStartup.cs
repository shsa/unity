using LeoECS.Ecs;
using Leopotam.Ecs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeoECS
{
    [RequireComponent(typeof(ViewService))]
    [RequireComponent(typeof(InputService))]
    public class EcsStartup : MonoBehaviour
    {
        EcsWorld _world;
        EcsSystems _systems;

        void Start()
        {
            // create ecs environment.
            _world = new EcsWorld();
            _systems = new EcsSystems(_world)
                .Add(new DemoInitSystem())
                .Add(new InputSystem())
                .Add(new PlayerMoveSystem())

                .Add(new PlayerViewCreateSystem())
                .Add(new PlayerViewUpdatePositionSystem())

                .Inject(GetComponent<IViewService>())
                .Inject(GetComponent<IInputService>())

                .OneFrame<Ecs.Input>()
                .OneFrame<PositionUpdated>()
                ;
            _systems.Init();
        }

        void Update()
        {
            // process all dependent systems.
            _systems.Run();
        }

        void OnDestroy()
        {
            // destroy systems logical group.
            _systems.Destroy();
            // destroy world.
            _world.Destroy();
        }
    }
}
