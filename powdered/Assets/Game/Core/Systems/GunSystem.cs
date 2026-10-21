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
            _state = state;
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
                Debug.LogWarning("GunSystem: no Starting Gun set on GameConfig, shooting is disabled");
        }

        public void Tick(float deltaTime)
        {
            if (_state.gunCooldown > 0f)
                _state.gunCooldown -= deltaTime;

            if (_gun == null || _state.gunCooldown > 0f)
                return;
            
            //UPGRADES
            if( _state.pistolUpgrades.Count > 0 )
            {
                _state.pistolUpgrades[0].effect(_state);
                _state.pistolUpgrades.RemoveAt(0);
            }

            bool wantsToFire = _gun.Automatic ? _input.AttackHeld : _input.AttackPressed;
            if (!wantsToFire)
                return;

            Camera camera = Camera.main;
            if (camera == null)
                return;

            

            _gun.Fire(new GunFireContext(camera.transform.position, camera.transform.forward), _state);

            if (_gun.FireSound != null)
                _audio.PlayOneShot(_gun.FireSound, _gun.FireVolume);

            _state.gunCooldown = _gun.SecondsBetweenShots;
        }
    }
}
