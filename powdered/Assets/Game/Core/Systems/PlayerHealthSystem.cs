using UnityEngine;

namespace Game.Core.Systems
{
    /// <summary>
    /// Regenerates the player's health after they haven't been hit for a few seconds
    /// </summary>
    public class PlayerHealthSystem : ISimulationSystem
    {
        readonly GameState _state;
        readonly GameConfig _config;

        public PlayerHealthSystem(GameState state, GameConfig config)
        {
            _state = state;
            _config = config;
        }

        public void Tick(float dt)
        {
            _state.TimeSinceLastHit += dt;

            float health = _state.PlayerHealth.Value;

            
            if (health >= GameState.PlayerMaxHealth) return;
            if (_state.TimeSinceLastHit < _config.HealthRegenDelay) return;

            _state.PlayerHealth.Value = Mathf.Min(
                GameState.PlayerMaxHealth, health + _config.HealthRegenPerSecond * dt);
        }
    }
}