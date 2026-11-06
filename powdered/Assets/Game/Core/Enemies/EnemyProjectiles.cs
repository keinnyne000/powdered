using UnityEngine;

namespace Game.Core.Enemies
{
    /// <summary>
    /// Data for one projectile in flight. The direction is fixed when it's created (no homing).
    /// </summary>
    public sealed class EnemyProjectile
    {
        public readonly GameObject Visual;
        public readonly Vector3 Direction;
        public readonly float Speed;
        public readonly float Damage;
        public readonly float Radius;
        public float TimeLeft;

        public EnemyProjectile(GameObject visual, Vector3 direction, float speed, float damage, float radius, float lifetime)
        {
            Visual = visual;
            Direction = direction;
            Speed = speed;
            Damage = damage;
            Radius = radius;
            TimeLeft = lifetime;
        }
    }
}
