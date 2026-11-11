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
        readonly AudioSource _audio;

        public GunSystem(GameState state, IInputSource input, GunBehavior gun)
        {
            _input = input;
            
            _audio = new GameObject("GunAudio").AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            if (gun == null)
                Debug.LogWarning("GunSystem: no Starting Gun set on GameConfig, shooting is disabled");
            _state = state;
            
            // TODO: This will be replaced by whatever system gives us guns!
            state.CurrentGun = gun;
        }

        public void Tick(float deltaTime)
        {
            if (_state.GunCooldown > 0f)
                _state.GunCooldown -= deltaTime;

            if (_state.CurrentGun == null || _state.GunCooldown > 0f)
                return;

            bool wantsToFire = _state.CurrentGun.Automatic ? _input.AttackHeld : _input.AttackPressed;
            if (!wantsToFire)
                return;

            Camera camera = Camera.main;
            if (camera == null)
                return;

            

            if (_state.CurrentGun.FireSound != null)
                _audio.PlayOneShot(_state.CurrentGun.FireSound, _state.CurrentGun.FireVolume);

            var shot = new ShootGameAction(camera.transform.position, camera.transform.forward);
            shot.Trigger(_state);

            // Set after the shot so Fire Rate gun parts can speed it up
            _state.GunCooldown = _state.CurrentGun.SecondsBetweenShots / Mathf.Max(0.01f, shot.FireRateMultiplier);
        }
    }
}
