using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Unity.Jobs;

namespace Game
{
    // https://www.raywenderlich.com/7630142-entity-component-system-for-unity-getting-started
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance;

        [Header("Spawner")]

        [SerializeField]
        public int spawnCount = 30;

        [SerializeField]
        public float spawnInterval = 3f;

        [SerializeField]
        public int difficultyBonus = 5;

        [SerializeField]
        public float spawnRadius = 20f;

        [SerializeField]
        public float minSpeed = 4f;
        [SerializeField]
        public float maxSpeed = 10f;

        [Header("Enemy")]

        [SerializeField]
        public GameObject[] enemyPrefabs = null;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
    }
}