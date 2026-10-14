using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Enemies
{
    /// <summary>
    /// In-scene representation of an EnemySpawner
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject SpawnPoint => spawnPoint;
        /// <summary>
        /// List of currently instantiated Spawner objects, auto-managed by this class.
        /// </summary>
        public static readonly List<EnemySpawner> Objects = new();
        public static event Action<EnemySpawner> OnNewSpawner;
        public static event Action<EnemySpawner> OnSpawnerDestroyed;
        
        [SerializeField] private GameObject spawnPoint;

        private void Awake()
        {
            Objects.Add(this);
            OnNewSpawner?.Invoke(this);
        }

        private void OnDestroy()
        {
            Objects.Remove(this);
            OnSpawnerDestroyed?.Invoke(this);
        }
    }
    
    /// Data class for EnemySpawners in GameState
    public class EnemySpawnerData
    {
    }
}