using DefenceFactory.Ecs;
using DefenceFactory.Game.World;
using Leopotam.Ecs;
using UnityEngine;
using Random = System.Random;

namespace DefenceFactory
{
    [RequireComponent(typeof(ViewService))]
    [RequireComponent(typeof(InputService))]
    [RequireComponent(typeof(InventoryService))]
    sealed class EcsStartup : MonoBehaviour
    {
        EcsWorld _world;
        EcsSystems _systems;

        void Start()
        {
            // void can be switched to IEnumerator for support coroutines.

            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
#if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_systems);
#endif
            _systems
                .Add(new InputSystem())

                .Add(LogicSystems())
                .Add(ViewSystems())

                .Add(new DestroySystem())

                // register one-frame components (order is important):
                .OneFrame<PositionUpdatedFlag>()
                .OneFrame<Ecs.Input>()
                .OneFrame<Drag>()
                .OneFrame<PlaceItemFlag>()
                .OneFrame<UpdateBlock>()
                //.OneFrame<ThreadChunk>()
                .OneFrame<DestroyedFlag>()

                // inject service instances here (order doesn't important):
                .Inject(GetComponent<IInputService>())
                .Inject(GetComponent<IViewService>())
                .Inject(GetComponent<IInventoryService>())
                .Inject(new GameWorld())
                .Inject(new Random())

                .Init();
        }

        EcsSystems LogicSystems()
        {
            var systems = new EcsSystems(_world);
            systems
                .Add(new GameInitSystem())
                .Add(new PlayerInputSystem())
                .Add(new PlayerMoveSystem())
                .Add(new StartThreadChunkSystem())
                .Add(new ThreadChunkSystem())
                .Add(new ApplyThreadChunkSystem())
                .Add(new PlaceItemSystem())
                .Add(new UpdateBlockSystem())
                .Add(new CreateChunkSystem())
                .Add(new DestroyChunkSystem())
                ;
            return systems;
        }

        EcsSystems ViewSystems()
        {
            var systems = new EcsSystems(_world);
            systems
                .Add(new PlayerViewCreateSystem())
                .Add(new PlayerViewPositionUpdatedSystem())
                .Add(new CreateChunkViewSystem())
                .Add(new UpdateChunkViewSystem())
                .Add(new DestroyChunkViewSystem())
                ;
            return systems;
        }

        void Update()
        {
            _systems?.Run();
        }

        void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
                _world.Destroy();
                _world = null;
            }
        }
    }
}