using System;
using Game.Core.Guns;
using Game.Core.Systems;
using UnityEngine;

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
        [SerializeField] private EnemySpawnSystemConfig enemySpawnSystemConfig;

        [SerializeField] private GameObject enemyPrefab;
        // this system will need to be expanded in the future to allow for multiple enemy types
        // probably create a class EnemyVisualLookup { EnemyType, VisualPrefab}
        // and init to a dictionary on start in game state
        [SerializeField] private GameObject droneVisualsPrefab;
        

        // This pattern exposes a read only method to get each field
        public GameObject PlayerPrefab => playerPrefab;
        public GunBehavior StartingGun => startingGun;
        public LevelData LevelData => levelData;
        
        
        // Enemies
        public EnemySpawnSystemConfig EnemySpawnSystemConfig => enemySpawnSystemConfig;
        public GameObject EnemyPrefab => enemyPrefab;
        public GameObject DroneVisualsPrefab => droneVisualsPrefab;
    }

    [System.Serializable]
    public class LevelData
    {
        public GameObject prefab;
    }
}