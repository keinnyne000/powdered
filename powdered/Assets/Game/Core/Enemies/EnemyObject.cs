using System;
using UnityEngine;

namespace Game.Core.Enemies
{
    /// <summary>
    /// In-scene representation of an enemy
    /// NOT responsible for its own life cycle.
    /// </summary>
    public abstract class EnemyObject : MonoBehaviour, IDamageable
    {
        public event Action<EnemyObject> OnDeath;
        public bool isDead = false;
        public float currentHealth = -1;

        public virtual void TakeDamage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                isDead = true;
                OnDeath?.Invoke(this);
            }
        }
    }
}