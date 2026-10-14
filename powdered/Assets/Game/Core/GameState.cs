using System.Collections.Generic;
using Game.Core.Enemies;
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
        public float playerSpeed;
        public float playerJumpForce;
        public float playerGravity;
        public Vector3 moveDirection;
        public bool jumped;

        //Knockback (recoil, explosions, etc)
        public Vector3 knockbackVelocity;
        public float knockbackDrag;

        //Gun
        public float gunCooldown;
        
        // Levels
        public GameObject levelParent;
        
        // Enemies
        /// <summary>
        /// Key: enemy spawner object. Value: associated spawner data.
        /// Used by EnemySpawnSystem to spawn enemies + know when a spawner data goes out of scope
        /// </summary>
        public Dictionary<EnemySpawner, EnemySpawnerData> EnemySpawners = new();
        public float TimeOfLastSpawn;

        /// <summary>
        /// A list of enemies in the scene
        /// </summary>
        public HashSet<EnemyObject> Enemies = new();
    }
}

