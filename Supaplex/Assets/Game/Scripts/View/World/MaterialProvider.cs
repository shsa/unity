using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View.World
{
    public enum TextureType
    {
        Main,
        Normal
    }

    public static class MaterialProvider
    {
        static int blockSize;
        static RenderTexture renderTexture;
        static Texture2D[] textures;

        // count blocks for line
        static int lineCount;
        // count pixels for line
        static int linePixels;
        // for UV: blockWidth * textureKoef = blockWidth / lineSize;
        static float uvKoef;
        static float uvBlockSize;
        static int currentColumn = 0;
        static int currentRow = 0;

        public static Material Create(int blockSizeIn, int blockCount)
        {
            blockSize = blockSizeIn;
            lineCount = Mathf.CeilToInt(Mathf.Sqrt(blockCount));
            var t = lineCount * blockSize;
            var b = Mathf.CeilToInt(Mathf.Log(t, 2)) - 1;
            var w = 2 << b;
            linePixels = w;
            uvKoef = 1.0f / linePixels;
            uvBlockSize = blockSize * uvKoef;
            var h = w;
            var count = (int)Enum.GetValues(typeof(TextureType)).Cast<TextureType>().Max() + 1;
            textures = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                textures[i] = new Texture2D(w, h, TextureFormat.ARGB32, false);
            }
            renderTexture = new RenderTexture(blockSize, blockSize, 0);

            // https://docs.unity3d.com/ScriptReference/Material.SetTexture.html
            var material = new Material(Shader.Find("Standard"));
            //Make sure to enable the Keywords
            material.EnableKeyword("_NORMALMAP");
            //material.EnableKeyword("_METALLICGLOSSMAP");

            //Set the Texture you assign in the Inspector as the main texture (Or Albedo)
            material.SetTexture("_MainTex", textures[(int)TextureType.Main]);
            //Set the Normal map using the Texture you assign in the Inspector
            material.SetTexture("_BumpMap", textures[(int)TextureType.Normal]);
            return material;
        }

        static Texture2D AjustTexture(Texture2D source)
        {
            if (source.width != blockSize || source.height != blockSize)
            {
                var newTexture = new Texture2D(blockSize, blockSize, TextureFormat.ARGB32, false);
                Graphics.ConvertTexture(source, newTexture);
                UnityEngine.Object.Destroy(source);
                return newTexture;
            }
            return source;
        }

        public static int AllocateBlock()
        {
            var index = currentRow * lineCount + currentColumn;
            if (++currentColumn >= lineCount)
            {
                currentColumn = 0;
                currentRow++;
            }
            return index;
        }

        public static Rect Replace(int index, Texture2D source, TextureType textureType)
        {
            var dst = textures[(int)textureType];
            var x = index % lineCount * blockSize;
            var y = index / lineCount * blockSize;

            source = AjustTexture(source);

            //Graphics.CopyTexture(source, renderTexture);
            //var t = RenderTexture.active;
            //RenderTexture.active = renderTexture;
            //dst.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), x, y);
            //dst.Apply();
            //RenderTexture.active = t;
            
            Graphics.CopyTexture(source, 0, 0, 0, 0, blockSize, blockSize, dst, 0, 0, x, y);

            UnityEngine.Object.Destroy(source);
            return new Rect(x * uvKoef, y * uvKoef, uvBlockSize, uvBlockSize);
        }
    }
}
