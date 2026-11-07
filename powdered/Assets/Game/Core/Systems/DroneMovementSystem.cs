using System;
using System.Collections.Generic;
using Game.Core.Enemies;
using UnityEngine;

namespace Game.Core.Systems
{
    /// <summary>
    /// Drone settings, lives in GameConfig so everything is tunable in the inspector
    /// </summary>
    [Serializable]
    public class DroneConfig
    {
        [Header("Movement")]
        public float maxSpeed = 3f;
        [Tooltip("How far (flat distance) the drone hovers from the player")]
        public float followDistance = 12f;
        [Tooltip("How far above the player's center the drone hovers")]
        public float hoverHeight = 2f;
        [Tooltip("Start slowing down this far from the hover spot so the drone doesn't overshoot")]
        public float slowDownDistance = 2f;

        [Header("PID (higher kp = snappier, higher kd = less overshoot)")]
        public float kp = 8f;
        public float ki = 0f;
        public float kd = 0.5f;

        [Header("Whiskers")]
        [Tooltip("Set to the Level layer ONLY, or the drone will avoid the player and other drones")]
        public LayerMask obstacleMask;
        public float whiskerLength = 4f;
        public float whiskerRadius = 0.4f;
        [Tooltip("How far the side whiskers fan out (1 = 45 degrees)")]
        public float whiskerSpread = 1f;
        public float avoidStrength = 2f;
        [Tooltip("Extra upward push when a wall is head-on")]
        public float climbBias = 1.5f;

        [Header("Shooting")]
        public GameObject projectilePrefab;
        [Tooltip("Seconds between shots")]
        public float fireInterval = 2f;
        [Tooltip("Drones only shoot when the player is within this distance")]
        public float fireRange = 20f;
        [Tooltip("Projectile speed in units per second")]
        public float projectileSpeed = 12f;
        public float projectileDamage = 10f;
        public float projectileRadius = 0.2f;
        [Tooltip("Seconds before a projectile that hit nothing disappears")]
        public float projectileLifetime = 6f;
        [Tooltip("What projectiles hit: tick Default (the player) and Level")]
        public LayerMask projectileHitMask = ~0;

        [Header("Contact")]
        public float contactDistance = 2f;
    }

    /// <summary>
    /// Flies drones to a hover spot around the player. Whiskers steer around walls and a PID
    /// smooths the movement.
    /// </summary>
    public class DroneMovementSystem : ISimulationSystem
    {
        class DroneData
        {
            public Vector3 velocity;
            public PidController pid;
            public bool touching;
        }

        readonly GameState _state;
        readonly DroneConfig _cfg;
        readonly Dictionary<EnemyObject, DroneData> _drones = new();
        readonly Vector3[] _whiskers = new Vector3[5];

        public DroneMovementSystem(GameState state, GameConfig config)
        {
            _state = state;
            _cfg = config.DroneConfig;
        }

        public void Tick(float dt)
        {
            _state.PlayerContacts.Clear();

            foreach (var dead in _state.DeadEnemies)
                _drones.Remove(dead);

            if (_state.playerTransform == null) return;
            if (!_state.Enemies.TryGetValue(EnemyType.Drone, out var drones)) return;

            // The player's transform is at their feet, their body center is about 1 unit up
            Vector3 playerCenter = _state.playerTransform.position + Vector3.up;

            foreach (var drone in drones)
            {
                if (_state.DeadEnemies.Contains(drone)) continue;

                if (!_drones.TryGetValue(drone, out var data))
                {
                    data = new DroneData { pid = new PidController(_cfg.kp, _cfg.ki, _cfg.kd) };
                    _drones[drone] = data;
                }

                Move(drone, data, playerCenter, dt);
                CheckContact(drone, data, playerCenter);
            }
        }

        void Move(EnemyObject drone, DroneData data, Vector3 playerCenter, float dt)
        {
            Vector3 pos = drone.transform.position;

            // Hover on a ring around the player at followDistance, hoverHeight above their center.
            // Only the flat (XZ) direction is used, so the hover height stays steady and the
            // drone doesn't creep upward.
            Vector3 flat = pos - playerCenter;
            flat.y = 0f;
            Vector3 awayDir = flat.sqrMagnitude > 0.001f ? flat.normalized : Vector3.forward;
            Vector3 target = playerCenter + awayDir * _cfg.followDistance + Vector3.up * _cfg.hoverHeight;

            Vector3 toTarget = target - pos;
            float targetDist = toTarget.magnitude;

            Vector3 wantedVelocity = Vector3.zero;   // at the hover spot, brake to a stop
            if (targetDist > 0.05f)
            {
                Vector3 desired = toTarget / targetDist;

                // Fastest path: if nothing is in the way, fly straight. Otherwise let the whiskers steer.
                bool clear = !Physics.SphereCast(pos, _cfg.whiskerRadius, desired, out _,
                    targetDist, _cfg.obstacleMask, QueryTriggerInteraction.Ignore);
                Vector3 dir = clear ? desired : Steer(pos, desired);

                // Ease off near the hover spot so the PID doesn't overshoot
                float arrive = Mathf.Clamp01(targetDist / _cfg.slowDownDistance);
                wantedVelocity = dir * _cfg.maxSpeed * arrive;
            }

            // PID: error is (velocity we want) - (velocity we have), output is acceleration
            Vector3 accel = data.pid.Update(wantedVelocity - data.velocity, dt);
            data.velocity = Vector3.ClampMagnitude(data.velocity + accel * dt, _cfg.maxSpeed);

            drone.transform.position = pos + data.velocity * dt;
        }

        /// <summary>
        /// Casts 5 whiskers around the desired direction and pushes away from whatever they hit
        /// </summary>
        Vector3 Steer(Vector3 pos, Vector3 desired)
        {
            // Build a sideways axis and an up axis around the desired direction
            Vector3 right = Vector3.Cross(Vector3.up, desired);
            if (right.sqrMagnitude < 0.001f) right = Vector3.right; // desired is straight up or down
            right.Normalize();
            Vector3 up = Vector3.Cross(desired, right);
            float k = _cfg.whiskerSpread;

            _whiskers[0] = desired;                           // center
            _whiskers[1] = (desired + right * k).normalized;  // right
            _whiskers[2] = (desired - right * k).normalized;  // left
            _whiskers[3] = (desired + up * k).normalized;     // up
            _whiskers[4] = (desired - up * k).normalized;     // down

            Vector3 result = desired;
            float headOn = 0f;

            for (int i = 0; i < _whiskers.Length; i++)
            {
                bool hitSomething = Physics.SphereCast(pos, _cfg.whiskerRadius, _whiskers[i], out var hit,
                    _cfg.whiskerLength, _cfg.obstacleMask, QueryTriggerInteraction.Ignore);

                // Visible in the Scene view while testing: green = clear, red = hit
                Debug.DrawRay(pos, _whiskers[i] * _cfg.whiskerLength, hitSomething ? Color.red : Color.green);

                if (!hitSomething) continue;

                // 0 = far end of the whisker, 1 = touching. Closer means a stronger push.
                float closeness = 1f - hit.distance / _cfg.whiskerLength;
                result += hit.normal * (closeness * _cfg.avoidStrength);

                if (i == 0) headOn = closeness;
            }

            // Wall straight ahead: the pushes can't pick a side, so go up and over
            result += Vector3.up * (headOn * _cfg.climbBias);

            return result.normalized;
        }

        // Only triggers when a drone is within contactDistance, which a ranged drone
        // hovering at followDistance won't be. Kept in case melee drones come back.
        void CheckContact(EnemyObject drone, DroneData data, Vector3 playerCenter)
        {
            bool touching = Vector3.Distance(drone.transform.position, playerCenter) <= _cfg.contactDistance + 0.25f;

            if (touching)
            {
                _state.PlayerContacts.Add(drone);
                if (!data.touching)
                    Debug.Log($"{drone.name} hit the player");
            }

            data.touching = touching;
        }
    }
}