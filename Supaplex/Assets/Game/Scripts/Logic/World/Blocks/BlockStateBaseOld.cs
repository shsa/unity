using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Logic.World.Blocks
{
    public partial class BlockStateOld
    {
        static int componentCount;
        static Stack<BlockStateComponent>[] componentPools;

        BlockStateComponent[] components;

        public bool HasComponent(StateEnum state)
        {
            return components[(int)state] != null;
        }

        public BlockStateComponent this[StateEnum state] {
            get {
                return components[(int)state];
            }
            set {
                components[(int)state] = value;
            }
        }

        Stack<BlockStateComponent> GetComponentPool(StateEnum state)
        {
            var componentPool = componentPools[(int)state];
            if (componentPool == null)
            {
                componentPool = new Stack<BlockStateComponent>();
                componentPools[(int)state] = componentPool;
            }

            return componentPool;
        }

        public T GetComponent<T>(StateEnum state) where T : BlockStateComponent
        {
            return components[(int)state] as T;
        }

        public T CreateComponent<T>(StateEnum state) where T : BlockStateComponent
        {
            var componentPool = GetComponentPool(state);
            return componentPool.Count > 0 ? componentPool.Pop() as T: Activator.CreateInstance<T>();
        }

        public void AddComponent(StateEnum state, BlockStateComponent component)
        {
            if (HasComponent(state))
            {
                replaceComponent(state, component);
            }
            else
            {
                components[(int)state] = component;
            }
        }

        public void RemoveComponent(StateEnum state)
        {
            if (HasComponent(state))
            {
                replaceComponent(state, null);
            }
        }

        void replaceComponent(StateEnum state, BlockStateComponent replacement)
        {
            var previousComponent = components[(int)state];
            if (replacement != previousComponent)
            {
                components[(int)state] = replacement;
                GetComponentPool(state).Push(previousComponent);
            }
        }

        public BlockStateOld()
        {
            components = new BlockStateComponent[componentCount];
            componentPools = new Stack<BlockStateComponent>[componentCount];
        }

        static BlockStateOld()
        {
            componentCount = (int)Enum.GetValues(typeof(StateEnum)).Cast<StateEnum>().Max() + 1;
        }
    }
}
