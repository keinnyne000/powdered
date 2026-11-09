using System;
using Game.Core.Guns;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Core.Systems
{
    /// <summary>
    /// Settings for GunPartRewardSystem. Lives in GameConfig.
    /// </summary>
    [Serializable]
    public class GunPartRewardConfig
    {
        [Tooltip("Enemies you need to kill to earn a random gun part")]
        [SerializeField, Min(1)] public int killsPerGunPart = 3;
    }

    /// <summary>
    /// Gives the player a random gun part every few kills.
    /// Placeholder for a real pickup (drops, chests, etc) later on.
    /// </summary>
    public class GunPartRewardSystem : ISimulationSystem
    {
        readonly GameState _state;
        readonly int _killsPerPart;
        int _nextRewardAt;

        public GunPartRewardSystem(GameState state, GameConfig config)
        {
            _state = state;
            _killsPerPart = Mathf.Max(1, config.GunPartRewardConfig?.killsPerGunPart ?? 3);
            _nextRewardAt = _killsPerPart;
        }

        public void Tick(float deltaTime)
        {
            while (_state.EnemiesKilled >= _nextRewardAt)
            {
                Grant(GunParts.All[Random.Range(0, GunParts.All.Count)]);
                _nextRewardAt += _killsPerPart;
            }
        }

        void Grant(GunPart part)
        {
            _state.GunParts.Add(part);
            _state.Modifiers.Add(part.CreateModifier());
            _state.GunPartCount.Value = _state.GunParts.Count;
            Debug.Log($"Got gun part: {part.Name} ({part.Description}). Total parts: {_state.GunParts.Count}");
        }
    }
}
