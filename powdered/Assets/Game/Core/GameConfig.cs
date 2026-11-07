using System;
using System.Collections.Generic;
using Game.Core.Enemies;
using Game.Core.Guns;
using Game.Core.Systems;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Core
{
    /// <summary>
    /// Game config asset, stores all unity-exposed game settings
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "Game Config", fileName = "newGameConfig")]
    public class GameConfig : ScriptableObject
    {
        // Private fields, protects settings from being accidentally changed at runtime
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GunBehavior startingGun;
        [SerializeField] private LevelData levelData;

        [Header("Enemies")]
        [SerializeField] private DroneConfig droneConfig;
        [SerializeField] private EnemySpawnSystemConfig enemySpawnSystemConfig;

        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private List<EnemyVisualLookup> enemyVisualLookups;
        

        [Header("Footsteps")]
        [SerializeField] private AudioClip[] footstepClips;
        [SerializeField, Range(0f, 1f)] private float footstepVolume = 0.6f;
        [Tooltip("Distance walked between steps")]
        [SerializeField, Min(0.1f)] private float footstepDistance = 2f;


        // This pattern exposes a read only method to get each field
        public GameObject PlayerPrefab => playerPrefab;
        public GunBehavior StartingGun => startingGun;
        public LevelData LevelData => levelData;

        
        
        // Enemies
        public EnemySpawnSystemConfig EnemySpawnSystemConfig => enemySpawnSystemConfig;
        public GameObject EnemyPrefab => enemyPrefab;
        public List<EnemyVisualLookup> EnemyVisualLookups => enemyVisualLookups;

        public DroneConfig DroneConfig => droneConfig;

        public AudioClip[] FootstepClips => footstepClips;
        public float FootstepVolume => footstepVolume;
        public float StepDistance => footstepDistance;

    }

    [System.Serializable]
    public class LevelData
    {
        public GameObject prefab;
    }

    [System.Serializable]
    public class EnemyVisualLookup
    {
        public EnemyType type;
        public GameObject prefab;
    }
}