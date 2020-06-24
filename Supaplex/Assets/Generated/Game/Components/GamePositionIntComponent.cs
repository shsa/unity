//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public PositionIntComponent positionInt { get { return (PositionIntComponent)GetComponent(GameComponentsLookup.PositionInt); } }
    public bool hasPositionInt { get { return HasComponent(GameComponentsLookup.PositionInt); } }

    public void AddPositionInt(UnityEngine.Vector2Int newValue) {
        var index = GameComponentsLookup.PositionInt;
        var component = (PositionIntComponent)CreateComponent(index, typeof(PositionIntComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplacePositionInt(UnityEngine.Vector2Int newValue) {
        var index = GameComponentsLookup.PositionInt;
        var component = (PositionIntComponent)CreateComponent(index, typeof(PositionIntComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemovePositionInt() {
        RemoveComponent(GameComponentsLookup.PositionInt);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherPositionInt;

    public static Entitas.IMatcher<GameEntity> PositionInt {
        get {
            if (_matcherPositionInt == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.PositionInt);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherPositionInt = matcher;
            }

            return _matcherPositionInt;
        }
    }
}
