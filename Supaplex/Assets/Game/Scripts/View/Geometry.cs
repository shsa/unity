using UnityEngine;
using Game.Logic;

namespace Game.View
{
    public static class Geometry
    {
        public static Mesh CreateCubeSide(Rect uvRect)
        {
            var mesh = new Mesh();

            mesh.vertices = new Vector3[] {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f)
            };

            mesh.triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3
            };

            mesh.uv = new Vector2[]
            {
                new Vector2(uvRect.xMin, uvRect.yMin),
                new Vector2(uvRect.xMin, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMin)
            };

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();

            return mesh;
        }

        static Vector3[] CubeFrontSideVertexes(Quaternion rotation)
        {
            return new Vector3[] {
                rotation * new Vector3(-0.5f, -0.5f, -0.5f),
                rotation * new Vector3(-0.5f, 0.5f, -0.5f),
                rotation * new Vector3(0.5f, 0.5f, -0.5f),
                rotation * new Vector3(0.5f, -0.5f, -0.5f)
            };
        }

        static Mesh CreateCubeFrontSideMesh(Quaternion rotation, Rect uvRect)
        {
            var mesh = new Mesh();

            mesh.vertices = CubeFrontSideVertexes(rotation);

            mesh.triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3
            };

            mesh.uv = new Vector2[]
            {
                new Vector2(uvRect.xMin, uvRect.yMin),
                new Vector2(uvRect.xMin, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMin)
            };

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();

            return mesh;
        }

        public static Mesh[] CreateCube(Rect[] uvSides)
        {
            var mm = new Mesh[6];
            mm[(int)Facing.South] = CreateCubeFrontSideMesh(Quaternion.identity, uvSides[(int)Facing.South]);
            mm[(int)Facing.North] = CreateCubeFrontSideMesh(Quaternion.Euler(0, 180, 0), uvSides[(int)Facing.North]);
            mm[(int)Facing.West] = CreateCubeFrontSideMesh(Quaternion.Euler(0, 90, 0), uvSides[(int)Facing.West]);
            mm[(int)Facing.East] = CreateCubeFrontSideMesh(Quaternion.Euler(0, -90, 0), uvSides[(int)Facing.East]);
            mm[(int)Facing.Up] = CreateCubeFrontSideMesh(Quaternion.Euler(90, 0, 0), uvSides[(int)Facing.Up]);
            mm[(int)Facing.Down] = CreateCubeFrontSideMesh(Quaternion.Euler(-90, 0, 0), uvSides[(int)Facing.Down]);
            return mm;
        }
    }
}
