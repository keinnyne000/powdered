using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.Platform
{
    [DisallowMultipleComponent]
    public sealed class App : MonoBehaviour
    {
        private readonly List<ISimulationSystem> _simulation = new List<ISimulationSystem>();
        private readonly List<IPresentationSystem> _presentation = new List<IPresentationSystem>();
        
        private Session _session;
        private GameConfig _gameConfigAsset;
        
        /// <summary>
        /// Registers an ISimulationSystem to be called each frame in order in which it was registered
        /// </summary>
        /// <param name="system"></param>
        public void Register(ISimulationSystem system) => _simulation.Add(system);

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            
            // Load the first game config in an Assets/.../Resources/ folder that is named "GameConfig"
            _gameConfigAsset = Resources.Load<GameConfig>("GameConfig");
        }

        private void Start()
        {
            _session = new Session(this, _gameConfigAsset);
        }

        void Update()
        {
            var dt = Time.deltaTime;
            foreach (var system in _simulation)
                system.Tick(dt);
        }

        private void LateUpdate()
        {
            var dt = Time.deltaTime;
            foreach (var system in _presentation)
                system.Present(dt);
        }
    }
}
