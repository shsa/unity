using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Logic;

public class ValueComponent<T>
{
    public T value;

    public override string ToString()
    {
        var name = this.GetType().Name.Replace("Component", "");
        return name + ":" + value.ToString();
    }
}

[Game]
public class PositionComponent : IComponent
{
    [EntityIndex]
    public Vector2Int value;

    public override string ToString()
    {
        return value.ToString();
    }
}

[Game]
public class ObjectTypeComponent : ValueComponent<ObjectType>, IComponent
{
}

[Game]
public class ObjectStateComponent : ValueComponent<ObjectState>, IComponent
{
}

[Game]
public class ViewComponent : IComponent
{
    public GameObject value;

    public override string ToString()
    {
        return "View(" + value.name + ")";
    }
}

