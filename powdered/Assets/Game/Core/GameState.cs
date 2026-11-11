using System.Collections.Generic;
using Game.Core.Enemies;
using Game.Core.Guns;
using UnityEngine;

namespace Game.Core
{
    public class GameState
    {
        // ! If a type needs to be accessible by the UI, 
        //   make sure to wrap it with Observable.cs

        // PLAYER VARIABLES
        public Transform playerTransform;
        public const float PlayerMaxHealth = 100f;
        public readonly Observable<float> PlayerHealth = new(PlayerMaxHealth);

        /// <summary>
        /// seconds since the player last took damage. reset by ProjectileSystem, read by PlayerHealthSystem
        /// </summary>
        public float TimeSinceLastHit;


        //Camera
        public float sensX;
        public float sensY;
        public float xRotation;
        public float yRotation;

        //Movement
        public Vector2 playerSpeed;
        public float maxWalkSpeed;
        public float playerAcceleration;
        public float playerFriction;
        public float playerJumpForce;
        public float playerGravity;
        public Vector3 moveDirection;
        public bool jumped;
        public bool isGrounded;

        //Knockback (recoil, explosions, etc)
        public Vector3 knockbackVelocity;
        public float knockbackDrag;

        //GUN VARIABLES
        public float GunCooldown = 0f;
        public GunBehavior CurrentGun;
        public readonly List<Modifier> Modifiers = new();
        // Hits in a row without missing. Reset on a miss. Used by Combo Power
        public int ComboHits;

        //GUN PARTS (upgrades)
        public int EnemiesKilled;
        public readonly List<GunPart> GunParts = new();
        public readonly Observable<int> GunPartCount = new(0);

        
        // LEVELS
        public GameObject levelParent;

        #region Enemies
        /// <summary>
        /// Key: enemy spawner object. Value: associated spawner data.
        /// Used by EnemySpawnSystem to spawn enemies + know when a spawner data goes out of scope
        /// </summary>
        public readonly Dictionary<EnemySpawner, EnemySpawnerData> EnemySpawners = new();
        public float TimeOfLastSpawn = float.NegativeInfinity;

        /// <summary>
        /// A list of every enemy in the scene, sorted by enemy type
        /// </summary>
        public readonly Dictionary<EnemyType, HashSet<EnemyObject>> Enemies = new();
        
        /// <summary>
        /// A list of enemies that were spawned in THIS TICK, cleared after each tick by EnemySpawnSystem
        /// </summary>
        public readonly HashSet<EnemyObject> NewEnemies = new();
        
        /// <summary>
        /// A list of enemies that died THIS TICK, cleared after each tick by EnemyReapSystem
        /// </summary>
        public readonly HashSet<EnemyObject> DeadEnemies = new();

        /// <summary>
        /// Get the visuals for a given enemy type.
        /// Initialized by EnemyVisualsSystem's constructor
        /// </summary>
        public Dictionary<EnemyType, GameObject> EnemyPrefabLookup;

        public readonly Dictionary<EnemyObject, GameObject> EnemyVisuals = new();

        /// <summary>
        /// enemies touching the player THIS TICK, cleared at the statr if each new tick by DroneMovementSystem
        /// a future damage system reads this
        /// </summary>
        public readonly HashSet<EnemyObject> PlayerContacts = new();
        
        /// <summary>
        /// enemy projectiles currently in flight moved and removed by projectile system
        /// </summary>
        public readonly List<EnemyProjectile> Projectiles = new();
        #endregion
    }
}

