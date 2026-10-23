using Game.Core.Systems;
using UnityEngine;

namespace Game.Core.Guns
{
    /// <summary>
    /// Base class for every gun. Each gun type (hitscan, projectile, etc) extends this
    /// and is created as an asset, so stats can be tweaked in the inspector.
    /// </summary>
    public abstract class GunBehavior : ScriptableObject
    {
        [Header("Firing")]
        [Tooltip("Shots per second")]
        [SerializeField, Min(0.01f)] private float fireRate = 4f;
        [Tooltip("Hold to keep firing. Off means one shot per click")]
        [SerializeField] private bool automatic;

        [Header("Effects")]
        [SerializeField, Min(0f)] private float damage = 10f;
        [Tooltip("Force applied to whatever gets hit")]
        [SerializeField, Min(0f)] private float knockbackForce = 5f;
        [Tooltip("Pushes the shooter backwards when firing")]
        [SerializeField, Min(0f)] private float recoilForce;

        public float FireRate => fireRate;
        public bool Automatic => automatic;
        public float Damage => damage;
        public float KnockbackForce => knockbackForce;
        public float RecoilForce => recoilForce;
        public float SecondsBetweenShots => 1f / fireRate;

        /// <summary>
        /// Fires the gun once. Applies recoil to the shooter, then runs the gun specific behavior.
        /// </summary>
        public void Fire(ShootGameAction context, GameState state)
        {
            if (recoilForce > 0f)
                state.knockbackVelocity -= context.Direction * recoilForce;

            state.GunCooldown = state.CurrentGun.SecondsBetweenShots;
            OnFire(context, state);
        }

        /// <summary>
        /// What actually happens when this gun shoots (raycast, spawn a projectile, etc)
        /// </summary>
        protected abstract void OnFire(ShootGameAction context, GameState state);
    }
}
