using Game.Core.Enemies;
using UnityEngine;

namespace Game.Core.Systems
{
    public class EnemySpawnSystem : ISimulationSystem
    {
        private GameState _state;
        private GameConfig _config;
        
        public EnemySpawnSystem(GameState state, GameConfig config)
        {
            _state = state;
            _config = config;
            
            EnemySpawner.OnAdded += OnNewSpawner;
            EnemySpawner.OnRemoved += OnSpawnerDestroyed;
        }
        
        public void Tick(float deltaTime)
        {
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