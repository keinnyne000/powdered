using UnityEngine;

namespace Game.Core.Guns
{
    /// <summary>
    /// Instant hit gun. Raycasts from the camera and applies damage and knockback to what it hits.
    /// </summary>
    [CreateAssetMenu(menuName = "Guns/Hitscan Gun", fileName = "newHitscanGun")]
    public class HitscanGunBehavior : GunBehavior
    {
        [Header("Hitscan")]
        [SerializeField, Min(0f)] private float range = 100f;
        [Tooltip("Random spread in degrees. 0 is perfectly accurate")]
        [SerializeField, Range(0f, 15f)] private float spreadDegrees;
        [SerializeField] private LayerMask hitMask = ~0;

        public float MaxRange => range;
        public float SpreadDegrees => spreadDegrees;

        protected override void OnFire(GunFireContext context, GameState state)
        {
            Vector3 direction = ApplySpread(context.Direction);

            if (!Physics.Raycast(context.Origin, direction, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawRay(context.Origin, direction * range, Color.yellow, 0.5f);
                return;
            }

            Debug.DrawLine(context.Origin, hit.point, Color.red, 0.5f);

            if (hit.rigidbody != null && KnockbackForce > 0f)
                hit.rigidbody.AddForceAtPosition(direction * KnockbackForce, hit.point, ForceMode.Impulse);

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(Damage);
        }

        private Vector3 ApplySpread(Vector3 direction)
        {
            if (spreadDegrees <= 0f)
                return direction;

            Vector3 offset = Random.insideUnitSphere * Mathf.Tan(spreadDegrees * Mathf.Deg2Rad);
            return (direction + offset).normalized;
        }
    }
}
