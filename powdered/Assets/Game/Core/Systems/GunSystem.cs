using System.Collections.Generic;
using Game.Core.Guns;
using Game.Core.Guns.Upgrades;
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
        readonly AudioSource _audio;

        public GunSystem(GameState state, IInputSource input, GunBehavior gun)
        {
            _input = input;
            _gun = gun;

            _state.gunCooldown = 0f;

            _audio = new GameObject("GunAudio").AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            _state.pistolUpgrades = new List<IUpgrades>
            {
                new IncreaseDamage()
            };

            if (_gun == null)
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
            
            //UPGRADES
            if( _state.pistolUpgrades.Count > 0 )
            {
                _state.pistolUpgrades[0].effect(_state);
                _state.pistolUpgrades.RemoveAt(0);
            }

            bool wantsToFire = _state.CurrentGun.Automatic ? _input.AttackHeld : _input.AttackPressed;
            if (!wantsToFire)
                return;

            Camera camera = Camera.main;
            if (camera == null)
                return;

            

            _gun.Fire(new GunFireContext(camera.transform.position, camera.transform.forward), _state);

            if (_gun.FireSound != null)
                _audio.PlayOneShot(_gun.FireSound, _gun.FireVolume);

            _state.gunCooldown = _gun.SecondsBetweenShots;
            new ShootGameAction(camera.transform.position, camera.transform.forward).Trigger(_state);
        }
    }
}
