using System.Collections.Generic;
using Game.Core.Enemies;
using Game.Core.Guns.Upgrades;
using UnityEngine;

namespace Game.Core
{
    public class GameState
    {
        // ! If a type needs to be accessible by the UI, 
        //   make sure to wrap it with Observable.cs

        // PLAYER VARIABLES
        //Camera
        public float sensX;
        public float sensY;
        public float xRotation;
        public float yRotation;

        //Movement
        public Vector2 playerSpeed;
        public float maxWalkSpeed;
        public float playerVelocity;
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
        public float gunCooldown;

        //Gun Upgrades
        public List<IUpgrades> pistolUpgrades;
        public float damageUp;
        
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

        #endregion
    }
}

