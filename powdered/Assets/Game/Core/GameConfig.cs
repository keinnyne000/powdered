using System;
using Game.Core.Guns;
using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// Game config asset, stores all unity-exposed game settings
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "Game Config", fileName = "newGameConfig")]
    public class GameConfig : ScriptableObject
    {
        // Private fields, protects settings from being accidentally changed at runtime
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GunBehavior startingGun;

        // This pattern exposes a read only method to get each field
        public GameObject PlayerPrefab => playerPrefab;
        public GunBehavior StartingGun => startingGun;
    }
}