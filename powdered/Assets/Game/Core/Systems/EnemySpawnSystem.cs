using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Enemies;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Game.Core.Systems
{
    /// <summary>
    /// A class for configuring settings related to the EnemySpawnSystem. Lives in GameConfig.
    /// </summary>
    [System.Serializable]
    public class EnemySpawnSystemConfig
    {
        // Time between enemy spawns. This will 100% need to be changed to something more complex
        // later down the line.
        [FormerlySerializedAs("enemySpawnTime")]
        [SerializeField] public float timeBetweenSpawns;

        [Tooltip("Max enemies alive at once. Spawning pauses until one dies")]
        [SerializeField, Min(0)] public int maxEnemies = 5;
    }
    
    public class EnemySpawnSystem : ISimulationSystem
    {
        private readonly GameState _state;
        private readonly GameConfig _config;
        
        public EnemySpawnSystem(GameState state, GameConfig config)
        {
            _state = state;
            _config = config;
            
            EnemySpawner.OnNewSpawner += OnNewSpawner;
            EnemySpawner.OnSpawnerDestroyed += OnSpawnerDestroyed;
            // Add spawners that already exist
            foreach(var spawner in EnemySpawner.Objects)
            {
                _state.EnemySpawners.Add(spawner, new EnemySpawnerData());
            }
        }
        
        public void Tick(float deltaTime)
        {
            // Clean Up the NewEnemies List
            while (_state.NewEnemies.Count > 0)
            {
                _state.NewEnemies.Pop();
            }
            
            if (Time.time - _state.TimeOfLastSpawn > _config.EnemySpawnSystemConfig.timeBetweenSpawns)
            {
                // At the cap, skip this spawn and restart the timer,
                // so a new enemy shows up one full interval after a slot frees up
                if (AliveEnemyCount() < _config.EnemySpawnSystemConfig.maxEnemies)
                {
                    // Currently, we ALWAYS spawn a drone. This should be updated later.
                    // TODO: update to spawn multiple types of enemies (Enemy Strategist)
                    Spawn(EnemyType.Drone);
                }
                _state.TimeOfLastSpawn = Time.time;
            }
        }

        int AliveEnemyCount()
        {
            int count = 0;
            foreach (var enemies in _state.Enemies.Values)
                count += enemies.Count;
            return count;
        }

        void Spawn(EnemyType enemyType)
        {
            if (_state.EnemySpawners.Keys.Count <= 0)
            {
                Debug.LogError($"Tried to spawn enemy but there are no spawners!");
                return;
            }
            
            var enemy = Object.Instantiate(_config.EnemyPrefab).GetComponent<EnemyObject>();
            enemy.name = enemyType.ToString();
            if (!_state.Enemies.ContainsKey(enemyType))
                _state.Enemies.Add(enemyType, new HashSet<EnemyObject>());
            if (!_state.Enemies[enemyType].Add(enemy))
            {
                Debug.LogError($"Tried to add {enemy} but it was already in the enemy list!");
                Object.Destroy(enemy);
                return;
            }
            // Currently, just uses the first spawner in the list (so the oldest)
            // TODO: Update this to however we want to handle spawning points (Enemy Strategist)
            var spawnPoint = _state.EnemySpawners.Keys.First().SpawnPoint.transform.position;
            enemy.transform.position = spawnPoint;
            
            // Give it the dead enemies list so it can notify when it dies
            enemy.DeadEnemies = _state.DeadEnemies;
            
            // Add this enemy to the new enemies list so other systems can do stuff to the new enemy
            // IE, spawn/bind visuals
            _state.NewEnemies.Add(enemy);
        }

        void OnNewSpawner(EnemySpawner spawner)
        {
            if (_state.EnemySpawners.ContainsKey(spawner))
                Debug.LogError($"Spawner {spawner} already exists");
            
            _state.EnemySpawners.Add(spawner, new EnemySpawnerData());
        }

        void OnSpawnerDestroyed(EnemySpawner spawner)
        {
            if (!_state.EnemySpawners.ContainsKey(spawner))
                Debug.LogError($"Tried to de-register {spawner} but Spawner does not exist!");
            
            _state.EnemySpawners.Remove(spawner);
        }
    }
}