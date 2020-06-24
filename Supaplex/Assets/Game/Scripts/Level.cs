using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class LevelObject
{
    public Vector2Int position;
    public ObjectType type;
}

public class Wall : LevelObject
{

}

public class Stone : LevelObject
{

}

public class Level : IEnumerable<LevelObject>
{
    struct LevelData
    {
        public string[] lines;
    }

    Dictionary<Vector2Int, LevelObject> data = new Dictionary<Vector2Int, LevelObject>();

    public LevelObject this[Vector2Int index] {
        get {
            return data[index];
        }
    }

    public static Level Load(string name)
    {
        var level = new Level();
        var text = Resources.Load<TextAsset>("Levels/" + name);
        var levelData = JsonUtility.FromJson<LevelData>(text.text);

        for (int j = 0; j < levelData.lines.Length; j++)
        {
            var line = levelData.lines[j];
            var y = levelData.lines.Length - j - 1;
            for (int x = 0; x < line.Length; x++)
            {
                switch (line[x])
                {
                    case '#':
                        {
                            var obj = new Wall();
                            obj.position = new Vector2Int(x, y);
                            obj.type = ObjectType.Wall;
                            level.data.Add(obj.position, obj);
                        }
                        break;
                    case 'o':
                        {
                            var obj = new Stone();
                            obj.position = new Vector2Int(x, y);
                            obj.type = ObjectType.Stone;
                            level.data.Add(obj.position, obj);
                        }
                        break;
                }
            }
        }

        return level;
    }

    IEnumerator<LevelObject> IEnumerable<LevelObject>.GetEnumerator()
    {
        return data.Values.GetEnumerator();
    }

    public IEnumerable<LevelObject> GetEnumerator(ObjectType type)
    {
        foreach (var obj in data.Values)
        {
            if (obj.type == type)
            {
                yield return obj;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return data.Values.GetEnumerator();
    }

    public bool IsEmpty(Vector2Int pos)
    {
        if (data.TryGetValue(pos, out var obj))
        {
            return obj.type == ObjectType.Empty;
        }
        return true;
    }

    public LevelObject Move(Vector2Int from, Vector2Int to)
    {
        if (data.TryGetValue(from, out var obj))
        {
            Remove(to);
            data.Add(to, obj);
            return obj;
        }
        return null;
    }

    public LevelObject Remove(Vector2Int pos)
    {
        if (data.TryGetValue(pos, out var obj))
        {
            data.Remove(pos);
            return obj;
        }
        return null;
    }
}
