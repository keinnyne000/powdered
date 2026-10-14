using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Enemies
{
    public enum EnemyType
    {
        Drone,
        Creeper, // (aww man) -- just here for an example, we won't actually have this
    }
    
    
    /// <summary>
    /// In-scene representation of an enemy
    /// NOT responsible for its own life cycle.
    /// </summary>
    public sealed class EnemyObject : MonoBehaviour, IDamageable
    {
        [HideInInspector] public float currentHealth = -1;
        [HideInInspector] public EnemyType type;
        [HideInInspector] public HashSet<EnemyObject> DeadEnemies = null;
        public event Action<EnemyObject> OnTakeDamage;
        
        public void TakeDamage(float amount)
        {
            OnTakeDamage?.Invoke(this);
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                DeadEnemies.Add(this);
            }
        }
    }
}