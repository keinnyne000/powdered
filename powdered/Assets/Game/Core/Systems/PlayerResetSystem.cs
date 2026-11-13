using UnityEngine;
 
namespace Game.Core.Systems
{
    /// <summary>
    /// PROTOTYPE: if the player falls below KillY or hits 0 health,
    /// reset their health + position and wipe every enemy in the scene.
    /// </summary>
    public class PlayerResetSystem : ISimulationSystem
    {
        const float KillY = -5f;
 
        readonly GameState _state;
        Vector3 _spawnPosition;
        bool _hasSpawnPosition;
 
        public PlayerResetSystem(GameState state)
        {
            _state = state;
        }
 
        public void Tick(float dt)
        {
            if (_state.playerTransform == null) return;
 
            // Remember where the player started on the first tick so we can put them back there
            if (!_hasSpawnPosition)
            {
                _spawnPosition = _state.playerTransform.position;
                _hasSpawnPosition = true;
            }
 
            bool fell = _state.playerTransform.position.y < KillY;
            bool dead = _state.PlayerHealth.Value <= 0f;
            if (!fell && !dead) return;
 
            ResetPlayer();
            DestroyAllEnemies();
        }
 
        void ResetPlayer()
        {
            _state.PlayerHealth.Value = GameState.PlayerMaxHealth;
 
            // CharacterController overwrites transform.position unless it's disabled while we move it
            var cc = _state.playerTransform.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            _state.playerTransform.position = _spawnPosition;
            if (cc != null) cc.enabled = true;
 
            // Kill leftover momentum so you don't respawn still falling at full speed
            _state.moveDirection = Vector3.zero;
            _state.knockbackVelocity = Vector3.zero;
            _state.jumped = false;
        }
 
        void DestroyAllEnemies()
        {
            foreach (var set in _state.Enemies.Values)
            {
                foreach (var enemy in set)
                {
                    if (enemy == null) continue;
 
                    if (_state.EnemyVisuals.TryGetValue(enemy, out var visual) && visual != null)
                        Object.Destroy(visual);
 
                    Object.Destroy(enemy.gameObject);
                }
                set.Clear();
            }
 
            _state.EnemyVisuals.Clear();
            _state.NewEnemies.Clear();     // so the visuals system doesn't try to bind to destroyed enemies
            _state.PlayerContacts.Clear();
 
            // TODO: projectiles still in flight survive the reset. Swap in whatever field
            // EnemyProjectile stores its GameObject in, then uncomment:
            // foreach (var p in _state.Projectiles) Object.Destroy(p.Visual);
            // _state.Projectiles.Clear();
 
            // Give the player a full spawn interval before the next enemy shows up
            _state.TimeOfLastSpawn = Time.time;
        }
    }
}
