using UnityEngine;

namespace Game.Core.Systems
{
    /// <summary>
    /// Handles spawning, destroying, and updating enemy visuals
    /// May become an IPresentationSystem in the future
    /// </summary>
    public class EnemyVisualsSystem : ISimulationSystem
    {
        readonly GameState _state;
        readonly GameConfig _config;

        public EnemyVisualsSystem(GameState state, GameConfig config)
        {
            _state = state;
            _config = config;
            
            _state.EnemyPrefabLookup = new();
            foreach (var lookup in config.EnemyVisualLookups)
                _state.EnemyPrefabLookup.Add(lookup.type, lookup.prefab);
        }
        public void Tick(float deltaTime)
        {
            foreach (var eo in _state.NewEnemies)
            {
                var visuals = Object.Instantiate(_state.EnemyPrefabLookup[eo.type], eo.transform);
                visuals.transform.localPosition = Vector3.zero;
                visuals.name = eo.type + "Visuals";
                _state.EnemyVisuals.Add(eo, visuals);
            }

            foreach (var eo in _state.DeadEnemies)
            {
                var deadVisuals = _state.EnemyVisuals[eo];
                _state.EnemyVisuals.Remove(eo);
                Object.Destroy(deadVisuals.gameObject);
            }
        }
    }
}