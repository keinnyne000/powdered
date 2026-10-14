using Game.Core.Enemies;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("enemySpawnTime")] [SerializeField] public float timeBetweenSpawns;
    }
    
    public class EnemySpawnSystem : ISimulationSystem
    {
        private readonly GameState _state;
        private readonly GameConfig _config;
        
        public EnemySpawnSystem(GameState state, GameConfig config)
        {
            _state = state;
            _config = config;
            
            EnemySpawner.OnAdded += OnNewSpawner;
            EnemySpawner.OnRemoved += OnSpawnerDestroyed;
        }
        
        public void Tick(float deltaTime)
        {
            if (Time.time - _state.TimeOfLastSpawn > _config.EnemySpawnSystemConfig.timeBetweenSpawns)
            {
                _state.TimeOfLastSpawn = Time.time;
            }
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