using UnityEngine;
using Game.Logic;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using Game.Logic.World;
using Game.View.World;

namespace Game.View
{
    public static class Geometry
    {
        static BlockPart[] parts = new BlockPart[6];

        public class CubeSide
        {
            public Vector3[] vertices;
            public int[] triangles;
            public Vector2[] uv;

            public CubeSide(Vector3[] vertices, int[] triangles, Vector2[] uv)
            {
                this.vertices = vertices;
                this.triangles = triangles;
                this.uv = uv;
            }
        }

        public class Cube
        {
            public CubeSide[] sides;

            public Cube(CubeSide[] sides)
            {
                this.sides = sides;

                var v = new List<Vector3>();
                var t = new List<int>();
                var u = new List<Vector2>();
                //for (int i = 0; i < sides.Length; i++)
                //{
                //    var side = sides[i];
                //    for (int j = 0; j < side.vertices.Length; i++)
                //    {
                //        v.Add(side.vertices[j]);
                //        u.Add(side.uv[j]);
                //    }

                //    var l = t.Count;
                //    for (int j = 0; j < side.triangles.Length; j++)
                //    {
                //        t.Add(l + side.triangles[j]);
                //    }
                //}

                //vertices = v.ToArray();
                //triangles = t.ToArray();
                //uv = u.ToArray();
            }
        }

        public static Cube cube;

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

        public static CubeSide CreateCubeSide(Quaternion rotation, Rect uvRect)
        {
            var vertices = CubeFrontSideVertexes(rotation);

            var triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3
            };

            var uv = new Vector2[]
            {
                new Vector2(uvRect.xMin, uvRect.yMin),
                new Vector2(uvRect.xMin, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMin)
            };

            return new CubeSide(vertices, triangles, uv);
        }

        public static Cube CreateCube2(Rect[] uvSides)
        {
            CubeSide[] sides = new CubeSide[6];
            sides[(int)Facing.South] = CreateCubeSide(Quaternion.identity, uvSides[(int)Facing.South]);
            sides[(int)Facing.North] = CreateCubeSide(Quaternion.Euler(0, 180, 0), uvSides[(int)Facing.North]);
            sides[(int)Facing.West] = CreateCubeSide(Quaternion.Euler(0, 90, 0), uvSides[(int)Facing.West]);
            sides[(int)Facing.East] = CreateCubeSide(Quaternion.Euler(0, -90, 0), uvSides[(int)Facing.East]);
            sides[(int)Facing.Up] = CreateCubeSide(Quaternion.Euler(90, 0, 0), uvSides[(int)Facing.Up]);
            sides[(int)Facing.Down] = CreateCubeSide(Quaternion.Euler(-90, 0, 0), uvSides[(int)Facing.Down]);
            cube = new Cube(sides);
            return cube;
        }

        public static void DrawBounds(Bounds b, float delay = 0)
        {
            // bottom
            var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
            var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
            var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
            var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

            Debug.DrawLine(p1, p2, Color.blue, delay);
            Debug.DrawLine(p2, p3, Color.red, delay);
            Debug.DrawLine(p3, p4, Color.yellow, delay);
            Debug.DrawLine(p4, p1, Color.magenta, delay);

            // top
            var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
            var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
            var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
            var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

            Debug.DrawLine(p5, p6, Color.blue, delay);
            Debug.DrawLine(p6, p7, Color.red, delay);
            Debug.DrawLine(p7, p8, Color.yellow, delay);
            Debug.DrawLine(p8, p5, Color.magenta, delay);

            // sides
            Debug.DrawLine(p1, p5, Color.white, delay);
            Debug.DrawLine(p2, p6, Color.gray, delay);
            Debug.DrawLine(p3, p7, Color.green, delay);
            Debug.DrawLine(p4, p8, Color.cyan, delay);
        }

        public static Vector3[] BlockSideVertexes(Quaternion rotation)
        {
            return new Vector3[] {
                rotation * new Vector3(-0.5f, -0.5f, -0.5f),
                rotation * new Vector3(-0.5f, 0.5f, -0.5f),
                rotation * new Vector3(0.5f, 0.5f, -0.5f),
                rotation * new Vector3(0.5f, -0.5f, -0.5f)
            };
        }

        public static int[] BlockSideTriangles()
        {
            return new int[]
            {
                0, 1, 2,
                0, 2, 3
            };
        }

        public static Vector2[] BlockSideUVs(Rect uvRect)
        {
            return new Vector2[]
            {
                new Vector2(uvRect.xMin, uvRect.yMin),
                new Vector2(uvRect.xMin, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMax),
                new Vector2(uvRect.xMax, uvRect.yMin)
            };
        }

        public static BlockPart GetBlockPart(Facing facing, Rect uvRect)
        {
            var part = parts[(int)facing];
            var newPart = new BlockPart();
            newPart.vertices = new Vector3[part.vertices.Length];
            part.vertices.CopyTo(newPart.vertices, 0);

            newPart.triangles = new int[part.triangles.Length];
            part.triangles.CopyTo(newPart.triangles, 0);

            newPart.uv = BlockSideUVs(uvRect);

            return newPart;
        }

        static Geometry()
        {
            void addPart(Facing facing, Quaternion rotation)
            {
                var part = new BlockPart();
                part.vertices = BlockSideVertexes(rotation);
                part.triangles = BlockSideTriangles();
                parts[(int)facing] = part;
            }

            addPart(Facing.North, Quaternion.Euler(0, 180, 0));
            addPart(Facing.East, Quaternion.Euler(0, -90, 0));
            addPart(Facing.South, Quaternion.identity);
            addPart(Facing.West, Quaternion.Euler(0, 90, 0));
            addPart(Facing.Up, Quaternion.Euler(90, 0, 0));
            addPart(Facing.Down, Quaternion.Euler(-90, 0, 0));
        }
    }
}
