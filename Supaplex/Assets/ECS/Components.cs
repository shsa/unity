using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

public class ValueComponent<T>
{
    public T value;

    public override string ToString()
    {
        var name = this.GetType().Name.Replace("Component", "");
        return name + ":" + value.ToString();
    }
}

public class BoolComponent
{
    public override string ToString()
    {
        var name = this.GetType().Name.Replace("Component", "");
        return name;
    }
}

[Game]
public class ChunkComponent : IComponent
{
    [EntityIndex]
    public Vector2Int position;

    public Game.Logic.World.Chunk value;

    public override string ToString()
    {
        return position.ToString();
    }
}

[Game]
public class ChunkPositionComponent : IComponent
{
    [EntityIndex]
    public Vector2Int value;

    public override string ToString()
    {
        return value.ToString();
    }
}

[Game]
public class PositionIntComponent : IComponent
{
    [EntityIndex]
    public Vector2Int value;

    public override string ToString()
    {
        return value.ToString();
    }
}

[Game]
public class PositionComponent : ValueComponent<Vector3>, IComponent
{
}

[Game]
public class MatrixComponent : ValueComponent<Matrix4x4>, IComponent
{
}

[Game]
public class ObjectTypeComponent : ValueComponent<Game.ObjectType>, IComponent
{
}

[Game]
public class ObjectStateComponent : ValueComponent<Game.ObjectState>, IComponent
{
}

[Game]
public class ViewComponent : IComponent
{
    public GameObject value;

    public override string ToString()
    {
        return value == null ? "View(null)" : "View(" + value.name + ")";
    }
}

[Game, Unique]
public class PlayerComponent : BoolComponent, IComponent
{
}

[Game]
public class DestroyedComponent : BoolComponent, IComponent
{
}