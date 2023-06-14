using System;
using System.Collections.Generic;
using TopDownShooter.World.Data;
using Unity.AI.Navigation;
using UnityEngine;

namespace TopDownShooter.World.Construction
{
    public class WorldBuilder : MonoBehaviour
    {
        [Header("World")]
        [SerializeReference] private Material _mapMaterial;
        [SerializeField] private SingleLayer _mapLayer;

        [Header("Enemies")]
        [SerializeReference] private GameObject _followerEnemyPrefab;
        [SerializeReference] private GameObject _orbiterEnemyPrefab;

        [Header("Player")]
        [SerializeReference] private GameObject _playerCharacterPrefab;
        [SerializeReference] private GameObject _playerCameraPrefab;

        private const int ChunkSize = 16;

        private Vector3 InMiddleOfTile(Tile tile)
        {
            return new Vector3(0.5f + tile.X, 0, 0.5f + tile.Y);
        }

        public void ReloadLevel(WorldData data)
        {
            CleanUp();
            LoadLevel(data);
        }

        private void CleanUp()
        {
            CleanUpDynamicObjects();
            CleanUpStaticObjects();
        }

        private void CleanUpStaticObjects()
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        private void CleanUpDynamicObjects()
        {
            foreach (var impersistentObject in FindObjectsOfType<ImpersistentObject>())
            {
                impersistentObject.DestroyOnLevelChange();
            }
        }

        private void LoadLevel(WorldData data)
        {
            BuildStaticObjects(data);
            BuildDynamicObjects(data);
            SpawnPlayer(data);
        }

        private void BuildStaticObjects(WorldData data)
        {
            GenerateChunks(data);
            GetComponent<NavMeshSurface>().BuildNavMesh();
        }

        private void BuildDynamicObjects(WorldData data)
        {
            foreach (var tile in data.Tiles)
            {
                foreach (var modifier in tile.Modifiers)
                {
                    if (modifier is EnemySpawnModifier esm)
                    {
                        var prefabToInstantiate = EnemyTypeToPrefab(esm.EnemyType);
                        for (int i = 0; i < esm.Count; i++)
                            Instantiate(prefabToInstantiate, InMiddleOfTile(tile), Quaternion.identity);
                    }
                }
            }
        }

        private GameObject EnemyTypeToPrefab(EnemyType type)
        {
            switch (type)
            {
                default:
                case (EnemyType.Follower): return _followerEnemyPrefab;
                case (EnemyType.Orbiter): return _orbiterEnemyPrefab;
            }
        }

        private void SpawnPlayer(WorldData data)
        {
            var spawns = new List<Tile>();
            foreach (var tile in data.Tiles)
            {
                foreach (var modifier in tile.Modifiers)
                {
                    if (modifier is PlayerSpawnModifier && !spawns.Contains(tile))
                    {
                        spawns.Add(tile);
                    }
                }
            }

            if (spawns.Count < 1)
                throw new InvalidOperationException("No player spawns on map!");

            var index = UnityEngine.Random.Range(0, spawns.Count);

            var spawn = spawns[index];
            Instantiate(_playerCharacterPrefab, InMiddleOfTile(spawn), Quaternion.identity);
            Instantiate(_playerCameraPrefab, InMiddleOfTile(spawn), Quaternion.identity);
        }

        private void GenerateChunks(WorldData map)
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
                    @object.GetComponent<MeshRenderer>().sharedMaterial = _mapMaterial;
                }
            }
        }
    }
}