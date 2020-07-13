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
        static int blockBorder = 1; // for skip seam
        static int blockSize;
        static Texture2D blockBorderTempTexture;
        static Texture2D blockTempTexture;
        static Texture2D[] textures;

        // count blocks for line
        static int lineCount;
        // count pixels for line
        static int linePixels;
        // for UV: blockWidth * textureKoef = blockWidth / lineSize;
        static float uvKoef;
        static float uvBlockSize;
        static int currentIndex = 0;
        public static Material material { get; private set; }

        public static Texture2D CreateTexture2D(int width, int height)
        {
            return new Texture2D(width, height, TextureFormat.ARGB32, false);
        }

        public static void Create(int blockSizeIn, int blockCount)
        {
            blockSize = blockSizeIn + blockBorder;
            lineCount = Mathf.CeilToInt(Mathf.Sqrt(blockCount));
            currentIndex = 0;
            var t = lineCount * blockSize;
            var b = Mathf.CeilToInt(Mathf.Log(t, 2)) - 1;
            var w = 2 << b;
            linePixels = w;
            uvKoef = 1.0f / linePixels;
            uvBlockSize = (blockSize - 2 * blockBorder) * uvKoef;
            var h = w;
            var count = (int)Enum.GetValues(typeof(TextureType)).Cast<TextureType>().Max() + 1;
            textures = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                textures[i] = CreateTexture2D(w, h);
            }
            blockBorderTempTexture = CreateTexture2D(blockSize, blockSize);
            blockTempTexture = CreateTexture2D(blockSize - 2 * blockBorder, blockSize - 2 * blockBorder);

            // https://docs.unity3d.com/ScriptReference/Material.SetTexture.html
            material = new Material(Shader.Find("Standard"));
            //Make sure to enable the Keywords
            material.EnableKeyword("_NORMALMAP");
            //material.EnableKeyword("_METALLICGLOSSMAP");

            //Set the Texture you assign in the Inspector as the main texture (Or Albedo)
            material.SetTexture("_MainTex", textures[(int)TextureType.Main]);
            //Set the Normal map using the Texture you assign in the Inspector
            material.SetTexture("_BumpMap", textures[(int)TextureType.Normal]);
            material.SetFloat("_Metallic", 0);
            material.SetFloat("_Glossiness", 0);
        }

        public static Texture2D GetBlockTexture()
        {
            return blockTempTexture;
        }

        static void AjustTexture(Texture2D source)
        {
            Graphics.ConvertTexture(source, blockBorderTempTexture);
            Graphics.ConvertTexture(source, blockTempTexture);
        }

        public static int AllocateBlock()
        {
            return currentIndex++;
        }

        public static Rect Replace(int index, Texture2D source, TextureType textureType)
        {
            var _active = RenderTexture.active;
            var dst = textures[(int)textureType];
            var x = (index % lineCount) * blockSize;
            var y = (index / lineCount) * blockSize;
            if (y > dst.height)
            {
                Debug.LogError("oops");
            }

            AjustTexture(source);

            //Graphics.CopyTexture(source, renderTexture);
            //var t = RenderTexture.active;
            //RenderTexture.active = renderTexture;
            //dst.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), x, y);
            //dst.Apply();
            //RenderTexture.active = t;
            
            Graphics.CopyTexture(blockBorderTempTexture, 0, 0, 0, 0, blockBorderTempTexture.width, blockBorderTempTexture.height, dst, 0, 0, x, y);
            Graphics.CopyTexture(blockTempTexture, 0, 0, 0, 0, blockTempTexture.width, blockTempTexture.height, dst, 0, 0, x + blockBorder, y + blockBorder);

            RenderTexture.active = _active;
            return new Rect((x + blockBorder) * uvKoef, (y + blockBorder) * uvKoef, uvBlockSize, uvBlockSize);
        }
    }
}
