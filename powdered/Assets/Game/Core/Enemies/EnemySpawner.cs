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
        /// <summary>
        /// List of currently instantiated Spawner objects, auto-managed by this class.
        /// </summary>
        public static readonly List<EnemySpawner> Objects = new();
        public static event Action<EnemySpawner> OnAdded;
        public static event Action<EnemySpawner> OnRemoved;

        private void Awake()
        {
            Objects.Add(this);
            OnAdded?.Invoke(this);
        }

        private void OnDestroy()
        {
            Objects.Remove(this);
            OnRemoved?.Invoke(this);
        }
    }
    
    /// Data class for EnemySpawners in GameState
    public class EnemySpawnerData
    {
    }
}