using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace TopDownShooter.WorldGeneration
{
    public class WorldManagement : MonoBehaviour
    {
        [SerializeReference] private TextAsset _mapCsv;
        [SerializeReference] private Material _material;
        [SerializeField] private LayerMask _worldMask;
        [SerializeField] private SingleLayer _mapLayer;
        public NavMeshDataInstance _navMeshDataInstance;
        public NavMeshData _navMeshData;

        private const int ChunkSize = 16;

        private void Awake()
        {
            var text = _mapCsv.text;
            var map = CsvToWorldDataConverter.ConvertFromCsv(text);
            GenerateChunks(map);

            GetComponent<NavMeshSurface>().BuildNavMesh();
        }

        void GenerateChunks(WorldData map)
        {
            var countX = 1 + map.Tiles.GetLength(0) / ChunkSize;
            var countY = 1 + map.Tiles.GetLength(1) / ChunkSize;

            for (int i = 0; i < countX; i++)
            {
                for (int j = 0; j < countY; j++)
                {
                    var types = new[] { typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider), typeof(Chunk) };
                    var @object = new GameObject($"Chunk {i}-{j}", types);
                    @object.layer = _mapLayer.LayerIndex;
                    @object.isStatic = true;
                    @object.transform.parent = transform;
                    @object.layer = gameObject.layer;
                    @object.tag = gameObject.tag;
                    @object.GetComponent<Chunk>().Build(map, i * ChunkSize, j * ChunkSize, ChunkSize - 1, ChunkSize - 1);
                    @object.GetComponent<MeshRenderer>().sharedMaterial = _material;
                }
            }
        }
    }
}