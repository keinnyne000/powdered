using Game.Core.Guns;
using UnityEngine;

namespace Game.Core.Systems
{
    /// <summary>
    /// Fires the player's current gun when attack is pressed, respecting fire rate
    /// </summary>
    public class GunSystem : ISimulationSystem
    {
        readonly GameState _state;
        readonly IInputSource _input;
        readonly GunBehavior _gun;

        public GunSystem(GameState state, IInputSource input, GunBehavior gun)
        {
            _state = state;
            _input = input;
            _gun = gun;

            _state.gunCooldown = 0f;

            if (_gun == null)
                Debug.LogWarning("GunSystem: no Starting Gun set on GameConfig, shooting is disabled");
        }

        public void Tick(float deltaTime)
        {
            if (_state.gunCooldown > 0f)
                _state.gunCooldown -= deltaTime;

            if (_gun == null || _state.gunCooldown > 0f)
                return;

            bool wantsToFire = _gun.Automatic ? _input.AttackHeld : _input.AttackPressed;
            if (!wantsToFire)
                return;

            Camera camera = Camera.main;
            if (camera == null)
                return;

            _gun.Fire(new GunFireContext(camera.transform.position, camera.transform.forward), _state);
            _state.gunCooldown = _gun.SecondsBetweenShots;
        }
    }
}
