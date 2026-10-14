using UnityEngine;

namespace Game.Core.Systems
{
    public class LevelSpawnSystem : ISimulationSystem
    {
        public LevelSpawnSystem(GameState state, GameConfig config)
        {
            if (config.LevelData.prefab == null)
            {
                Debug.LogError($"Level data prefab is null");
                return;
            }
            state.levelParent = Object.Instantiate(config.LevelData.prefab);
        }
        
        public void Tick(float deltaTime) { ; }
    }
}