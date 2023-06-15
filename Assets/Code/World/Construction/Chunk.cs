using TopDownShooter.World.Data;
using UnityEngine;

namespace TopDownShooter.World.Construction
{
    public class Chunk : MonoBehaviour
    {
        private Mesh mesh;

        private void Awake()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        public void Build(WorldData data, int x, int y, int sizeX, int sizeY)
        {
            var mesher = new Mesher(mesh, data, x, y, sizeX, sizeY);
            mesher.BuildMesh();
        }

        private void OnDestroy()
        {
            Destroy(mesh);
        }
    }
}