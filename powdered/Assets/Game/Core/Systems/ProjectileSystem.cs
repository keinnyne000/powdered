using UnityEngine;

namespace Game.Core.Systems
{
    /// <summary>
    /// Moves enemy projectiles in a straight line, damages the player on hit,
    /// and removes projectiles that hit something or run out of time
    /// </summary>
    public class ProjectileSystem : ISimulationSystem
    {
        readonly GameState _state;
        readonly DroneConfig _cfg;

        public ProjectileSystem(GameState state, GameConfig config)
        {
            _state = state;
            _cfg = config.DroneConfig;
        }

        public void Tick(float dt)
        {
            // Loop backwards so projectiles can be removed while looping
            for (int i = _state.Projectiles.Count - 1; i >= 0; i--)
            {
                var p = _state.Projectiles[i];
                p.TimeLeft -= dt;

                if (p.Visual == null || p.TimeLeft <= 0f)
                {
                    Remove(i);
                    continue;
                }

                Vector3 pos = p.Visual.transform.position;
                float step = p.Speed * dt;

                // Sweep a sphere along this frame's movement so fast projectiles can't skip through things
                if (Physics.SphereCast(pos, p.Radius, p.Direction, out RaycastHit hit, step,
                        _cfg.projectileHitMask, QueryTriggerInteraction.Ignore))
                {
                    if (_state.playerTransform != null && hit.collider.transform.IsChildOf(_state.playerTransform))
                        DamagePlayer(p.Damage);

                    Remove(i);
                    continue;
                }

                // Straight line, same direction every frame
                p.Visual.transform.position = pos + p.Direction * step;
            }
        }

        void DamagePlayer(float amount)
        {
            _state.TimeSinceLastHit = 0f;
            float before = _state.PlayerHealth.Value;
            float after = Mathf.Max(0f, before - amount);
            _state.PlayerHealth.Value = after;

            Debug.Log($"Player hit for {amount}, health: {after}");
            if (before > 0f && after <= 0f)
                Debug.Log("Player died");   // nothing handles death yet
        }

        void Remove(int index)
        {
            var p = _state.Projectiles[index];
            if (p.Visual != null)
                Object.Destroy(p.Visual);
            _state.Projectiles.RemoveAt(index);
        }
    }
}