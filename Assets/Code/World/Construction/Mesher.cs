using System.Collections.Generic;
using System.Linq;
using TopDownShooter.World.Data;
using UnityEngine;

namespace TopDownShooter.World.Construction
{
    public class Mesher
    {
        private List<Vector3> vertices;
        private List<int> indices;
        private List<Vector2> uvs;

        private Mesh _mesh;
        private WorldData _data;

        private int _minX;
        private int _minY;
        private int _maxX;
        private int _maxY;

        public Mesher(Mesh target, WorldData data, int offsetX, int offsetY, int sizeX, int sizeY)
        {
            _mesh = target;
            _data = data;
            _minX = offsetX;
            _minY = offsetY;
            _maxX = offsetX + sizeX;
            _maxY = offsetY + sizeY;
        }

        public void BuildMesh()
        {
            vertices = new List<Vector3>();
            indices = new List<int>();
            uvs = new List<Vector2>();
            for (int x = _minX; x <= _maxX; x++)
            {
                for (int y = _minY; y <= _maxY; y++)
                {
                    BuildTile(x, y);
                }
            }

            _mesh.SetVertices(vertices);
            _mesh.SetUVs(0, uvs);
            _mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
            _mesh.UploadMeshData(false);
        }

        private void BuildTile(int x, int y)
        {
            var currentTile = GetTile(x, y);

            switch (currentTile)
            {
                case TileType.Wall:
                    BuildWall(x, y);
                    break;

                case TileType.Floor:
                    BuildFloor(x, y);
                    break;

                default:
                    return;
            }
        }

        private TileType GetTile(int x, int y)
        {
            if (x < 0 || x >= _data.Tiles.GetLength(0))
                return TileType.Empty;
            if (y < 0 || y >= _data.Tiles.GetLength(1))
                return TileType.Empty;

            return _data.Tiles[x, y].TileType;
        }

        private void BuildWall(int x, int y)
        {
            bool connectsUp = GetTile(x, y + 1) == TileType.Wall;
            bool connectsDown = GetTile(x, y - 1) == TileType.Wall;
            bool connectsLeft = GetTile(x - 1, y) == TileType.Wall;
            bool connectsRight = GetTile(x + 1, y) == TileType.Wall;

            var a = new Vector3(x, 0, y);
            var b = new Vector3(x + 1, 0, y);
            var c = new Vector3(x + 1, 0, y + 1);
            var d = new Vector3(x, 0, y + 1);
            var e = new Vector3(x, 1, y);
            var f = new Vector3(x + 1, 1, y);
            var g = new Vector3(x + 1, 1, y + 1);
            var h = new Vector3(x, 1, y + 1);
            //var aUv = new Vector2(0, 0);
            //var bUv = new Vector2(1, 0);
            var cUv = new Vector2(1, 1);
            //var dUv = new Vector2(0, 1);

            // Z +
            //BuildFace(e, f, g, h, aUv, bUv, cUv, dUv);
            BuildWallTop(x, y);
            // Y -
            if (GetTile(x, y - 1) != TileType.Wall)
                BuildFace(b, f, e, a, cUv, cUv, cUv, cUv);
            // Y +
            if (GetTile(x, y + 1) != TileType.Wall)
                BuildFace(d, h, g, c, cUv, cUv, cUv, cUv);
            // X -
            if (GetTile(x - 1, y) != TileType.Wall)
                BuildFace(a, e, h, d, cUv, cUv, cUv, cUv);
            // X +
            if (GetTile(x + 1, y) != TileType.Wall)
                BuildFace(c, g, f, b, cUv, cUv, cUv, cUv);
        }

        private void BuildWallTop(int x, int y)
        {
            bool up = GetTile(x, y + 1) == TileType.Wall;
            bool down = GetTile(x, y - 1) == TileType.Wall;
            bool left = GetTile(x - 1, y) == TileType.Wall;
            bool right = GetTile(x + 1, y) == TileType.Wall;

            var a = new Vector3(x, 1, y);
            var b = new Vector3(x + 1, 1, y);
            var c = new Vector3(x + 1, 1, y + 1);
            var d = new Vector3(x, 1, y + 1);
            var uvs = GetUvsFromConnection(up, down, left, right);
            BuildFace(a, b, c, d, uvs[0], uvs[1], uvs[2], uvs[3]);
        }

        private void BuildFloor(int x, int y)
        {
            bool up = GetTile(x, y + 1) == TileType.Floor;
            bool down = GetTile(x, y - 1) == TileType.Floor;
            bool left = GetTile(x - 1, y) == TileType.Floor;
            bool right = GetTile(x + 1, y) == TileType.Floor;

            var a = new Vector3(x, 0, y);
            var b = new Vector3(x + 1, 0, y);
            var c = new Vector3(x + 1, 0, y + 1);
            var d = new Vector3(x, 0, y + 1);
            var uvs = GetUvsFromConnection(up, down, left, right);
            BuildFace(a, b, c, d, uvs[0], uvs[1], uvs[2], uvs[3]);
        }

        private void BuildFace(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 aUv, Vector2 bUv, Vector2 cUv, Vector2 dUv)
        {
            var index = vertices.Count;
            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);
            vertices.Add(d);
            uvs.Add(aUv);
            uvs.Add(bUv);
            uvs.Add(cUv);
            uvs.Add(dUv);
            indices.Add(index + 2);
            indices.Add(index + 1);
            indices.Add(index);
            indices.Add(index);
            indices.Add(index + 3);
            indices.Add(index + 2);
        }

        private Vector2[] GetUvsFromConnection(bool up, bool down, bool left, bool right)
        {
            // all filled
            if (up && down && left && right)
                return TextureFromIdAndRotation(5, 0);

            // one missing
            if (up && !down && left && right)
                return TextureFromIdAndRotation(4, 2);
            if (up && down && !left && right)
                return TextureFromIdAndRotation(4, 3);
            if (up && down && left && !right)
                return TextureFromIdAndRotation(4, 1);
            if (!up && down && left && right)
                return TextureFromIdAndRotation(4, 0);

            // two opposite missing
            if (up && down && !left && !right)
                return TextureFromIdAndRotation(3, 1);
            if (!up && !down && left && right)
                return TextureFromIdAndRotation(3, 0);

            // concave corner
            if(up && !down && left && !right)
                return TextureFromIdAndRotation(2, 2);
            if (up && !down && !left && right)
                return TextureFromIdAndRotation(2, 3);
            if(!up && down && left && !right)
                return TextureFromIdAndRotation(2, 1);
            if(!up && down && !left && right)
                return TextureFromIdAndRotation(2, 0);

            // only one
            if (up && !down && !left && !right)
                return TextureFromIdAndRotation(1, 2);
            if (!up && down && !left && !right)
                return TextureFromIdAndRotation(1, 0);
            if (!up && !down && left && !right)
                return TextureFromIdAndRotation(1, 1);
            if (!up && !down && !left && right)
                return TextureFromIdAndRotation(1, 3);

            // none filled
            if (!up && !down && !left && !right)
                return TextureFromIdAndRotation(0, 0);

            return TextureFromIdAndRotation(0, 0);
        }

        private Vector2[] TextureFromIdAndRotation(int id, int timesRotated)
        {
            var pixelSize = 1f / 128f;
            var textureSize = 16f;
            var multiplier = pixelSize * textureSize;
            var x = id % 8;
            var y = 7-((id-x) / 8);

            var arr = new Vector2[] {
                new Vector2(multiplier*x, multiplier*y),
                new Vector2(multiplier*(x+1), multiplier*y),
                new Vector2(multiplier*(x+1), multiplier*(y+1)),
                new Vector2(multiplier*x, multiplier*(y+1)),
            };
            return Rotate(arr, timesRotated);
        }

        private static Vector2[] Rotate(Vector2[] ar, int k)
        {
            if (k <= 0 || k > ar.Length - 1)
                return ar;
            return ar.Skip(k)
                     .Concat(ar.Take(k))
                     .ToArray();
        }
    }
}