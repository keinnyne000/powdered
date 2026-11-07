using System.Collections.Generic;
using Game.Core.Enemies;
using UnityEngine;

namespace Game.Core.Systems
{
    /// <summary>
    /// Makes drones shoot at the player. Each shot is aimed once and flies straight (no homing).
    /// </summary>
    public class DroneShootSystem : ISimulationSystem
    {
        readonly GameState _state;
        readonly DroneConfig _cfg;
        readonly Dictionary<EnemyObject, float> _cooldowns = new();

        public DroneShootSystem(GameState state, GameConfig config)
        {
            _state = state;
            _cfg = config.DroneConfig;

            if (_cfg.projectilePrefab == null)
                Debug.LogWarning("DroneShootSystem: no Projectile Prefab set on GameConfig > Drone Config, drones won't shoot");
        }

        public void Tick(float dt)
        {
            foreach (var dead in _state.DeadEnemies)
                _cooldowns.Remove(dead);

            if (_cfg.projectilePrefab == null || _state.playerTransform == null) return;
            if (!_state.Enemies.TryGetValue(EnemyType.Drone, out var drones)) return;

            Vector3 playerCenter = _state.playerTransform.position + Vector3.up;

            foreach (var drone in drones)
            {
                if (_state.DeadEnemies.Contains(drone)) continue;

                // New drones start on a full cooldown so they don't fire the instant they spawn
                if (!_cooldowns.TryGetValue(drone, out float cooldown))
                    cooldown = _cfg.fireInterval;
                cooldown -= dt;

                Vector3 pos = drone.transform.position;
                if (cooldown <= 0f && CanShoot(pos, playerCenter))
                {
                    Shoot(pos, playerCenter);
                    cooldown = _cfg.fireInterval;
                }

                _cooldowns[drone] = Mathf.Max(cooldown, 0f);
            }
        }

        /// <summary>
        /// In range, and no wall between the drone and the player
        /// </summary>
        bool CanShoot(Vector3 from, Vector3 target)
        {
            if (Vector3.Distance(from, target) > _cfg.fireRange) return false;
            return !Physics.Linecast(from, target, _cfg.obstacleMask, QueryTriggerInteraction.Ignore);
        }

        void Shoot(Vector3 from, Vector3 target)
        {
            // Aim once, at the moment of firing. The direction never changes after this.
            Vector3 direction = (target - from).normalized;

            GameObject visual = Object.Instantiate(_cfg.projectilePrefab, from, Quaternion.LookRotation(direction));

            _state.Projectiles.Add(new EnemyProjectile(
                visual, direction, _cfg.projectileSpeed, _cfg.projectileDamage,
                _cfg.projectileRadius, _cfg.projectileLifetime));
        }
    }
}