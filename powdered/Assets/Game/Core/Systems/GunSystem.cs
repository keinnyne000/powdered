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
            
            // TODO: This will be moved to whatever our 'upgrade pickup' system is.
            var mod = new Modifier("Example-BonusDamage-Modifier")
                .Register<ShootGameAction>((a, s) => a.BonusDamage += 1);
            _state.Modifiers.Add(mod);
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

            _state.GunCooldown = _state.CurrentGun.SecondsBetweenShots;
            new ShootGameAction(camera.transform.position, camera.transform.forward).Trigger(_state);
        }
    }
}
