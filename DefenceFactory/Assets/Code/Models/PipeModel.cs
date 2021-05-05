using DefenceFactory.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DefenceFactory.Models
{
    public class PipeModel : Model
    {
        static DirectionSet exclude = ~(DirectionSet.NE | DirectionSet.SE | DirectionSet.SW | DirectionSet.NW);

        public string name { get; private set; }

        Texture2D texture;
        int x;
        int y;
        int size;

        class PipeItem
        {
            public string name;
            public int x;
            public int y;
            public int rotation;
            public BlockView view;
        }

        PipeItem[] pipes = new PipeItem[256];
        public PipeModel(Game.Model model)
        {
            name = $"{model.type}.{model.model}";
            var json = Resources.Load<TextAsset>($"Models/{model.type}/{model.model}");
            var config = JsonUtility.FromJson<Models.PipeConfig>(json.text);

            texture = Resources.Load<Texture2D>(config.texture);
            x = config.x;
            y = config.y;
            size = config.size;

            void newItem(DirectionSet dirs, int index, int rotation)
            {
                var s = "";
                for (var d = DirectionEnum.First; d <= DirectionEnum.Last; d++)
                {
                    if (dirs.HasDir(d))
                    {
                        s += d.ToString();
                    }
                }
                if (string.IsNullOrEmpty(s))
                {
                    s = "None";
                }
                pipes[(int)dirs] = new PipeItem
                {
                    name = this.name + "." + s,
                    x = this.x + index * size,
                    y = this.y,
                    rotation = rotation
                };
            }
            newItem(DirectionSet.None, 5, 0);

            newItem(DirectionSet.E, 0, 0);
            newItem(DirectionSet.S, 0, 270);
            newItem(DirectionSet.W, 0, 180);
            newItem(DirectionSet.N, 0, 90);

            newItem(DirectionSet.W | DirectionSet.E, 1, 0);
            newItem(DirectionSet.N | DirectionSet.S, 1, 90);

            newItem(DirectionSet.W | DirectionSet.N | DirectionSet.E, 2, 0);
            newItem(DirectionSet.N | DirectionSet.E | DirectionSet.S, 2, 270);
            newItem(DirectionSet.E | DirectionSet.S | DirectionSet.W, 2, 180);
            newItem(DirectionSet.S | DirectionSet.W | DirectionSet.N, 2, 90);

            newItem(DirectionSet.N | DirectionSet.E | DirectionSet.S | DirectionSet.W, 3, 0);

            newItem(DirectionSet.W | DirectionSet.N, 4, 0);
            newItem(DirectionSet.N | DirectionSet.E, 4, 270);
            newItem(DirectionSet.E | DirectionSet.S, 4, 180);
            newItem(DirectionSet.S | DirectionSet.W, 4, 90);
        }

        public override string GetKey(Meta meta)
        {
            var dirs = (DirectionSet)meta & exclude;
            var pipe = pipes[(int)dirs];
            return pipe.name;
        }

        public override BlockView GetPrefab(Meta meta)
        {
            var dirs = (DirectionSet)meta & exclude;
            var pipe = pipes[(int)dirs];
            if (pipe.view == default)
            {
                var go = new GameObject(pipe.name);
                go.transform.position = new Vector3(0, 0, -1000);
                var image = new GameObject();
                image.transform.SetParent(go.transform);
                image.transform.localPosition = Vector3.zero;
                image.transform.rotation = Quaternion.Euler(0, 0, pipe.rotation);
                var sr = image.AddComponent<SpriteRenderer>();
                sr.sprite = Sprite.Create(texture, new Rect(pipe.x, pipe.y, size, size), new Vector2(0.5f, 0.5f), size);
                pipe.view = go.AddComponent<BlockView>();
            }
            return pipe.view;
        }
    }
}
